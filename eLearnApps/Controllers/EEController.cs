using eLearnApps.Business.Interface;
using eLearnApps.Core;
using eLearnApps.Core.Caching;
using eLearnApps.CustomAttribute;
using eLearnApps.Entity.Logging;
using eLearnApps.Entity.Security;
using eLearnApps.Valence;
using eLearnApps.Helpers;
using eLearnApps.ViewModel.Valence;
using System.IO.Compression;
using System.Text;
using System.Web.Hosting;
using OrgUnitUser = eLearnApps.Entity.Valence.OrgUnitUser;
using System.Text.RegularExpressions;
using System.Net.Http.Headers;
using eLearnApps.Entity.Logging.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;
using System.Xml;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using eLearnApps.ViewModel.EE;
using eLearnApps.Models;
using eLearnApps.Models.GptZeroModels;

namespace eLearnApps.Controllers
{
    [Authorize, TrackingLog]
    public class EEController : BaseController
    {
        private readonly ICourseService _courseService;
        private readonly IIcsService _icsService;
        private readonly ICmtService _cmtService;
        private readonly IUserService _userService;
        private readonly ILoggingService _loggingService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        private readonly Constants _constants;

        private readonly string[] sizes = new string[4] { "10", "11", "12", "14" };
        private readonly string[] spaces = new string[4] { "1.0", "1.15", "1.5", "2.0" };

        readonly ValenceApi vapi;

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static List<char> allowedCharacters = new List<char>("1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz.{}:\\\"");

        public EEController(IIcsService icsService,
            ICourseService courseService,
            ICmtService cmtService,
            ILoggingService loggingService,
            IUserService userService,
            ICacheManager cacheManager,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration,
            IServiceProvider serviewProvider,
            ICompositeViewEngine compositeViewEngine,
            IWebHostEnvironment webHostEnvironment,
            IErrorLogService errorLogService,
            IServiceScopeFactory serviceScopeFactory) : base(cacheManager, errorLogService, httpContextAccessor, configuration, serviewProvider, compositeViewEngine)
        {
            _icsService = icsService;
            _courseService = courseService;
            _cmtService = cmtService;
            _userService = userService;
            _loggingService = loggingService;
            _webHostEnvironment = webHostEnvironment;
            _constants = new Constants(configuration);
            _configuration = configuration;
            vapi = new ValenceApi(configuration);
            _serviceScopeFactory = serviceScopeFactory;
        }
        [HttpGet, ClaimRequirement(nameof(StandardPermissionProvider.ManageExtraction))]
        public IActionResult Index()
        {
            var userGuideRelativePath = "~/Content/guide/EE_Instructor_UserGuide.pdf";
            string userGuideFilePath = Path.Combine(_webHostEnvironment.WebRootPath, userGuideRelativePath.TrimStart('/'));

            if (System.IO.File.Exists(userGuideFilePath))
            {
                ViewBag.UserGuide = Url.Content(userGuideRelativePath);
            }
            return View();
        }

        [HttpGet, ClaimRequirement(nameof(StandardPermissionProvider.ManageExtraction))]
        public IActionResult _Options()
        {
            //get grade object id
            var quizObjects = vapi.GetQuizObjectsByCourseId(CourseId);
            LogDebug("examextraction-_options: GetQuizObjectsByCourseId", $"{CourseId}");

            LogDebug("examextraction-_options: inspect quiz objects return by WS", $"{JsonSerializer.Serialize(quizObjects)}");

            var quizzes = new List<SelectListItem>();

            // Auto-select the last item on the list
            int i = 1;
            foreach (var quiz in quizObjects.Objects.ToList())
            {
                quizzes.Add(new SelectListItem()
                {
                    Text = quiz.Name,
                    Value = quiz.QuizId.ToString(),
                    Selected = i == quizObjects.Objects.ToList().Count() ? true : false
                });

                LogDebug("examextraction-_options: Added quiz as a select item", $"Text:{quiz.Name}, Value:{quiz.QuizId.ToString()}, Index:{i}");
                i++;
            }

            LogDebug("examextraction-_options: inspect final selectlist", $"{JsonSerializer.Serialize(quizzes)}");
            var fontSizes = new List<SelectListItem>();
            var lineSpaces = new List<SelectListItem>();

            foreach (var size in sizes)
            {
                fontSizes.Add(new SelectListItem()
                {
                    Text = size.ToString(),
                    Value = size.ToString(),
                    Selected = size == "11" ? true : false
                });
            }

            foreach (var space in spaces)
            {
                lineSpaces.Add(new SelectListItem()
                {
                    Text = space.ToString(),
                    Value = space.ToString(),
                    Selected = space == "1.15" ? true : false
                });
            }
            var sections = new List<SelectListItem>();
            sections.AddRange(vapi.GetSectionByCourseId(CourseId)?.Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = s.SectionId.ToString(),
            }).ToList() ?? new List<SelectListItem>());
            if (sections.Count > 1)
            {
                sections.Insert(0, new SelectListItem
                {
                    Value = "",
                    Text = "All"
                });
            }

