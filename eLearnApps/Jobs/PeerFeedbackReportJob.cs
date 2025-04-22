using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using eLearnApps;
using eLearnApps.Business.Interface;
using eLearnApps.Core;
using eLearnApps.Entity.Logging;
using eLearnApps.Models;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using ICacheManager = eLearnApps.Core.Caching.ICacheManager;

public class PeerFeedbackReportJob
{
    private IUserService _userService;
    private ICacheManager _cacheManager;
    private ILoggingService _loggingService;
    private string _baseReportFolderPath;
    private int _commandTimeoutInSeconds;
    private string _sqlConnectionString;
    private eLearnApps.Constants _constants;
    public PeerFeedbackReportJob(
        IServiceProvider serviceProvider)
    {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var constants = new eLearnApps.Constants(configuration);
        _constants = constants;
        _baseReportFolderPath = configuration.GetValue<string>("PeerFeedbackReportBaseFolderPath") ?? "";
        _commandTimeoutInSeconds = constants.PeerFeedbackReportQueryTimeout;
        _sqlConnectionString = configuration.GetValue<string>("DataContext") ?? "";
        _userService = serviceProvider.GetRequiredService<IUserService>();
        _loggingService = serviceProvider.GetRequiredService<ILoggingService>();
        _cacheManager = serviceProvider.GetRequiredService<ICacheManager>();
    }

