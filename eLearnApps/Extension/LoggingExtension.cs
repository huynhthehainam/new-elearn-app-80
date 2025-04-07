using eLearnApps.Business.Interface;
using eLearnApps.Core;
using eLearnApps.Entity.Logging;
using eLearnApps.Entity.Logging.Dto;
using eLearnApps.ViewModel.Logging;

namespace eLearnApps.Extension
{
    public static class LoggingExtension
    {
        public static void Log(this ILoggingService loggingService, LogType logType, LoggingModel model)
        {
            model.ToolId = model.ToolId ?? "";
            var logData = new Logging();
            switch (logType)
            {
                case LogType.BatchJob:
                    var batchJob = new BatchJobLog
                    {
                        CreatedOn = DateTime.UtcNow,
                        ExecutableName = model.ExecutableName,
                        JobStart = model.JobStart,
                        JobEnd = model.JobEnd,
                        LastUpdatedOn = DateTime.UtcNow,
                        Parameters = model.Parameter
                    };
                    var batchJobDetail = new BatchJobLogDetail
                    {
                        CreatedOn = DateTime.UtcNow,
                        LogContent = model.FullMessage,
                        LogLevel = model.LogLevel
                    };
                    logData.BatchJob = batchJob;
                    logData.BatchJobDetail = batchJobDetail;
                    break;
                case LogType.Error:
                    var error = new ErrorLog
                    {
                        UserId = model.UserId,
                        OrgUnitId = model.OrgUnitId,
                        ToolId = model.ToolId,
                        ToolAccessRoleId = model.RoleId,
                        IpAddress = model.IpAddress,
                        SessionId = model.SessionId,
                        ErrorMessage = model.ShortMessage,
                        ErrorPage = model.Page,
                        ErrorDetails = model.FullMessage,
                        ErrorTime = DateTime.UtcNow
                    };
                    logData.Error = error;
                    break;
                case LogType.TrackLog:
                    var trackLog = new ToolAccessLog
                    {
                        UserId = model.UserId,
                        OrgUnitId = model.OrgUnitId,
                        ToolId = model.ToolId,
                        IpAddress = string.IsNullOrEmpty(model.IpAddress) ? Constants.LocalIpAddress : model.IpAddress,
                        SessionId = model.SessionId,
                        AccessTime = DateTime.UtcNow,
                        OrgUnitRoleId = model.RoleId,
                        Username = string.IsNullOrEmpty(model.UserName) ? "Unknown" : model.UserName
                    };
                    logData.ToolAccess = trackLog;
                    break;
            }

            loggingService.Save(logType, logData);
        }
       
    }
}