            var vm = new CreateExtractionViewModel
            {
                Quizzes = quizzes,
                FontSizes = fontSizes,
                Sections = sections,
                LineSpaces = lineSpaces,
                CourseId = CourseId,
                EnableGPTZeroOption = _constants.EnableGPTZeroOption,
                QuestionTypes = GetQuestionType()
            };

            LogDebug("examextraction-_options: inspect return vm", $"{JsonSerializer.Serialize(vm)}");

            return PartialView("_Options", vm);
        }

        [HttpPost]
        [ClaimRequirement(nameof(StandardPermissionProvider.ManageExtraction))]
        [ValidateAntiForgeryToken]
        [DeleteFile]
        public async Task<ActionResult> CreateExtraction(CreateExtractionViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.FontSize) || !sizes.Contains(model.FontSize))
                model.FontSize = sizes.First();

            if (string.IsNullOrWhiteSpace(model.LineSpace) || !spaces.Contains(model.LineSpace))
                model.LineSpace = spaces.First();

            LogDebug("examextraction-createextraction: inspect viewmodel", $"{JsonSerializer.Serialize(model)}");

            bool isValidQuiz = int.TryParse(model.QuizId, out int quizId);

            if (!isValidQuiz)
                return Json("");

            if (model.GroupBy == (int)ExamExtractionGroupBy.Question)
            {
                model.IsQuestionShown = true;
                model.ExportOption = (int)ExportOption.AllToOneDocument;
            }

            // Get section(s) info
            var sections = GetSectionsByCourseId(model.CourseId, model.SortBy);

            LogDebug("examextraction-createextraction: inspect sections", $"{JsonSerializer.Serialize(sections)}");

            List<UserResponse> allResponses = new List<UserResponse>();

            StringBuilder sbTextResponse = new StringBuilder();
            // Loop over each section and over each user to get their responses
            var listUserResponse = new List<UserResponse>();
            var studentAnswers = new Dictionary<int, string>();
            //var isFirst = true;
            foreach (var section in sections)
            {
                LogDebug("examextraction-createextraction: inspect section.Enrollments", $"{JsonSerializer.Serialize(section.Enrollments)}");

                foreach (var userIds in section.Enrollments)
                {
                    var quizResponse = vapi.GetQuizResponseByUserId(model.CourseId, quizId, userIds);
                    quizResponse.SectionName = section.Name;
                    //if (isFirst)
                    //{
                    //    isFirst = false;
                    //    quizResponse.Responses[0].TextResponse = @"<span color: yellow>The environment is the foundation of life on Earth, encompassing the air we breathe, the water we drink, the land we inhabit, and the ecosystems that sustain us. As our global population continues to grow and industrialization accelerates, the strain on our natural resources and ecosystems has reached alarming levels. Protecting the environment is not just a matter of preserving natural beauty; it is essential for our survival and the well-being of future generations. </span>
                    //Diners who are keen to buy unsold food from eateries at up to 80 per cent off the retail price before closing time, instead of letting it go to waste, can tap a new mobile phone application.
                    //<b>1. Human activities, particularly over the last century, have led to significant environmental degradation. </b>
                    //<b>2. Deforestation, pollution, and climate change are some of the most pressing issues we face today.</b>
                    //<span color: yellow>Customers who use the app can order a “surprise bag” in advance, and they can get any type of unsold food from the menu at day’s end for between 50 per cent and 80 per cent off the usual retail price. </span>
                    //<b>Food waste is a growing problem. With rising inflation, it has put pressure on both consumers and businesses.</b>
                    //Cafe chain Paul in response to queries said the decision to partner with Yindii is its “first step in exploring alternative ways to promote and reduce wastage”.
                    //And while uptake was initially slow – the app is still quite new in Singapore – interest in the surprise bags has been steady across locations, with some stores experiencing an increase of about 10 per cent, Paul added.
                    //";
                    //}
                    //var appdatapath = Server.MapPath("~/App_Data/");
                    //string text = System.IO.File.ReadAllText($"{appdatapath}StudentAnswer.txt");
                    //quizResponse.Responses[0].TextResponse = text;
                    quizResponse.Responses = quizResponse.Responses.OrderBy(x => x.QuestionNumber).ToList();
                    if (quizResponse.Responses.Count > 0)
                    {
                        var longAnswers = quizResponse.Responses.Where(x => x.QuestionNumber > 0
                                                                            && !string.IsNullOrEmpty(x.TextResponse)
                                                                            && string.Equals(x.QuestionType, "Long answer", StringComparison.OrdinalIgnoreCase)).ToList();
                        if (longAnswers != null && longAnswers.Any())
                        {
                            foreach (var item in longAnswers)
                            {
                                if (string.IsNullOrEmpty(item.TextResponse)) continue;

                                log.Debug($"UserId = {userIds}, Awnswer: {item.TextResponse}");
                                if (studentAnswers.ContainsKey(userIds))
                                {
                                    studentAnswers[userIds] += $"\r\n{item.TextResponse}";

                                }
                                else
                                {
                                    studentAnswers.Add(userIds, item.TextResponse);
                                }
                            }
                        }
                    }
                    listUserResponse.Add(quizResponse);
                }
            }
            List<int> studentsTitle = new List<int>();
            //Highlight each sentence possibly generated by AI Tools (e.g ChatGPT)
            string gptZeroTitle = string.Empty;
            if (model.AdditionalExportOptions == 1 && studentAnswers.Any() && _constants.EnableGPTZeroOption)
            {

                var content = await RequestGptZero(studentAnswers);
                //var contentJson = JsonConvert.SerializeObject(content);
                // for get reponse of GPT without calling API
                // get response once, copy json content to GptResp.json file, comment the line above, activate these commented lines
                //var appdatapath = Server.MapPath("~/App_Data/");
                //string jsonText = System.IO.File.ReadAllText($"{appdatapath}GptResp.json");
                //var content = JsonConvert.DeserializeObject<GPTZeroModel>(jsonText);
                var logGPTResult = _configuration.GetValue<bool>("LogGPTResponse");
                if (logGPTResult)
                    log.Info("examextraction-createextraction: inspect gpt-response: " + $"{JsonSerializer.Serialize(content)}");


                //var stopWatch = new Stopwatch();
                //stopWatch.Start();

                if (content != null)
                {
                    var result = content;
                    UpdateUserResponseWithGPTResult(ref listUserResponse, ref result, ref gptZeroTitle, ref model, ref studentsTitle);
                }
                //var resultJson = JsonConvert.SerializeObject(listUserResponse);
                //stopWatch.Stop()
                //var executionTime = stopWatch.ElapsedTicks;
            }
            allResponses.AddRange(listUserResponse);

            // Filter responses
            var responses = allResponses
                            .Where(q => q.HasResponse)
                            .OrderBy(q => q.SectionName)
                            .ToList();

            var selectedQuiz = vapi.GetQuizObjectByQuizId(model.CourseId, quizId);
            var courseDetail = vapi.GetCourceDetailById(model.CourseId);
            LogDebug("examextraction-createextraction: inspect selected quiz info", $"{JsonSerializer.Serialize(selectedQuiz)}");

            var extractionOptions = new ExtractionInfoModel
            {
                ExportOption = model.ExportOption,
                GptZeroTitle = gptZeroTitle,
                GtpZeroTitleCondition = studentsTitle,
                CourseCode = courseDetail.Code,
                CourseId = CourseId,
                QuizName = selectedQuiz.Name,
                QuizId = quizId,
                IsQuestionShown = model.IsQuestionShown,
                IsStudentNameShown = model.IsStudentNameShown,
                FontSize = model.FontSize,
                LineSpacing = model.LineSpace,
                Sections = sections,
                ClientTimezone = model.ClientTimezone,
                SortBy = model.SortBy,
                GroupBy = model.GroupBy,
                IsOutputZipped = model.GroupBy == (int)ExamExtractionGroupBy.Student && model.ExportOption == (int)ExportOption.OneDocumentPerStudent,
                QuestionTypes = model.QuestionTypes.Where(x => x.IsSelected).Select(x => x.Name).ToList(),
                SectionId = model.SectionId,
                SectionName = model.SectionName,
            };
            if (model.AdditionalExportOptions == 1 && _constants.EnableGPTZeroOption)
                GptZeroInsertAuditLog(CourseId, UserInfo.UserId);
            LogDebug("examextraction-createextraction: inspect extractionoptions", $"{JsonSerializer.Serialize(extractionOptions)}");
            return DownloadFile(responses.ToList(), extractionOptions);
        }

        #region private support functions

        private List<QuestionTypeViewModel> GetQuestionType()
        {
            var result = new List<QuestionTypeViewModel>();
            var questionTypes = _constants.QuestionTypes.Split(',').ToList();
            foreach (var item in questionTypes)
            {
                result.Add(new QuestionTypeViewModel { IsSelected = false, Name = item });
            }
            result[0].IsSelected = true;
            return result;
        }

        private string FilterWhitelist(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return string.Empty;

            string result = string.Concat(fileName.Where(c => allowedCharacters.Contains(c)));
            return result;
        }

        private List<ViewModel.Valence.SectionData> GetSectionsByCourseId(int courseId, int orderBy)
        {
            // Get section(s) info
            var sections = vapi.GetSectionsByCourseId(courseId);
            LogDebug("examextraction-getsectionsbycourseid: deprec-inspect sections returned by WS", $"{JsonSerializer.Serialize(sections)}");


            // Get classlist info as well
            var classlist = vapi.GetEnrolledUser(courseId);
            if (orderBy == (int)ExamExtractionSortBy.Name)
            {
                classlist = classlist.OrderBy(x => x.User.DisplayName).ToList();
            }
            else if (orderBy == (int)ExamExtractionSortBy.OrgDefinedId)
            {
                classlist = classlist.OrderBy(x => x.User.OrgDefinedId).ToList();
            }
            LogDebug("examextraction-getsectionsbycourseid: inspect classlist returned by WS", $"{JsonSerializer.Serialize(classlist)}");

            List<ViewModel.Valence.SectionData> result = null;

            var students = classlist
                            .Where(q => string.Equals(q.Role.Name, "Student", StringComparison.OrdinalIgnoreCase) || string.Equals(q.Role.Name, "Audit Student", StringComparison.OrdinalIgnoreCase))
                            .Select(q => int.Parse(q.User.Identifier))
                            .ToList();
            classlist.ForEach(c =>
            {
                var section = sections.FirstOrDefault(s => s.Enrollments.Any(e => e == int.Parse(c.User.Identifier)));
                if (section != null)
                {
                    c.User.SetSection(section.Name, section.SectionId);
                }
            });
            classlist = classlist.Where(e => e.User.GetSectionId() != 0).ToList();

            result = new List<ViewModel.Valence.SectionData>()
            {
                new ViewModel.Valence.SectionData
                {
                    Description = null,
                    Enrollments = students,
                    Name = "DefaultSection",
                    SectionId = -1,
                    Classlist = classlist
                }
            };

            LogDebug("examextraction-getsectionsbycourseid: inspect result", $"{JsonSerializer.Serialize(classlist)}");
            return result;
        }

        private string GetSafeFilename(ExtractionInfoModel options)
        {
            var maxQuizNameLength = Convert.ToInt32(_configuration.GetValue<string>("MaxQuizNameLength"));
            String filename = $"{options.CourseCode}" + (string.IsNullOrEmpty(options.SectionName) ? "" : $" - {options.SectionName}") + $" - {ShortenString(options.QuizName, maxQuizNameLength)}.docx";

            // remove invisible unicode character. it will cause breakage
            var regex = new Regex(@"[\p{Cc}\p{Cf}\p{Mn}\p{Me}\p{Zl}\p{Zp}]");
            filename = regex.Replace(filename, "");

            var encodedDocName = EncodeFileName(filename);

            return encodedDocName;
        }
        private Dictionary<string, int> _fileNameMap = new Dictionary<string, int>();
        private string GetSafeFilename(ExtractionInfoModel options, eLearnApps.Entity.Valence.User student)
        {
            var studentIdentifier = string.Empty;
            if (options.IsStudentNameShown)
            {
                if (student != null)
                    studentIdentifier = string.IsNullOrWhiteSpace(student.DisplayName) ? string.Empty : student.DisplayName.Trim();
            }
            else
            {
                // use campus id when not showing name
                if (student != null)
                    studentIdentifier = string.IsNullOrWhiteSpace(student.OrgDefinedId) ? string.Empty : student.OrgDefinedId.Trim();
            }
            var maxQuizNameLength = Convert.ToInt32(_configuration.GetValue<string>("MaxQuizNameLength"));
            var maxStudentNameLength = Convert.ToInt32(_configuration.GetValue<string>("MaxStudentNameLength"));
            var studentName = ShortenString(studentIdentifier, maxStudentNameLength);
            if (_fileNameMap.ContainsKey(studentName))
            {
                var orderValue = _fileNameMap[studentName];
                _fileNameMap[studentName] = orderValue + 1;
                studentName = studentName + $"({orderValue})";
            }
            else
            {
                _fileNameMap.Add(studentName, 1);
            }
            String filename = options.CourseCode + (string.IsNullOrEmpty(options.SectionName) ? "" : $" - {student.GetSectionName()}") + " - " + ShortenString(options.QuizName, maxQuizNameLength) + $" - " + studentName + ".docx";
            var encodedDocName = EncodeFileName(filename);
            return encodedDocName;
        }
        private string ShortenString(string text, int maxLength)
        {
            return text.Substring(0, Math.Min(maxLength, text.Length)) + (text.Length > maxLength ? "..." : "");
        }

        private string EncodeFileName(string filename)
        {
            // max filename lenght is 128
            if (filename.Length > 128)
                filename = filename.Substring(0, 128);
            StringBuilder sb = new StringBuilder(filename);

            // replace unsave char with underscore
            string encodedDocName =
                sb
                  .Replace(';', '_')
                  .Replace('"', '_')
                  .Replace('/', '_')
                  .Replace(' ', '_')
                  .Replace(',', '_')
                  .Replace(':', '_')
                  .ToString();

            LogDebug("examextraction-getsafefilename: inspect filename", $"{JsonSerializer.Serialize(encodedDocName)}");
            return encodedDocName;
        }



        private void SaveMemoryStream(MemoryStream ms, string fileName)
        {
            using (FileStream outStream = System.IO.File.OpenWrite(fileName))
            {
                ms.WriteTo(outStream);
            }
        }
        [NonAction]
        private ActionResult DownloadFile(List<UserResponse> userResponses, ExtractionInfoModel options)
        {
            var groupBy = (ExamExtractionGroupBy)options.GroupBy;
            var sessionId = HttpContext.Session.Id;
            var uploadPath = Path.Combine("wwwroot", "Content", "Upload", "EE", sessionId);
            var filename = GetSafeFilename(options);
            var fullUploadPath = Path.Combine(Directory.GetCurrentDirectory(), uploadPath);
            var outputPath = Path.Combine(fullUploadPath, filename);


            LogDebug("examextraction-downloadfile",
                $"filename: {(filename)}, " +
                $"path: {(fullUploadPath)}" +
                $"outputPath: {(outputPath)}");

            if (options.IsOutputZipped)
            {
                var zipPath = Path.Combine(fullUploadPath, DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
                var extractionOptions = new ExtractionInfoModel
                {
                    GptZeroTitle = options.GptZeroTitle,
                    CourseCode = options.CourseCode,
                    CourseId = options.CourseId,
                    QuizName = options.QuizName,
                    QuizId = options.QuizId,
                    IsQuestionShown = options.IsQuestionShown,
                    IsStudentNameShown = options.IsStudentNameShown,
                    FontSize = options.FontSize,
                    LineSpacing = options.LineSpacing,
                    ClientTimezone = options.ClientTimezone,
                    SortBy = options.SortBy,
                    GroupBy = options.GroupBy,
                    QuestionTypes = options.QuestionTypes,
                    SectionName = options.SectionName,
                };

                Directory.CreateDirectory(fullUploadPath);
                Directory.CreateDirectory(zipPath);

                string gptZeroTitle = extractionOptions.GptZeroTitle;

                foreach (var section in options.Sections)
                {
                    var responses = userResponses.Where(q => q.SectionName == section.Name).ToList();
                    var fullQuestions = vapi.GetQuizQuestions(options.CourseId, options.QuizId)
                        .Select((e, i) => new ExtractQuestion
                        {
                            QuestionNumber = i + 1,
                            QuestionText = e.QuestionText.Html,
                            QuestionType = e.QuestionTypeId.GetDescription()
                        })
                        .Where(e => extractionOptions.QuestionTypes.Contains(e.QuestionType))
                        .ToList();

                    var selectedStudents = section.Classlist
                        .Where(e => !options.SectionId.HasValue || e.User.GetSectionId() == options.SectionId.Value)
                        .OrderByDescending(e =>
                        {
                            var responseByStudent = userResponses.Where(x => x.StudentId == Convert.ToInt32(e.User.Identifier)).ToList();
                            return responseByStudent.Any() ? 1 : 0;
                        })
                        .ThenBy(e => e.User.StudentSectionNameNumber)
                        .ToList();

                    foreach (var student in selectedStudents)
                    {
                        extractionOptions.Sections = new List<SectionData>
                {
                    new SectionData
                    {
                        Classlist = new List<OrgUnitUser>{ student },
                        Name = section.Name,
                        SectionId = section.SectionId
                    }
                };

                        if (options.ExportOption == (int)ExportOption.OneDocumentPerStudent && !string.IsNullOrEmpty(gptZeroTitle))
                        {
                            extractionOptions.GptZeroTitle = options.GtpZeroTitleCondition.Any() &&
                                options.GtpZeroTitleCondition.Contains(Convert.ToInt32(student.User.Identifier))
                                ? gptZeroTitle
                                : string.Empty;
                        }

                        using var ms = new MemoryStream();
                        OpenXmlHelper openXmlHelper = new OpenXmlHelper(_configuration);
                        openXmlHelper.CreateDoc(ms, responses, fullQuestions, extractionOptions);
                        var studentFileName = GetSafeFilename(options, student.User);
                        outputPath = Path.Combine(zipPath, studentFileName);
                        SaveMemoryStream(ms, outputPath);
                    }
                    filename = GetSafeFilename(options).Replace(".docx", ".zip");
                    var fullFilename = $"{fullUploadPath}/{filename}";
                    ZipFile.CreateFromDirectory(zipPath, fullFilename);
                    var contentDisposition = new System.Net.Mime.ContentDisposition
                    {
                        FileName = filename,
                        Inline = true
                    };
                    var bytes = System.IO.File.ReadAllBytes(fullFilename);
                    Response.Headers["Content-Disposition"] = contentDisposition.ToString();
                    return File(bytes, "application/zip");
                }
            }
            else
            {
                using var ms = new MemoryStream();

                foreach (var section in options.Sections)
                {
                    var responses = userResponses.Where(q => q.SectionName == section.Name).ToList();
                    var fullQuestions = vapi.GetQuizQuestions(options.CourseId, options.QuizId)
                        .Select((e, i) => new ExtractQuestion
                        {
                            QuestionNumber = i + 1,
                            QuestionText = e.QuestionText.Html,
                            QuestionType = e.QuestionTypeId.GetDescription()
                        })
                        .Where(e => options.QuestionTypes.Contains(e.QuestionType))
                        .ToList();

                    if (groupBy == ExamExtractionGroupBy.Student)
                    {
                        OpenXmlHelper openXmlHelper = new OpenXmlHelper(_configuration);
                        openXmlHelper.CreateDoc(ms, responses, fullQuestions, options);
                    }
                    else if (groupBy == ExamExtractionGroupBy.Question)
                    {
                        CreateWordDocumentGroupByQuestion docByQuestion = new CreateWordDocumentGroupByQuestion(responses, options, fullQuestions, _configuration);
                        docByQuestion.CreatePackage(ms);
                    }
                    var contentDisposition = new System.Net.Mime.ContentDisposition
                    {
                        FileName = filename,
                        Inline = true
                    };
                    Response.Headers["Content-Disposition"] = contentDisposition.ToString();

                    return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
                }
            }

            return BadRequest("Unable to generate file.");
        }

        private void LogDebug(string actioncategory, string actionDescription)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var logItem = new Logging
            {
                Audit = new AuditLog
                {
                    UserId = 0,
                    OrgUnitId = 0,
                    IpAddress = ipAddress,
                    ToolId = "elearnapps",
                    ToolAccessRoleId = 0,
                    ActionCategory = actioncategory,
                    ActionDescription = actionDescription,
                    ActionTime = DateTime.UtcNow
                }
            };

            var debugmessage = JsonSerializer.Serialize(logItem);
            debugmessage = FilterWhitelist(debugmessage);

            // Log4Net
            log.Debug(debugmessage);
        }

        #endregion
        private async Task<GPTZeroModel> RequestGptZero(Dictionary<int, string> studentAnswers)
        {
            log.Info($"RequestGptZero POST answers: {JsonSerializer.Serialize(studentAnswers)}");
            GPTZeroModel results = null;
            var batches = studentAnswers
                .Select((entry, index) => new { entry, index })
                .GroupBy(x => x.index / _constants.GPTZeroMaxItemPerRequest)
                .Select(group => group.ToDictionary(x => x.entry.Key, x => x.entry.Value)).ToList();
            log.Info($"Split into {batches.Count} batches");
            foreach (var batch in batches)
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("x-api-key", _constants.GPTZeroApiKey);
                    using (var form = new MultipartFormDataContent())
                    {
                        foreach (var item in batch)
                        {
                            using (var fs = new MemoryStream(Encoding.UTF8.GetBytes(item.Value)))
                            using (var streamContent = new StreamContent(fs))
                            {
                                var fileContent = new ByteArrayContent(await streamContent.ReadAsByteArrayAsync());
                                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("text/plain");
                                form.Add(fileContent, "files", $"UserId_{item.Key}-{Guid.NewGuid()}.txt");
                            }
                        }
                        try
                        {
                            HttpResponseMessage response = await httpClient.PostAsync(_constants.GPTZeroUrl, form);
                            log.Info($"Call API GptZero StatusCode: {response.StatusCode}");
                            response.EnsureSuccessStatusCode();
                            string finalResult = await response.Content.ReadAsStringAsync();

                            if (results == null)
                                results = JsonSerializer.Deserialize<GPTZeroModel>(finalResult);
                            else
                            {
                                var tempResult = JsonSerializer.Deserialize<GPTZeroModel>(finalResult);
                                results.Documents.AddRange(tempResult.Documents);
                            }
                        }
                        catch (Exception e)
                        {
                            log.Info($"GptZero Error: {e.Message}");
                            return null;
                        }
                    }
                }
            }

            return results;
        }
        /// <summary>
        /// track gpt zero usage
        /// </summary>
        /// <param name="orgUnitId"></param>
        /// <param name="userId"></param>
        [NonAction]
        private void GptZeroInsertAuditLog(int orgUnitId, int userId)
        {
            Task.Run(async () =>
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();

                await auditService.InsertAsync(new Audit
                {
                    ToolId = Category.Ee.ToString().ToUpper(),
                    DateTime = DateTime.UtcNow,
                    OrgUnitId = orgUnitId,
                    UserId = userId.ToString(),
                    ResourceId = Convert.ToInt32(AuditResourceId.ExportEEWithAIContentDetector),
                    Type = "Click"
                });
            });
        }
        [NonAction]
        private void UpdateUserResponseWithGPTResult(ref List<UserResponse> listUserResponse, ref GPTZeroModel result, ref string gptZeroTitle, ref CreateExtractionViewModel model, ref List<int> studentsTitle)
        {
            if (result != null && result.Documents != null && result.Documents.Any())
            {
                // to match with gpt response because gpt will automatically trim double space into single space
                listUserResponse.ForEach(res =>
                {
                    res.Responses.ForEach((r) =>
                    {
                        r.TextResponse = r.TextResponse.Replace("  ", " ");
                    });
                });
                var allSentences = new List<Sentence>();
                // to differentiate sentence  between documents
                result.Documents.ForEach(e =>
                {
                    var guid = Guid.NewGuid();
                    foreach (var s in e.Sentences)
                    {
                        s.SetNoSpecialSentenceContent(OpenXmlExtensions.RemoveSpecialCharacter(s.SentenceContent));
                        s.SetGuid(guid);
                        var convertedSentence = s.SentenceContent.Trim();
                        convertedSentence = OpenXmlExtensions.HtmlEncode(convertedSentence);
                        s.SentenceContent = convertedSentence;
                    }
                    allSentences.AddRange(e.Sentences);
                });

                var currentIndexOfSentence = 0;
                foreach (var userResponse in listUserResponse)
                {
                    if (userResponse.Responses.Any(x => x.QuestionNumber == 0)) continue;
                    var comparableResponses = userResponse.Responses.Where(response => string.Equals(response.QuestionType, "Long answer", StringComparison.OrdinalIgnoreCase)).ToList();
                    Guid? userDocumentGuid = null;
                    foreach (var response in comparableResponses)
                    {
                        var indexOfResponse = 0;
                        var noSpecialIndexOfResponse = 0;
                        var compareResponse = response.TextResponse;
                        if (!response.GetIsConverted())
                        {
                            compareResponse = OpenXmlExtensions.HtmlEncode(compareResponse);
                        }

                        var selectedSentences = new List<Sentence>();
                        while (currentIndexOfSentence < allSentences.Count)
                        {
                            var s = allSentences[currentIndexOfSentence];
                            var noSpecialSentence = s.GetNoSpecialSentenceContent();
                            var noSpecialCompareResponse = OpenXmlExtensions.RemoveSpecialCharacter(compareResponse);
                            var listIndex = new List<int>();
                            foreach (Match m in Regex.Matches(compareResponse, Regex.Escape(s.SentenceContent), RegexOptions.IgnoreCase))
                            {
                                listIndex.Add(m.Index);
                            }
                            var index = listIndex.Any(e => e >= indexOfResponse)
                                ? listIndex.First(e => e >= indexOfResponse)
                                : -1;

                            var noSpecialListIndex = new List<int>();
                            foreach (Match m in Regex.Matches(noSpecialCompareResponse, Regex.Escape(noSpecialSentence), RegexOptions.IgnoreCase))
                            {
                                noSpecialListIndex.Add(m.Index);
                            }
                            var noSpecialIndex = noSpecialListIndex.Any(e => e >= noSpecialIndexOfResponse)
                                ? noSpecialListIndex.First(e => e >= noSpecialIndexOfResponse)
                                : -1;

                            if ((index > -1 || noSpecialIndex > -1) && (userDocumentGuid == null || userDocumentGuid == s.GetGuid())) //must be same user gpt document
                            {
                                if (s.GeneratedProb > 0 && s.HighlightSentenceForAi)
                                {
                                    selectedSentences.Add(s);
                                }
                                userDocumentGuid = s.GetGuid();
                                indexOfResponse = index;
                                noSpecialIndexOfResponse = noSpecialIndex;
                                currentIndexOfSentence++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        foreach (var selectedSentence in selectedSentences)
                        {
                            var convertedSentence = selectedSentence.SentenceContent;
                            if (string.IsNullOrEmpty(gptZeroTitle))
                                gptZeroTitle = "Sentences highlighted below are more likely to be generated by AI or taken from published material. " +
                                    "The nature of AI-generated content is changing constantly. While we build detection features for non original work, " +
                                    "we recommend that instructors take these results as one of many pieces in a holistic assessment of student work.";

                            // to encode whote text response before replacing
                            if (!response.GetIsConverted())
                            {
                                response.TextResponse = OpenXmlExtensions.HtmlEncode(response.TextResponse);
                                response.SetIsConverted(true);
                            }
                            var input = response.TextResponse.Replace(System.Environment.NewLine, "<br/>");
                            int index = input.IndexOf(convertedSentence, StringComparison.OrdinalIgnoreCase);
                            var replacement = $"<span style='background-color:yellow;color: black'>{convertedSentence}</span>";
                            var indexOfReplacedPoint = input.LastIndexOf(replacement);
                            if (indexOfReplacedPoint > -1)
                            {
                                var listIndex = new List<int>();
                                foreach (Match m in Regex.Matches(input, Regex.Escape(convertedSentence), RegexOptions.IgnoreCase))
                                {
                                    listIndex.Add(m.Index);
                                }
                                var indexWhenThereReplacement = listIndex.Any(e => e >= indexOfReplacedPoint + replacement.Length) ? listIndex.First(e => e >= indexOfReplacedPoint + replacement.Length) : -1;
                                index = indexWhenThereReplacement;
                            }

                            if (index != -1)
                            {
                                input = input.Substring(0, index) + replacement + input.Substring(index + convertedSentence.Length);
                            }
                            response.TextResponse = input;
                            if (model.ExportOption == (int)ExportOption.OneDocumentPerStudent)
                                studentsTitle.Add(userResponse.StudentId);

                        }

                    }
                    // To skip to next student
                    var previousCurrentIndexOfSentence = currentIndexOfSentence - 1;
                    while (previousCurrentIndexOfSentence > -1 && currentIndexOfSentence < allSentences.Count)
                    {
                        var s = allSentences[currentIndexOfSentence];
                        var previousS = allSentences[previousCurrentIndexOfSentence];
                        if (s.GetGuid() == previousS.GetGuid())
                        {
                            currentIndexOfSentence++;
                            previousCurrentIndexOfSentence = currentIndexOfSentence - 1;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

        }
    }

}