    public async Task Run(
        PeerFeedBackReportType peerFeedBackReportType,
        PeerFeedBackReportGroupBy peerFeedBackReportGroupBy,
        List<int> sessions,
        string sessionNames,
        List<int> courses,
        string baseReportFolderPath,
        string requestId,
        UserModel triggerByUser,
        string timeZone = "Asia/Singapore",
        CancellationToken cancellationToken = default(CancellationToken)
        )
    {
        var _userInfo = triggerByUser;
        var toEmailAddress = _userInfo.EmailAddress;
        var toName = _userInfo.DisplayName;
        var dateTimeNow = DateTime.Now;

        // admin list
        var peerFeedbackAdminListCsv = _constants.PeerFeedbackAdmins;
        var peerFeedbackAdminEmails = new List<string>();

        string reportNameType = string.Empty;
        switch (peerFeedBackReportType)
        {
            case PeerFeedBackReportType.IndividualStudentResult:
                reportNameType = "Individual Student Result";
                break;
            case PeerFeedBackReportType.OverallResponseRate:
                reportNameType = "Overall Response Rate (" + (peerFeedBackReportGroupBy == PeerFeedBackReportGroupBy.CourseSection ? "Group by Course and Academic Group" : "Group by School Code and Academic Year") + ")";
                break;
            case PeerFeedBackReportType.OverallDescriptorResult:
                reportNameType = "Overall Descriptors Result (" + (peerFeedBackReportGroupBy == PeerFeedBackReportGroupBy.CourseSection ? "Group by Course and Academic Group" : "Group by School Code and Academic Year") + ")";
                break;
            case PeerFeedBackReportType.OverallMeanScoreResult:
                reportNameType = "Overall Mean Score (" + (peerFeedBackReportGroupBy == PeerFeedBackReportGroupBy.CourseSection ? "Group by Course and Academic Group" : "Group by School Code and Academic Year") + ")";
                break;
        }

        var reportParameters = $"ReportType={peerFeedBackReportType} GroupBy={peerFeedBackReportGroupBy} Sessions={string.Join(",", sessions)} Courses={string.Join(",", courses)}";

        try
        {


            if (String.IsNullOrEmpty(peerFeedbackAdminListCsv))
            {
                peerFeedbackAdminEmails.Add("lst_dev@smu.edu.sg");
            }
            else
            {
                var arr = peerFeedbackAdminListCsv.Split(',');
                foreach (string str in arr)
                {
                    int userId = Convert.ToInt32(str);
                    var userInfo = _userService.GetById(userId);
                    if (userInfo != null)
                    {
                        peerFeedbackAdminEmails.Add(userInfo.EmailAddress);
                    }
                }
            }
        }
        catch (Exception ee)
        {
            var errMsg = $"[{requestId}] Unable to start report job - error parsing admin list email. {reportParameters} BaseReportFolderPath={_baseReportFolderPath} SqlConnStr={_sqlConnectionString}";

            var longErrorMessage = $"{errMsg} {ee.Message} {ee.StackTrace}";

            LogDebug(_userInfo.UserId, errMsg);

            _loggingService.Error(
                _userInfo.UserId,
                nameof(ToolName.Psfs),
                0,
                0,
                nameof(PeerFeedbackReportJob),
                errMsg,
                longErrorMessage,
                "",
                "");

            return;
        }

        if (_userInfo == null)
        {
            var errMsg = $"[{requestId}] Unable to start report job - missing userinfo. {reportParameters} BaseReportFolderPath={_baseReportFolderPath} SqlConnStr={_sqlConnectionString}";
            _loggingService.Error(
                _userInfo.UserId,
                nameof(ToolName.Psfs),
                0,
                0,
            nameof(PeerFeedbackReportJob),
                errMsg,
                errMsg,
                "",
                "");
            LogDebug(_userInfo.UserId, errMsg);
            return;
        }


        //overide baseReportFolderPath if passed
        if (!String.IsNullOrEmpty(baseReportFolderPath))
            this._baseReportFolderPath = baseReportFolderPath;

        LogDebug(_userInfo.UserId, $"[{requestId}] - Started Report Job {reportParameters} BaseReportFolderPath={_baseReportFolderPath} SqlConnStr={_sqlConnectionString}");

        try
        {
            // Send initial email to user
            // ----------------------------------------
            var emailHelper = new EmailHelper(_userService, _loggingService, _cacheManager, _userInfo.UserId, 0);
            var emailSubject = $"[Peer & Self Feedback] Acknowledgment of Report Processing Request - Reference ID: {requestId}";
            var emailTemplateAcknowledge = $"acknowledge_template.html";
            var emailBodyAcknowledge = emailHelper.GetEmailTemplate(emailTemplateAcknowledge);

            if (!string.IsNullOrEmpty(emailBodyAcknowledge))
            {
                emailBodyAcknowledge = emailBodyAcknowledge.Replace("{PSFSSessions}", sessionNames);
                emailBodyAcknowledge = emailBodyAcknowledge.Replace("{Recipient}", toName);
                emailBodyAcknowledge = emailBodyAcknowledge.Replace("{ReferenceID}", requestId);
                emailBodyAcknowledge = emailBodyAcknowledge.Replace("{ReportNameType}", reportNameType);
                emailBodyAcknowledge = emailBodyAcknowledge.Replace("{RequestDate}", dateTimeNow.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture));
            }

            var isInitialMailSent = emailHelper.SendMail(
                toEmailAddress,
                toName,
                _constants.SystemMailAddress,
                _constants.SystemMailName,
                emailSubject,
                emailBodyAcknowledge,
                true);

            if (isInitialMailSent)
                LogDebug(_userInfo.UserId, $"[{requestId}] - Initial email sent to {_userInfo.EmailAddress} {reportParameters}");
            else
            {
                LogDebug(_userInfo.UserId, $"[{requestId}] - Aborting job, unable to send initial email to {_userInfo.EmailAddress} {reportParameters}");
                return;
            }

            // run stored procedure
            // ----------------------------------------

            LogDebug(_userInfo.UserId, $"[{requestId}] - Querying Report Data started. {reportParameters}");
            var reportData = GetReportData(peerFeedBackReportType, peerFeedBackReportGroupBy, sessions, courses);
            LogDebug(_userInfo.UserId, $"[{requestId}] - Querying Report Data completed. {reportParameters}");

            // generate report file
            // ----------------------------------------

            // simulate 10 minutes job.
            //var dateTimeNowPlus10Minutes = DateTime.Now.AddMinutes(10);
            //while (DateTime.Now < dateTimeNowPlus10Minutes)
            //{
            //    if (cancellationToken.IsCancellationRequested)
            //    {
            //        LogDebug(_userInfo.UserId, $"[{requestId}] - Job cancelled by user.");
            //        return;
            //    }
            //    LogDebug(_userInfo.UserId, $"[{requestId}] - TickTock");
            //    await Task.Delay(10000);
            //}

            var fileName = GetPeerFeedBackTemplateFileName(peerFeedBackReportType, peerFeedBackReportGroupBy, requestId);

            // delete existing file if exists.
            if (File.Exists(fileName))
                File.Delete(fileName);

            ExportToExcel(reportData, fileName);
            LogDebug(_userInfo.UserId, $"[{requestId}] - Excel file generated {fileName}. {reportParameters}");


            // email with attachment
            // ----------------------------------------
            emailHelper = new EmailHelper(_userService, _loggingService, _cacheManager, _userInfo.UserId, 0);
            emailSubject = $"[Peer & Self Feedback] Your Report is Ready for Download - Reference ID: {requestId}";
            var emailTemplate = $"report_ready_template.html";
            var emailBody = emailHelper.GetEmailTemplate(emailTemplate);

            if (!string.IsNullOrEmpty(emailBody))
            {
                emailBody = emailBody.Replace("{PSFSSessions}", sessionNames);
                emailBody = emailBody.Replace("{Recipient}", toName);
                emailBody = emailBody.Replace("{ReferenceID}", requestId);
                emailBody = emailBody.Replace("{ReportNameType}", reportNameType);
                emailBody = emailBody.Replace("{RequestDate}", dateTimeNow.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture));
                emailBody = emailBody.Replace("{CompletionDate}", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture));
            }
            var attachment = new System.Net.Mail.Attachment(fileName);
            var isFinalEmailSent = emailHelper.SendMailWithAttachment(
                ToolName.Psfs,
                toEmailAddress,
                toName,
                _constants.SystemMailAddress,
                _constants.SystemMailName,
                emailSubject,
                emailBody,
                attachment,
                true);

            if (isFinalEmailSent)
                LogDebug(_userInfo.UserId, $"[{requestId}] - Completed - Final email sent to {_userInfo.EmailAddress} with report. {reportParameters}");
            else
                LogDebug(_userInfo.UserId, $"[{requestId}] - Completed with WARNING - Failed to send final email to {_userInfo.EmailAddress} with report. {reportParameters}");

        }
        catch (Exception ex)
        {


            var emailHelper = new EmailHelper(_userService, _loggingService, _cacheManager, _userInfo.UserId, 0);
            var emailSubject = $"Peer Feedback - Error generating report {peerFeedBackReportType} requested on {dateTimeNow} by {_userInfo.DisplayName} with request ID {requestId}";
            var emailTemplate = $"report_ready_template.html";
            var emailBody = $"Error processing your request to generate report {peerFeedBackReportType} requested on {dateTimeNow}.\n Error Message: {ex.Message} \n Error Stacktrace: {ex.StackTrace}";

            _loggingService.Error(
                _userInfo.UserId,
                nameof(ToolName.Psfs),
                0,
                0,
                nameof(PeerFeedbackReportJob),
                $"[{requestId}] {emailSubject}",
                emailBody,
                "",
                "");

            LogDebug(_userInfo.UserId, $"[{requestId}] - Error - Failed processing report. {reportParameters}");

            emailHelper.SendMailMultiple(
                peerFeedbackAdminEmails,
                _constants.SystemMailAddress,
                _constants.SystemMailName,
                emailSubject,
                emailBody,
                false);



            return;
        }
    }

    public void ExportToExcel(DataSet reportData, string filePath)
    {
        IEnumerable<string> percentageColumns = _constants.ReportPercentageColumns.Split(',');
        using (var workbook = SpreadsheetDocument.Create(filePath, SpreadsheetDocumentType.Workbook))
        {
            var workbookPart = workbook.AddWorkbookPart();
            workbook.WorkbookPart.Workbook = new Workbook();
            workbook.WorkbookPart.Workbook.Sheets = new Sheets();

            var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
            var sheetData = new SheetData();
            sheetPart.Worksheet = new Worksheet(sheetData);

            var sheets = workbook.WorkbookPart.Workbook.GetFirstChild<Sheets>();
            var relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);

            var sheetName = "Report";
            var sheet = new Sheet() { Id = relationshipId, SheetId = (UInt32Value)1U, Name = sheetName };
            sheets.Append(sheet);

            var headerRow = new Row();

            foreach (DataColumn column in reportData.Tables[0].Columns)
            {
                if (_constants.ReportHideColumns.IndexOf(column.ColumnName, StringComparison.OrdinalIgnoreCase) > -1) continue;

                var percentageSuffix = column.ColumnName.IndexOf("Percentage", StringComparison.OrdinalIgnoreCase) > -1 || percentageColumns.Contains(column.ColumnName) ? " (%)" : string.Empty;
                var columnName = $"{column.ColumnName}{percentageSuffix}";
                var cell = new Cell
                {
                    DataType = CellValues.String,
                    CellValue = new CellValue(columnName)
                };
                headerRow.AppendChild(cell);
            }
            sheetData.AppendChild(headerRow);
            foreach (DataRow row in reportData.Tables[0].Rows)
            {
                var newRow = new Row();
                foreach (DataColumn column in reportData.Tables[0].Columns)
                {
                    if (_constants.ReportHideColumns.IndexOf(column.ColumnName, StringComparison.OrdinalIgnoreCase) > -1) continue;

                    var data = row[column].ToString();
                    if (data.Contains("%"))
                        data = data.Replace("%", "").Trim();
                    bool isNumber = double.TryParse(data, out double _);
                    var cell = new Cell
                    {
                        CellValue = new CellValue(data),
                        DataType = isNumber ? CellValues.Number : CellValues.String
                    };
                    newRow.AppendChild(cell);
                }
                sheetData.AppendChild(newRow);
            }
        }
    }

    public string GetPeerFeedBackTemplateFileName(PeerFeedBackReportType peerFeedBackReportType, PeerFeedBackReportGroupBy peerFeedBackReportGroupBy, string requestId, string baseReportFolderPath = "", string timeZone = "Asia/Singapore")
    {
        string fileName = string.Empty;
        var reportType = peerFeedBackReportType;
        var groupBy = peerFeedBackReportGroupBy == PeerFeedBackReportGroupBy.CourseSection ? "GroupByCourseAcademicGroup" : "GroupBySchoolCodeAcademicYear";
        switch (reportType)
        {
            case PeerFeedBackReportType.IndividualStudentResult:
                fileName = "IndividualStudentResult";
                break;
            case PeerFeedBackReportType.OverallResponseRate:
                fileName = $"ResponseRate_{groupBy}";
                break;
            case PeerFeedBackReportType.OverallDescriptorResult:
                fileName = $"Descriptors_{groupBy}";
                break;
            case PeerFeedBackReportType.OverallMeanScoreResult:
                fileName = $"MeanScore_{groupBy}";
                break;
        }

        //default using appsetting baseReportFolderPath OR when passed in DoJob.        
        var _baseFolder = _baseReportFolderPath;

        //if passed in this method, then use it.
        if (!String.IsNullOrEmpty(baseReportFolderPath))
            _baseFolder = baseReportFolderPath;
        //var tz = TZConvert.IanaToWindows(timeZone);
        //var clientTz = TimeZoneInfo.FindSystemTimeZoneById(tz);
        //var currentTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, clientTz);
        //var fullFileName = $"{fileName}_{currentTime.ToString("yyyyMMdd")}_{currentTime.ToString("HHmmss")}.xlsx";
        var fullFileName = $"{fileName}_{requestId}.xlsx";
        return $@"{_baseFolder}\{fullFileName}";
    }

    public DataSet GetReportData(
        PeerFeedBackReportType peerFeedBackReportType,
        PeerFeedBackReportGroupBy peerFeedBackReportGroupBy,
        List<int> sessions, List<int> courses)
    {

        //TODO: add in final PROC NAME and PARAMETERS

        var procName = "";
        switch (peerFeedBackReportType)
        {
            case PeerFeedBackReportType.IndividualStudentResult:
                procName = "SP_StudentResult";
                break;
            case PeerFeedBackReportType.OverallResponseRate:
                procName = peerFeedBackReportGroupBy == PeerFeedBackReportGroupBy.CourseSection
                    ? "SP_OverallResponseRateByCourseAcadGroup"
                    : "SP_OverallResponseRateBySchoolCodeAcadYear";
                break;
            case PeerFeedBackReportType.OverallDescriptorResult:
                procName = peerFeedBackReportGroupBy == PeerFeedBackReportGroupBy.CourseSection
                    ? "SP_DescriptorResponseByCourseAcadGroup"
                    : "SP_DescriptorResponseBySchoolCodeAcadYear";
                break;
            case PeerFeedBackReportType.OverallMeanScoreResult:
                procName = peerFeedBackReportGroupBy == PeerFeedBackReportGroupBy.CourseSection
                    ? "SP_MeanScoreByCourseAcadGroup"
                    : "SP_MeanScoreBySchoolCodeAcadYear";
                break;
        }

        DataSet result = null;
        SqlConnection sqlConn = null;
        // call stored procedure procName with timeout of 5 minutes and read the results into a dataset 

        using (sqlConn = new SqlConnection(_sqlConnectionString))
        {
            using (var cmd = new SqlCommand(procName, sqlConn))
            {
                using (var sda = new SqlDataAdapter(cmd))
                {
                    cmd.CommandTimeout = _commandTimeoutInSeconds;
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@SessionIds", string.Join(",", sessions));
                    cmd.Parameters.AddWithValue("@CourseIds", string.Join(",", courses));

                    result = new DataSet();
                    sda.Fill(result);
                }
            }
        }

        return result;
    }

    private void LogDebug(int userId, string message)
    {
        var loggingModel = new DebugLog();

        loggingModel.ShortMessage = message != null && message.Length > 500 ? message.Substring(0, 500) : message;


        loggingModel.FullMessage = message;

        loggingModel.PageUrl = nameof(PeerFeedbackReportJob);
        loggingModel.UserId = userId;
        loggingModel.OrgUnitId = 0;
        loggingModel.ToolId = nameof(ToolName.Psfs);
        _loggingService.Debug(loggingModel);
    }
}