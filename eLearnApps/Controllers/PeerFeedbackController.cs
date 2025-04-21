using eLearnApps.Business.Interface;
using eLearnApps.Core;
using eLearnApps.Core.Caching;
using eLearnApps.Core.Extension;
using eLearnApps.CustomAttribute;
using eLearnApps.Entity.LmsTools;
using eLearnApps.Entity.LmsTools.Dto;
using eLearnApps.Entity.Logging;
using eLearnApps.Extension;
using eLearnApps.Helpers;
using eLearnApps.Models;
using eLearnApps.Models.PeerFeedback;
using eLearnApps.ViewModel.KendoUI;
using eLearnApps.ViewModel.PeerFeedback;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using TimeZoneConverter;

namespace eLearnApps.Controllers
{
    public class PeerFeedbackController : BaseController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region CTOR

        public PeerFeedbackController(
            ICourseService courseService,
            IUserService userService,
            ICacheManager cacheManager,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration,
            IServiceProvider serviewProvider,
            ICompositeViewEngine compositeViewEngine,
            IWebHostEnvironment webHostEnvironment,
            IErrorLogService errorLogService,
            IPeerFeedbackService peerFeedbackService,
            ISemesterService semesterService,
            IUserEnrollmentService userEnrollmentService,
            ICategoryGroupService categoryGroupService,
            ICourseCategoryService courseCategoryService,
            ILoggingService loggingService,
            IUserGroupService userGroupService, IValenceService valenceService,
            IAuditService auditService) : base(cacheManager, errorLogService, httpContextAccessor, configuration, serviewProvider, compositeViewEngine)
        {
            _courseService = courseService;
            _userService = userService;
            _semesterService = semesterService;
            _peerFeedbackService = peerFeedbackService;
            _categoryGroupService = categoryGroupService;
            _userEnrollmentService = userEnrollmentService;
            _userGroupService = userGroupService;
            _courseCategoryService = courseCategoryService;
            _loggingService = loggingService;
            _auditService = auditService;
            _valenceService = valenceService;
        }

        #endregion

        // GET: PeerFeedback
        public IActionResult Index()
        {
            return View();
        }
        [PeerFeedBackAuthorize(Role = "student")]
        public ActionResult PeerFeedback()
        {
            log.Info("**************** START PeerFeedback ****************");
            // after reading this var, then set it to false
            bool isInitialLanding = (bool)Session["IsInitialLanding"];
            log.Info($"IsIsInitialLanding: {isInitialLanding.ToJson()}");
            Session["IsInitialLanding"] = false;

            log.Info("------ start PeerFeedBackEvaluationList ------");
            log.Info($"param: UserId = {UserInfo.UserId}");
            var peerfeedbacks = _peerFeedbackService.PeerFeedBackEvaluationList(UserInfo.UserId);
            log.Info("------ end PeerFeedBackEvaluationList ------");
            log.Info("Check closed Session");
            var HasClosedSession = peerfeedbacks.Any(p => p.To < DateTime.UtcNow);
            log.Info($"HasClosedSession: {HasClosedSession.ToJson()}");

            var vm = new Models.PeerFeedback.StudentPeerFeedbackViewModel()
            {
                UserInfo = UserInfo,
                HasClosedSession = HasClosedSession,
                IsInitialLanding = isInitialLanding
            };
            log.Info("**************** END PeerFeedback ****************");
            return PartialView(vm);
        }
        [PeerFeedBackAuthorize(Role = "student")]
        public ActionResult PeerFeedbackList()
        {
            log.Info("**************** START PeerFeedbackList ****************");
            log.Info("------ start PeerFeedBackEvaluationList ------");
            var result = _peerFeedbackService.PeerFeedBackEvaluationList(UserInfo.UserId);
            log.Info("------ end PeerFeedBackEvaluationList ------");
            var model = new PeerFeedbackListModel();
            var feedBacks = new List<PeerFeedbackViewModel>();
            log.Info("Loop through PeerFeedBackEvaluationList");
            foreach (var item in result)
            {
                double progress = 0; int groupCountComplete = 0;
                log.Info("------ start PeerFeedbackTargetsGetByPairingGroup ------");
                var targets = _peerFeedbackService.PeerFeedbackTargetsGetByPairingGroup(item.PeerFeedBackPairingId, item.PeerFeedBackGroupId);
                log.Info("------ end PeerFeedbackTargetsGetByPairingGroup ------");
                log.Info("------ start PeerFeedBackResponsesGetListByUser ------");
                var allResponses = _peerFeedbackService.PeerFeedBackResponsesGetListByUser(item.PeerFeedBackId, item.PeerFeedBackSessionId, item.PeerFeedBackGroupId, UserInfo.UserId);
                log.Info("------ end PeerFeedBackResponsesGetListByUser ------");
                log.Info("------ start PeerFeedBackResponsesGetList ------");
                var responsesGroup = _peerFeedbackService.PeerFeedBackResponsesGetList(item.PeerFeedBackId, item.PeerFeedBackSessionId, item.PeerFeedBackGroupId).ToList();
                log.Info("------ end PeerFeedBackResponsesGetList ------");
                Dictionary<int, int> dicUserQuestion = new Dictionary<int, int>();

                log.Info("------ start PeerFeedbackQuestionMapList ------");
                var peerFeedBackQuestions = _peerFeedbackService.PeerFeedbackQuestionMapList(item.PeerFeedBackId);
                log.Info("------ end PeerFeedbackQuestionMapList ------");
                double userCompleteCount = 0;
                int userGroupCompleteCount = 0;
                int userCount = targets.Count;
                log.Info("Loop through PeerFeedbackTargets");
                foreach (var user in targets)
                {
                    double questionCompleteCount = 0;
                    questionCompleteCount = allResponses
                            .Where(x => x.TargetUserId == user.UserId)
                            .Select(x => x.PeerFeedbackQuestionId)
                            .Distinct()
                            .Count();
                    var usersWithQuestion = responsesGroup
                       .Where(x => x.EvaluatorUserId == user.UserId)
                       .Select(x => new PeerFeedBackResponses
                       {
                           TargetUserId = x.TargetUserId,
                           PeerFeedbackQuestionId = x.PeerFeedbackQuestionId
                       }).Distinct().ToList();
                    var userGroupCount = usersWithQuestion.Select(x => x.TargetUserId).Distinct().Count();
                    var questionGroupCount = usersWithQuestion.GroupBy(p => p.TargetUserId, p => p.PeerFeedbackQuestionId, (targetUserId, questions) => new { userId = targetUserId, QuestionCount = questions.Distinct().Count() }).Select(x => x.QuestionCount).Sum();

                    if (questionCompleteCount == peerFeedBackQuestions.Count)
                        userCompleteCount++;
                    if (userGroupCount == userCount && questionGroupCount == peerFeedBackQuestions.Count * userCount)
                        userGroupCompleteCount++;
                }
                progress = (userCompleteCount / (targets.Count)) * 100;
                groupCountComplete = userGroupCompleteCount;
                var peerFeedbackViewModel = new PeerFeedbackViewModel
                {
                    Name = item.CourseName,
                    Label = item.Name,
                    EntryStartTime = item.From,
                    EntryCloseTime = item.To,
                    Progress = progress,
                    GroupCountComplete = groupCountComplete,
                    TotalUserCountInGroup = targets.Count,
                    PeerFeedBackGroupId = item.PeerFeedBackGroupId,
                    PeerFeedBackPairingId = item.PeerFeedBackPairingId,
                    PeerFeedBackSessionId = item.PeerFeedBackSessionId,
                    PeerFeedBackId = item.PeerFeedBackId,
                    SessionName = item.SessionName,
                    PeerFeedBackKey = item.PeerFeedBackId.ToEncrypt()
                };
                feedBacks.Add(peerFeedbackViewModel);
            }
            log.Info("data fulfilled list entity responses");
            model.PeerFeedbacks = feedBacks;
            log.Info("**************** END PeerFeedbackList ****************");
            return PartialView(model);
        }
        [PeerFeedBackAuthorize(Role = "student")]
        public ActionResult PeerFeedbackDetail(int peerFeedBackId, int peerFeedBackSessionId, int peerFeedBackGroupId, int peerFeedBackPairingId, double progress)
        {
            log.Info("**************** START PeerFeedbackDetail ****************");
            log.Info($"------ start get categoryGroupService by Id: {peerFeedBackGroupId} ------");
            var category = _categoryGroupService.GetById(peerFeedBackGroupId);
            log.Info($"------ end get categoryGroupService by Id: {peerFeedBackGroupId} ------");
            log.Info($"------ start PeerFeedBackEvaluationDetail------");
            log.Info($"param: SessionId = {peerFeedBackSessionId}, GroupId = {peerFeedBackGroupId}, PairingId = {peerFeedBackPairingId}, UserId = {UserInfo.UserId}");
            var result = _peerFeedbackService.PeerFeedBackEvaluationDetail(peerFeedBackSessionId, peerFeedBackGroupId, peerFeedBackPairingId, UserInfo.UserId);
            log.Info($"------ end PeerFeedBackEvaluationDetail ------");
            var model = new PeerFeedbackDetailList();
            var feedBacks = new List<PeerFeedbackDetail>();
            log.Info("start fill data to list entity responses");
            foreach (var student in result)
            {
                if (UserInfo.UserId == student.Id) continue;
                var item = new PeerFeedbackDetail
                {
                    DisplayName = student.DisplayName,
                    GroupName = category.Name
                };
                feedBacks.Add(item);
            }
            log.Info("data fulfilled list entity responses");
            model.PeerFeedbackDetails = feedBacks;
            model.PeerFeedBackGroupId = peerFeedBackGroupId;
            model.PeerFeedBackPairingId = peerFeedBackPairingId;
            model.PeerFeedBackSessionId = peerFeedBackSessionId;
            model.PeerFeedBackId = peerFeedBackId;
            model.Progress = progress;
            log.Info("**************** END PeerFeedbackDetail ****************");
            return PartialView(model);
        }
        [PeerFeedBackAuthorize(Role = "student")]
        public ActionResult PeerFeedbackResult(int? defaultActiveId = null, int? defaultGroupId = null, int? defaultSessionId = null)
        {
            List<PeerFeedbackResultModel> model = GetPeerFeedbackResultModelList(defaultActiveId, defaultGroupId, defaultSessionId);

            if (defaultActiveId == null && defaultGroupId == null && defaultSessionId == null)
            {
                if (model.Where(x => x.Session.PeerFeedbackResultSessionStatus == PeerFeedbackResultSessionStatus.Ongoing).Count() > 0)
                {
                    var detailModel = model.Where(x => x.Session.PeerFeedbackResultSessionStatus == PeerFeedbackResultSessionStatus.Ongoing).OrderByDescending(x => x.Session.EntryStartTime).FirstOrDefault();
                    detailModel.DefaultActive = true;
                }
                else if (model.Where(x => x.Session.PeerFeedbackResultSessionStatus == PeerFeedbackResultSessionStatus.Closed).Count() > 0)
                {
                    var detailModel = model.Where(x => x.Session.PeerFeedbackResultSessionStatus == PeerFeedbackResultSessionStatus.Closed).OrderByDescending(x => x.Session.EntryStartTime).FirstOrDefault();
                    detailModel.DefaultActive = true;
                }
                else if (model.Where(x => x.Session.PeerFeedbackResultSessionStatus == PeerFeedbackResultSessionStatus.Incomplete).Count() > 0)
                {
                    var detailModel = model.Where(x => x.Session.PeerFeedbackResultSessionStatus == PeerFeedbackResultSessionStatus.Incomplete).OrderByDescending(x => x.Session.EntryStartTime).FirstOrDefault();
                    detailModel.DefaultActive = true;
                }
            }

            log.Info("data fulfilled list entity responses");
            log.Info("**************** END PeerFeedbackResult ****************");
            return PartialView(model);
        }
        [PeerFeedBackAuthorize(Role = "student")]
        public ActionResult PeerFeedbackResultDetail(string key, int peerFeedBackId, int peerFeedBackSessionId, int peerFeedBackGroupId, int peerFeedBackPairingId)
        {
            log.Info("**************** START PeerFeedbackResultDetail ****************");
            double progress = 0;
            double userCompleteCount = 0;
            log.Info("Check paramKey");
            var paramKey = $"{peerFeedBackId}{peerFeedBackSessionId}{peerFeedBackGroupId}".ToEncrypt();
            if (!string.Equals(key, paramKey, StringComparison.Ordinal))
            {
                log.Warn($"Param key is not correct - issue with param.PeerFeedBackId = {peerFeedBackId}, param.PeerFeedBackSessionId = {peerFeedBackSessionId}");
                return RedirectToAction("Index", "Error");
            }
            log.Info("------ start PeerFeedbackGetById ------");
            log.Info($"param: peerFeedBackId = {peerFeedBackId}");
            var peerFeedBack = _peerFeedbackService.PeerFeedbackGetById(peerFeedBackId);
            log.Info("------ end PeerFeedbackGetById ------");
            log.Info("------ start PeerFeedbackSessionsGetById ------");
            log.Info($"param: peerFeedBackSessionId = {peerFeedBackSessionId}");
            var session = _peerFeedbackService.PeerFeedbackSessionsGetById(peerFeedBackSessionId);
            log.Info("------ end PeerFeedbackSessionsGetById ------");
            log.Info("Check peerFeedBack == null || session == null");
            if (peerFeedBack == null || session == null)
            {
                log.Warn($"peerFeedBack == null || session == null - issue with param.PeerFeedBackId = {peerFeedBackId}, param.PeerFeedBackSessionId = {peerFeedBackSessionId}");
                return RedirectToAction("Index", "Error");
            }
            log.Info("------ start PeerFeedbackQuestionMapList ------");
            log.Info($"param: peerFeedBackId = {peerFeedBackId}");
            var peerFeedBackQuestions = _peerFeedbackService.PeerFeedbackQuestionMapList(peerFeedBackId);
            log.Info("------ start PeerFeedbackQuestionMapList ------");
            log.Info("------ start GetByCategoryGroupId ------");
            log.Info($"param: peerFeedBackGroupId = {peerFeedBackGroupId}");
            var usersInGroup = _userService.GetByCategoryGroupId(peerFeedBackGroupId);
            log.Info("------ start GetByCategoryGroupId ------");
            log.Info("------ start PeerFeedBackResponsesGetList ------");
            log.Info($"param: peerFeedBackId = {peerFeedBackId}, SessionId = {peerFeedBackSessionId}, GroupId = {peerFeedBackGroupId}");
            var allResponses = _peerFeedbackService.PeerFeedBackResponsesGetList(peerFeedBackId, peerFeedBackSessionId, peerFeedBackGroupId);
            log.Info("------ start PeerFeedBackResponsesGetList ------");
            log.Info("Start fill data into response model");
            log.Info("Loop through PeerFeedBackResponsesGetList");

            List<dynamic> dynamicList = new List<dynamic>();

            if (allResponses.Any())
            {
                var ratingQuestions = _peerFeedbackService.GetListPeerFeedbackRatingQuestions();
                var ratingColorProvider = new RatingRGBProvider(ratingQuestions);

                foreach (var user in usersInGroup)
                {
                    double questionCompleteCount = 0;
                    foreach (var peerFeedBackQuestion in peerFeedBackQuestions)
                    {
                        HybridDictionary dicRatingComplete = new HybridDictionary();
                        var responses = allResponses.Where(x => x.TargetUserId == user.Id && x.PeerFeedbackQuestionId == peerFeedBackQuestion.Id && x.EvaluatorUserId == UserInfo.UserId).ToList();
                        var questionRatingMap = _peerFeedbackService.GetFeedbackQuestionRatingMapByQuestionId(peerFeedBackQuestion.Id);
                        foreach (var it in questionRatingMap)
                        {
                            if (responses.Any(r => r.PeerFeedBackRatingId == it.RatingQuestionId) && !dicRatingComplete.Contains(it.RatingQuestionId))
                                dicRatingComplete.Add(it.RatingQuestionId, true);
                        }

                        if (dicRatingComplete.Count > 0)
                            questionCompleteCount++;

                        if (user.Id == UserInfo.UserId)
                        {
                            dynamic obj = new ExpandoObject();
                            obj.peerFeedbackQuestionId = peerFeedBackQuestion.Id;

                            var responsePerQuestion = allResponses.Where(x => x.PeerFeedbackQuestionId == peerFeedBackQuestion.Id).ToList();
                            responsePerQuestion = GetCompleteEvaluation(responsePerQuestion, peerFeedBackId, peerFeedBackQuestion.Id, peerFeedBackSessionId, peerFeedBackGroupId);
                            responsePerQuestion = responsePerQuestion
                                .Where(x => x.TargetUserId == UserInfo.UserId && x.EvaluatorUserId != UserInfo.UserId)
                                .GroupBy(y => new { y.EvaluatorUserId, y.PeerFeedBackRatingId })
                                .Select(z => new PeerFeedBackResponses
                                {
                                    PeerFeedBackRatingId = z.Key.PeerFeedBackRatingId
                                }).ToList();
                            TextColorValue textColorValue = GetMeanScore(responsePerQuestion);
                            obj.Text = textColorValue.Text;
                            obj.ColorCode = textColorValue.ColorCode;

                            dynamicList.Add(obj);
                        }
                    }

                    if (questionCompleteCount.Equals(peerFeedBackQuestions.Count))
                        userCompleteCount++;
                }
                progress = (userCompleteCount / (usersInGroup.Count)) * 100;
            }
            log.Info("data fulfilled list entity responses");

            var model = new PeerFeedBackResultDetailViewModel
            {
                PeerFeedBackId = peerFeedBackId,
                PeerFeedBackGroupId = peerFeedBackGroupId,
                PeerFeedBackSessionId = peerFeedBackSessionId,
                PeerFeedBackPairingId = peerFeedBackPairingId,
                Questions = PeerFeedbackQuestionTemplate(peerFeedBackId),
                Session = new PeerFeedbackSessionViewModel
                {
                    Id = session.Id,
                    EndTotalMilliseconds = session.EntryCloseTime.TotalMilliseconds(),
                    StartTotalMilliseconds = session.EntryStartTime.TotalMilliseconds(),
                    Label = session.Label
                },
                Key = key,
                Progress = progress
            };

            model.Questions.ForEach(x =>
            {
                dynamic obj = dynamicList.Where(y => y.peerFeedbackQuestionId == x.Id).FirstOrDefault();
                if (obj != null)
                {
                    x.MeanScore = new TextColorValue
                    {
                        Text = obj.Text,
                        ColorCode = obj.ColorCode
                    };
                }
            });

            log.Info($"insert audit log");
            var audit = new AuditEntry
            {
                OrgUnitId = _peerFeedbackService.GetCourseId(peerFeedBackGroupId),
                AuditType = AuditType.Click,
                UserId = UserInfo.UserId.ToString(),
                ToolId = Constants.ToolIdPeerFeedback,
                ResourceId = Convert.ToInt32(AuditResourceId.ViewedEvaluation)
            };
            _loggingService.AuditUserAction(audit);

            log.Info("**************** END PeerFeedbackResultDetail ****************");
            return PartialView("_PeerFeedbackResultDetail", model);
        }

        public List<PeerFeedbackResultModel> GetPeerFeedbackResultModelList(int? defaultActiveId = null, int? defaultGroupId = null, int? defaultSessionId = null)
        {
            log.Info("**************** START PeerFeedbackResult ****************");
            log.Info("------ start PeerFeedBackEvaluationList ------");
            log.Info($"param: UserId = {UserInfo.UserId}");
            var result = _peerFeedbackService.PeerFeedBackEvaluationList(UserInfo.UserId);
            log.Info("------ end PeerFeedBackEvaluationList ------");
            var peerFeedbackResultModelList = new List<PeerFeedbackResultModel>();
            log.Info("Check default values");

            log.Info("Start fill data into response model");
            log.Info("Loop through PeerFeedBackEvaluationList");
            foreach (var item in result)
            {
                double progress = 0;
                var usersInGroup = _userService.GetByCategoryGroupId(item.PeerFeedBackGroupId);
                var allResponses = _peerFeedbackService.PeerFeedBackResponsesGetList(item.PeerFeedBackId, item.PeerFeedBackSessionId, item.PeerFeedBackGroupId);
                allResponses = allResponses.Where(x => x.EvaluatorUserId == UserInfo.UserId).ToList();
                if (allResponses.Any())
                {
                    var peerFeedbackQuestions = _peerFeedbackService.PeerFeedbackQuestionMapList(item.PeerFeedBackId);
                    double userCompleteCount = 0;
                    foreach (var user in usersInGroup)
                    {
                        double questionCompleteCount = 0;
                        foreach (var peerFeedBackQuestion in peerFeedbackQuestions)
                        {
                            HybridDictionary dicRatingComplete = new HybridDictionary();
                            var responses = allResponses.Where(x => x.TargetUserId == user.Id && x.PeerFeedbackQuestionId == peerFeedBackQuestion.Id).ToList();
                            var questionRatingMap = _peerFeedbackService.GetFeedbackQuestionRatingMapByQuestionId(peerFeedBackQuestion.Id);
                            foreach (var it in questionRatingMap)
                            {
                                if (responses.Any(r => r.PeerFeedBackRatingId == it.RatingQuestionId) && !dicRatingComplete.Contains(it.RatingQuestionId))
                                    dicRatingComplete.Add(it.RatingQuestionId, true);
                            }
                            if (dicRatingComplete.Count > 0)
                                questionCompleteCount++;
                        }
                        if (questionCompleteCount.Equals(peerFeedbackQuestions.Count))
                            userCompleteCount++;
                    }
                    progress = (userCompleteCount / (usersInGroup.Count)) * 100;
                }
                var peerFeedBackSession = _peerFeedbackService.PeerFeedbackSessionsGetById(item.PeerFeedBackSessionId);
                bool isDoneProgress = progress.Equals(100);

                bool isDoneDateTime = peerFeedBackSession.EntryCloseTime <= DateTime.UtcNow;
                if (peerFeedBackSession.EntryStartTime > DateTime.UtcNow) continue;
                string warningMessage = string.Empty;
                if (!isDoneDateTime)
                {
                    log.Info("Evaluation not finish");
                    warningMessage = "The peer and self feedback exercise is still on-going. You will be able to view your results after the exercise is closed, if you have submitted your feedback for all your group members. Do note that you will not be able to access the feedback of your peers if you do not complete this exercise.";
                }
                else if (!isDoneProgress)
                {
                    log.Info("not done evaluation");
                    //warningMessage = "As you did not complete the Peer & Self Feedback for all your group members, you are unable to access the feedback of your peers. We hope that you will participate in the Peer & Self Feedback in the coming term.";
                }

                var terms = _peerFeedbackService.PeerFeedbackSessionsGetTerm();
                var session = _peerFeedbackService.PeerFeedbackSessionsGetById(item.PeerFeedBackSessionId);

                //if (isDoneDateTime && isDoneProgress)
                //{
                var detailModel = new PeerFeedbackResultModel
                {
                    Name = item.Name,
                    CourseId = item.CourseId,
                    CourseName = item.CourseName,
                    WarningMessage = warningMessage,
                    PeerFeedBackId = item.PeerFeedBackId,
                    PeerFeedBackGroupId = item.PeerFeedBackGroupId,
                    PeerFeedBackPairingId = item.PeerFeedBackPairingId,
                    PeerFeedBackSessionId = item.PeerFeedBackSessionId,
                    Key = $"{item.PeerFeedBackId}{item.PeerFeedBackSessionId}{item.PeerFeedBackGroupId}".ToEncrypt(),
                    DefaultActive = defaultActiveId == item.PeerFeedBackId && defaultGroupId == item.PeerFeedBackGroupId && defaultSessionId == item.PeerFeedBackSessionId,
                    Session = new PeerFeedbackSessionViewModel
                    {
                        Id = peerFeedBackSession.Id,
                        EndTotalMilliseconds = peerFeedBackSession.EntryCloseTime.TotalMilliseconds(),
                        StartTotalMilliseconds = peerFeedBackSession.EntryStartTime.TotalMilliseconds(),
                        Label = peerFeedBackSession.Label,
                        EntryStartTime = peerFeedBackSession.EntryStartTime,
                        EntryCloseTime = peerFeedBackSession.EntryCloseTime,
                        PeerFeedbackResultSessionStatus = !isDoneDateTime ? PeerFeedbackResultSessionStatus.Ongoing : (!isDoneProgress ? PeerFeedbackResultSessionStatus.Incomplete : PeerFeedbackResultSessionStatus.Closed),
                        PeerFeedbackId = item.PeerFeedBackId,
                        PeerFeedBackKey = item.PeerFeedBackId.ToEncrypt(),
                        Progress = progress,
                        Term = terms.First(a => a.Value == Convert.ToInt32(session.Strm)).Text
                    }
                };
                peerFeedbackResultModelList.Add(detailModel);
                //}
            }

            return peerFeedbackResultModelList;
        }

        // Identify the median rating based on the responses
        private TextColorValue GetMedianRating(List<PeerFeedBackResponses> responses)
        {
            var ratingQuestions = _peerFeedbackService.GetListPeerFeedbackRatingQuestions();
            var ratingColorProvider = new RatingRGBProvider(ratingQuestions);
            TextColorValue textColorValue = new TextColorValue();

            // Sort responses by rating - Exceed, Meet, Did not meet - in this order
            responses = responses.OrderBy(x => x.PeerFeedBackRatingId).ToList();

            // If the number of responses is odd, get the median rating and and color code accordingly
            // If the number of responses is even, get the two median ratings and identify the median according to the rules below.
            // Rules:
            //      Exceed + Meet = Meet
            //      Exceed + Below = Meet
            //      Meet + Below = Meet
            if (responses.Count > 0)
            {
                if (responses.Count % 2 != 0)
                {
                    int ratingId = responses[(responses.Count / 2)].PeerFeedBackRatingId.Value;
                    textColorValue.Text = ratingQuestions.Where(x => x.Id.Equals(ratingId)).First().Name;
                    textColorValue.ColorCode = ratingColorProvider.GetRatingColorCodes(ratingId);
                }
                else
                {
                    int median1 = responses[(responses.Count / 2) - 1].PeerFeedBackRatingId.Value;
                    PeerFeedbackRatingQuestion ratingQuestion1 = ratingQuestions.Where(x => x.Id.Equals(median1)).First();

                    int median2 = responses[(responses.Count / 2)].PeerFeedBackRatingId.Value;
                    PeerFeedbackRatingQuestion ratingQuestion2 = ratingQuestions.Where(x => x.Id.Equals(median2)).First();

                    if (median1 == median2)
                    {
                        textColorValue.Text = ratingQuestion1.Name;
                        textColorValue.ColorCode = ratingColorProvider.GetRatingColorCodes(median1);
                    }
                    else if ((string.Equals(ratingQuestion1.Name, Constants.ExceedsExpectations, StringComparison.OrdinalIgnoreCase) && string.Equals(ratingQuestion2.Name, Constants.MeetsExpectations, StringComparison.OrdinalIgnoreCase)) || (string.Equals(ratingQuestion2.Name, Constants.ExceedsExpectations, StringComparison.OrdinalIgnoreCase) && string.Equals(ratingQuestion1.Name, Constants.MeetsExpectations, StringComparison.OrdinalIgnoreCase)) ||

                        (string.Equals(ratingQuestion1.Name, Constants.ExceedsExpectations, StringComparison.OrdinalIgnoreCase) && string.Equals(ratingQuestion2.Name, Constants.YetToMeetExpectations, StringComparison.OrdinalIgnoreCase)) || (string.Equals(ratingQuestion2.Name, Constants.ExceedsExpectations, StringComparison.OrdinalIgnoreCase) && string.Equals(ratingQuestion1.Name, Constants.YetToMeetExpectations, StringComparison.OrdinalIgnoreCase)) ||

                        (string.Equals(ratingQuestion1.Name, Constants.MeetsExpectations, StringComparison.OrdinalIgnoreCase) && string.Equals(ratingQuestion2.Name, Constants.YetToMeetExpectations, StringComparison.OrdinalIgnoreCase)) || (string.Equals(ratingQuestion2.Name, Constants.MeetsExpectations, StringComparison.OrdinalIgnoreCase) && string.Equals(ratingQuestion1.Name, Constants.YetToMeetExpectations, StringComparison.OrdinalIgnoreCase))

                        )
                    {
                        textColorValue.Text = Constants.MeetsExpectations;
                        textColorValue.ColorCode = ratingColorProvider.GetRatingColorCodes(ratingQuestions.Where(x => string.Equals(x.Name, Constants.MeetsExpectations, StringComparison.OrdinalIgnoreCase)).First().Id);
                    }
                }
            }

            return textColorValue;
        }

        // Identify the mean score based on the responses
        private TextColorValue GetMeanScore(List<PeerFeedBackResponses> responses)
        {
            var ratingQuestions = _peerFeedbackService.GetListPeerFeedbackRatingQuestions();
            var ratingColorProvider = new RatingRGBProvider(ratingQuestions);
            TextColorValue textColorValue = new TextColorValue();

            // Get the average of PeerFeedBackRatingId and identify the mean score according to the rules below.
            // Rules:
            //      Below 1.5 = Yet to Meet
            //      1.5 to 2.5 = Meet
            //      Above 2.5 = Exceed
            if (responses.Count > 0)
            {
                double mean = responses.Average(x => x.PeerFeedBackRatingId.Value);

                if (mean < 1.5)
                {
                    textColorValue.Text = Constants.YetToMeetExpectations;
                    textColorValue.ColorCode = ratingColorProvider.GetRatingColorCodes(ratingQuestions.Where(x => string.Equals(x.Name, Constants.YetToMeetExpectations, StringComparison.OrdinalIgnoreCase)).First().Id);
                }
                else if (mean >= 1.5 && mean <= 2.5)
                {
                    textColorValue.Text = Constants.MeetsExpectations;
                    textColorValue.ColorCode = ratingColorProvider.GetRatingColorCodes(ratingQuestions.Where(x => string.Equals(x.Name, Constants.MeetsExpectations, StringComparison.OrdinalIgnoreCase)).First().Id);
                }
                else if (mean > 2.5)
                {
                    textColorValue.Text = Constants.ExceedsExpectations;
                    textColorValue.ColorCode = ratingColorProvider.GetRatingColorCodes(ratingQuestions.Where(x => string.Equals(x.Name, Constants.ExceedsExpectations, StringComparison.OrdinalIgnoreCase)).First().Id);
                }
            }

            return textColorValue;
        }

        [PeerFeedBackAuthorize(Role = "student")]
        public ActionResult PeerFeedbackResultDetailAllCourses()
        {
            log.Info("**************** START PeerFeedbackResultDetailAllCourses ****************");

            log.Info("------ start GetListPeerFeedbackQuestions ------");
            var peerFeedBackQuestionList = _peerFeedbackService.GetListPeerFeedbackQuestions();

            List<dynamic> dynamicList = new List<dynamic>();
            peerFeedBackQuestionList.ForEach(y =>
            {
                var allResponses = GetCompleteEvaluation(null, 0, y.Id, 0, 0, true);
                allResponses = allResponses.Where(x => x.TargetUserId == UserInfo.UserId).ToList();

                if (allResponses.Any())
                {
                    var filters = allResponses.Select(x => new PeerFeedBackResponseGroupByModel
                    {
                        PeerFeedBackGroupId = x.PeerFeedBackGroupId,
                        PeerFeedbackId = x.PeerFeedbackId,
                        PeerFeedbackSessionId = x.PeerFeedbackSessionId
                    }).DistinctBy(x => new
                    {
                        x.PeerFeedbackId,
                        x.PeerFeedBackGroupId,
                        x.PeerFeedbackSessionId
                    }).ToList();

                    foreach (var filter in filters)
                    {
                        var session = _peerFeedbackService.PeerFeedbackSessionsGetById(filter.PeerFeedbackSessionId);
                        if (session.EntryCloseTime >= DateTime.UtcNow) continue;

                        var allResponsesByGroup = _peerFeedbackService.PeerFeedBackResponsesGetList(filter.PeerFeedbackId, filter.PeerFeedbackSessionId, filter.PeerFeedBackGroupId);
                        double progress = 0;
                        var responses = allResponsesByGroup.Where(x => x.EvaluatorUserId == UserInfo.UserId).ToList();
                        if (responses.Any())
                        {
                            double userCompleteCount = 0;
                            var peerFeedBackQuestions = _peerFeedbackService.PeerFeedbackQuestionMapList(filter.PeerFeedbackId);
                            var usersInGroup = _userService.GetByCategoryGroupId(filter.PeerFeedBackGroupId).ToList();
                            foreach (var user in usersInGroup)
                            {
                                double questionCompleteCount = 0;

                                foreach (var peerFeedBackQuestion in peerFeedBackQuestions)
                                {
                                    HybridDictionary dicRatingComplete = new HybridDictionary();
                                    var items = responses.Where(x => x.TargetUserId == user.Id && x.PeerFeedbackQuestionId == peerFeedBackQuestion.Id).ToList();
                                    if (items.Any())
                                    {
                                        var questionRatingMapByGroup = _peerFeedbackService.GetFeedbackQuestionRatingMapByQuestionId(peerFeedBackQuestion.Id);
                                        foreach (var it in questionRatingMapByGroup)
                                        {
                                            if (items.Any(r => r.PeerFeedBackRatingId == it.RatingQuestionId) && !dicRatingComplete.Contains(it.RatingQuestionId))
                                                dicRatingComplete.Add(it.RatingQuestionId, true);
                                        }
                                        if (dicRatingComplete.Count > 0)
                                            questionCompleteCount++;
                                    }
                                }
                                if (questionCompleteCount.Equals(peerFeedBackQuestions.Count))
                                    userCompleteCount++;
                            }
                            progress = (userCompleteCount / (usersInGroup.Count)) * 100;
                            if (progress < 100)
                            {
                                allResponses.RemoveAll(x => x.PeerFeedbackId == filter.PeerFeedbackId && x.PeerFeedbackSessionId == filter.PeerFeedbackSessionId && x.PeerFeedBackGroupId == filter.PeerFeedBackGroupId);
                            }
                        }
                    }
                }
                var targetResponses = allResponses.Where(x => x.EvaluatorUserId != UserInfo.UserId).ToList();

                var responsePerQuestion = GetCompleteEvaluation(targetResponses, 0, y.Id, 0, 0);
                responsePerQuestion = responsePerQuestion
                    .Where(x => x.TargetUserId == UserInfo.UserId && x.EvaluatorUserId != UserInfo.UserId)
                    .GroupBy(yy => new { yy.EvaluatorUserId, yy.PeerFeedBackRatingId, yy.PeerFeedBackGroupId, yy.PeerFeedbackId, yy.PeerFeedbackSessionId })
                    .Select(z => new PeerFeedBackResponses
                    {
                        PeerFeedBackRatingId = z.Key.PeerFeedBackRatingId
                    }).ToList();

                dynamic obj = new ExpandoObject();
                obj.peerFeedbackQuestionId = y.Id;
                TextColorValue textColorValue = GetMeanScore(responsePerQuestion);
                obj.Text = textColorValue.Text;
                obj.ColorCode = textColorValue.ColorCode;
                dynamicList.Add(obj);
            });

            log.Info("------ start GetListPeerFeedbackQuestions ------");
            var model = new PeerFeedBackResultDetailViewModel
            {
                GroupBy = (int)PeerFeedBackResultGroupBy.AllCourses,
                Questions = peerFeedBackQuestionList.Select(x => new PeerFeedbackQuestionModel
                {
                    Description = x.Description,
                    Id = x.Id,
                    Title = x.Title
                }).ToList(),
            };

            model.Questions.ForEach(x =>
            {
                dynamic obj = dynamicList.Where(y => y.peerFeedbackQuestionId == x.Id).FirstOrDefault();
                x.MeanScore = new TextColorValue
                {
                    Text = obj.Text,
                    ColorCode = obj.ColorCode
                };
            });

            List<PeerFeedbackResultModel> peerFeedbackResultModelList = GetPeerFeedbackResultModelList();
            model.PeerFeedbackResultModelList = peerFeedbackResultModelList.Where(x => x.Session.PeerFeedbackResultSessionStatus == PeerFeedbackResultSessionStatus.Closed).ToList();

            log.Info($"insert audit log");
            var audit = new AuditEntry
            {
                AuditType = AuditType.Click,
                UserId = UserInfo.UserId.ToString(),
                ToolId = Constants.ToolIdPeerFeedback,
                ResourceId = Convert.ToInt32(AuditResourceId.ViewedEvaluation),
            };
            _loggingService.AuditUserAction(audit);

            log.Info("**************** END PeerFeedbackResultDetailAllCourses ****************");
            return PartialView("_PeerFeedbackResultDetailAllCourses", model);
        }
        [PeerFeedBackAuthorize(Role = "student")]
        public ActionResult PeerFeedbackResponse(int peerFeedBackId, int peerFeedBackSessionId, int peerFeedBackGroupId, int peerFeedBackPairingId, string peerFeedBackKey, double progress = 0, bool closed = false)
        {
            log.Info("**************** START PeerFeedbackResultDetailAllCourses ****************");
            if (string.IsNullOrEmpty(peerFeedBackKey))
            {
                log.Warn($"Null PeerFeedbackKey - issue with param.PeerFeedBackId = {peerFeedBackId}, param.PeerFeedBackSessionId = {peerFeedBackSessionId}");
                return RedirectToAction("Index", "Error");
            }

            log.Info("------ start PeerFeedbackGetById ------");
            log.Info($"param: peerFeedBackId = {peerFeedBackId}");
            var peerFeedBack = _peerFeedbackService.PeerFeedbackGetById(peerFeedBackId);
            log.Info("------ start PeerFeedbackGetById ------");
            log.Info("------ start PeerFeedbackSessionsGetById ------");
            log.Info($"param: peerFeedBackSessionId = {peerFeedBackSessionId}");
            var peerFeedBackSession = _peerFeedbackService.PeerFeedbackSessionsGetById(peerFeedBackSessionId);
            log.Info("------ start PeerFeedbackSessionsGetById ------");
            log.Info("------ start PeerFeedbackQuestionTemplate ------");
            log.Info($"param: peerFeedBackId = {peerFeedBackId}");
            var questions = PeerFeedbackQuestionTemplate(peerFeedBackId);
            log.Info("------ start PeerFeedbackQuestionTemplate ------");
            if (peerFeedBack == null || peerFeedBackSession == null || questions == null || !string.Equals(peerFeedBackKey, peerFeedBack.Id.ToEncrypt(), StringComparison.Ordinal))
            {
                log.Warn($"peerFeedBack == null || peerFeedBackSession == null || questions == null || peerFeedBackKey not correct - issue with param.PeerFeedBackId = {peerFeedBackId}, param.PeerFeedBackSessionId = {peerFeedBackSessionId}");
                return RedirectToAction("Index", "Error");
            }
            log.Info("------ start GetTargetByPairingId ------");
            log.Info($"param: peerFeedBackPairingId = {peerFeedBackPairingId}");
            var students = _peerFeedbackService.GetTargetByPairingId(peerFeedBackPairingId);
            log.Info("------ start GetTargetByPairingId ------");
            log.Info("------ start PeerFeedBackResponsesGetListWithGroupAndUser ------");
            log.Info($"param: SessionId = {peerFeedBackSessionId}, GroupId = {peerFeedBackGroupId}, peerFeedBackId = {peerFeedBackId}, UserId = {UserInfo.UserId}");
            var peerFeedBackResponses =
                _peerFeedbackService.PeerFeedBackResponsesGetListWithGroupAndUser(peerFeedBackId, peerFeedBackSessionId, peerFeedBackGroupId, UserInfo.UserId);
            var peerFeedBackResponseRemarks =
                _peerFeedbackService.PeerFeedBackResponseRemarksGetListWithGroupAndUser(peerFeedBackId, peerFeedBackSessionId, peerFeedBackGroupId, UserInfo.UserId);
            log.Info("------ start PeerFeedBackResponsesGetListWithGroupAndUser ------");
            var courseId = _peerFeedbackService.GetCourseId(peerFeedBackGroupId);
            var users = new List<PeerFeedbackUserModel>();
            log.Info("Fill data to PeerFeedbackUserModel");
            foreach (var student in students)
            {
                var user = new PeerFeedbackUserModel
                {
                    UserId = student.Id,
                    DisplayName = student.DisplayName
                };
                users.Add(user);
            }
            log.Info("Fill data to PeerFeedbackResponseViewModel");
            var model = new PeerFeedbackResponseViewModel
            {
                PeerFeedBackPairingId = peerFeedBackPairingId,
                PeerFeedBackId = peerFeedBackId,
                PeerFeedBackSessionId = peerFeedBackSessionId,
                PeerFeedBackGroupId = peerFeedBackGroupId,
                CourseId = courseId,
                Progress = progress,
                Name = peerFeedBack.Name,
                StartTime = peerFeedBackSession.EntryStartTime,
                EndTime = peerFeedBackSession.EntryCloseTime,
                Users = users.OrderBy(x => x.UserId == UserInfo.UserId).ToList(),
                Questions = questions,
                Closed = closed,
                PeerFeedBackKey = peerFeedBack.Id.ToEncrypt(),
                PeerFeedBackResponses = peerFeedBackResponses.Select(x => new PeerFeedBackResponseViewModel
                {
                    EvaluatorUserId = x.EvaluatorUserId,
                    Id = x.Id,
                    PeerFeedBackOptionId = x.PeerFeedBackOptionId,
                    LastUpdateTime = x.LastUpdateTime,
                    PeerFeedBackRatingId = x.PeerFeedBackRatingId,
                    PeerFeedbackId = x.PeerFeedbackId,
                    PeerFeedbackQuestionId = x.PeerFeedbackQuestionId,
                    PeerFeedbackSessionId = x.PeerFeedbackSessionId,
                    TargetUserId = x.TargetUserId
                }).ToList(),
                PeerFeedBackResponseRemarks = peerFeedBackResponseRemarks.Select(x => new PeerFeedBackResponseRemarkViewModel
                {
                    Id = x.Id,
                    EvaluatorUserId = x.EvaluatorUserId,
                    TargetUserId = x.TargetUserId,
                    PeerFeedbackId = x.PeerFeedbackId,
                    PeerFeedbackSessionId = x.PeerFeedbackSessionId,
                    LastUpdateTime = x.LastUpdateTime,
                    PeerFeedBackGroupId = x.PeerFeedBackGroupId,
                    Remarks = x.Remarks,
                }).ToList()
            };
            log.Info("data fulfilled list responses");
            log.Info("**************** END PeerFeedbackResponse ****************");
            return PartialView(model);
        }
        [HttpPost]
        [PeerFeedBackAuthorize(Role = "student")]
        public ActionResult PeerFeedbackResponseSave([Bind(Include = "PeerFeedBackKey,PeerFeedBackId,PeerFeedBackSessionId,PeerFeedBackGroupId,Users")] PeerFeedBackResponseUserModel response)
        {
            log.Info("**************** START PeerFeedbackResponseSave ****************");
            var jsonParam = JsonConvert.SerializeObject(response);
            log.Info($"Event Name = PeerFeedbackResponseSave and Event Param = {jsonParam}");
            if (string.IsNullOrEmpty(response.PeerFeedBackKey))
            {
                log.Warn($"param.PeerFeedBackKey = {response.PeerFeedBackKey}");
                return RedirectToAction("Index", "Error");
            }
            log.Info($"param.PeerFeedBackId = {response.PeerFeedBackId}");
            var peerFeedBack = _peerFeedbackService.PeerFeedbackGetById(response.PeerFeedBackId);
            log.Debug($"PeerFeedbackGetById OK response = {JsonConvert.SerializeObject(peerFeedBack)}");
            log.Info($"param.PeerFeedBackSessionId = {response.PeerFeedBackSessionId}");
            var session = _peerFeedbackService.PeerFeedbackSessionsGetById(response.PeerFeedBackSessionId);
            log.Debug($"PeerFeedbackSessionsGetById OK response = = {JsonConvert.SerializeObject(session)}");
            if (peerFeedBack == null || session == null)
            {
                log.Warn($"peerFeedBack == null || session == null - issue with param.PeerFeedBackId = {response.PeerFeedBackId}, param.PeerFeedBackSessionId = {response.PeerFeedBackSessionId}");
                return RedirectToAction("Index", "Error");
            }
            if (session.PeerFeedbackId != peerFeedBack.Id
                || !string.Equals(response.PeerFeedBackKey, peerFeedBack.Id.ToEncrypt(), StringComparison.Ordinal))
            {
                log.Warn($"Dangerous actions from users - {UserInfo.DisplayName} - {UserInfo.UserId}");
                return RedirectToAction("Index", "Error");
            }

            if (session.EntryCloseTime < DateTime.UtcNow)
            {
                string _errMsg = "Session has ended, you can no longer save your feedback.";
                log.Warn($"{_errMsg}");
                return Json(new
                {
                    ErrorMessage = _errMsg
                });
            }

            var responses = new List<PeerFeedBackResponses>();
            log.Info("start get data to list entity responses");
            foreach (var item in response.Users)
            {
                foreach (var question in item.Questions)
                {
                    foreach (var rating in question.Ratings)
                    {
                        if (rating.RatingId > 0)
                        {
                            foreach (var option in rating.Options)
                            {
                                if (option.OptionId > 0)
                                {
                                    var peerFeedBackResponses = new PeerFeedBackResponses
                                    {
                                        IsDeleted = false,
                                        TargetUserId = item.UserId,
                                        LastUpdateTime = DateTime.UtcNow,
                                        EvaluatorUserId = UserInfo.UserId,
                                        PeerFeedBackRatingId = rating.RatingId,
                                        PeerFeedBackOptionId = option.OptionId,
                                        PeerFeedbackId = response.PeerFeedBackId,
                                        PeerFeedbackQuestionId = question.QuestionId,
                                        PeerFeedbackSessionId = response.PeerFeedBackSessionId,
                                        PeerFeedBackGroupId = response.PeerFeedBackGroupId
                                    };
                                    responses.Add(peerFeedBackResponses);
                                }
                            }
                        }
                    }
                }
            }
            log.Info("data fulfilled list entity responses");
            log.Info("------ start PeerFeedBackResponsesDeletePeerFeedBack ------");
            log.Info($"delete parameters: response.PeerFeedBackId = {response.PeerFeedBackId}, response.PeerFeedBackSessionId = {response.PeerFeedBackSessionId}, response.PeerFeedBackGroupId = {response.PeerFeedBackGroupId}, UserInfo.UserId = {UserInfo.UserId}");
            _peerFeedbackService.PeerFeedBackResponsesDeletePeerFeedBack(response.PeerFeedBackId, response.PeerFeedBackSessionId, response.PeerFeedBackGroupId, UserInfo.UserId);
            log.Info("action PeerFeedBackResponsesDeletePeerFeedBack successful!");
            log.Info("------ end PeerFeedBackResponsesDeletePeerFeedBack ------");
            log.Info("------ start PeerFeedBackResponsesInsert ------");
            log.Info($"insert paramters: {jsonParam}");
            _peerFeedbackService.PeerFeedBackResponsesInsert(responses);
            log.Info("action PeerFeedBackResponsesInsert successful!");
            log.Info("------ end PeerFeedBackResponsesInsert ------");

            var remarks = new List<PeerFeedBackResponseRemarks>();
            log.Info("start get data to list entity responses");
            foreach (var item in response.Users)
            {
                if (item.Remark != null)
                {
                    var peerFeedBackResponseRemarks = new PeerFeedBackResponseRemarks
                    {
                        Id = item.Remark.Id,
                        IsDeleted = false,
                        TargetUserId = item.Remark.TargetUserId,
                        LastUpdateTime = DateTime.UtcNow,
                        EvaluatorUserId = UserInfo.UserId,
                        PeerFeedbackId = response.PeerFeedBackId,
                        PeerFeedbackSessionId = response.PeerFeedBackSessionId,
                        PeerFeedBackGroupId = response.PeerFeedBackGroupId,
                        Remarks = item.Remark.Remarks
                    };
                    remarks.Add(peerFeedBackResponseRemarks);
                }
            }
            log.Info("data fulfilled list entity response remarks");
            //log.Info("------ start PeerFeedBackResponseRemarksDeletePeerFeedBack ------");
            //log.Info($"delete parameters: response.PeerFeedBackId = {response.PeerFeedBackId}, response.PeerFeedBackSessionId = {response.PeerFeedBackSessionId}, response.PeerFeedBackGroupId = {response.PeerFeedBackGroupId}, UserInfo.UserId = {UserInfo.UserId}");
            //_peerFeedbackService.PeerFeedBackResponseRemarksDeletePeerFeedBack(response.PeerFeedBackId, response.PeerFeedBackSessionId, response.PeerFeedBackGroupId, UserInfo.UserId);
            //log.Info("action PeerFeedBackResponseRemarksDeletePeerFeedBack successful!");
            //log.Info("------ end PeerFeedBackResponseRemarksDeletePeerFeedBack ------");
            log.Info("------ start PeerFeedBackResponseRemarksInsert ------");
            log.Info($"insert paramters: {jsonParam}");
            _peerFeedbackService.PeerFeedBackResponseRemarksInsertOrUpdate(remarks);
            log.Info("action PeerFeedBackResponseRemarksInsert successful!");
            log.Info("------ end PeerFeedBackResponseRemarksInsert ------");
            log.Info("**************** END PeerFeedbackResponseSave ****************");
            return Json(string.Empty);
        }
        [PeerFeedBackAuthorize(Role = "admin,instructor")]
        public ActionResult Dashboard()
        {
            return PartialView();
        }

        [PeerFeedBackAuthorize(Role = "admin")]
        public ActionResult DashboardDetail(int peerFeedBackQuestionId, List<string> strms, string school)
        {
            log.Info("**************** START DashboardDetail ****************");

            if (strms == null || !strms.Any())
            {
                log.Warn($"param List strms is null or empty");
                throw new Exception("param List strms is null or empty");
            }
            int totalCountUser = 0;
            Dictionary<int, int> dicOption = new Dictionary<int, int>();
            List<PeerFeedBackResponses> allResponses = new List<PeerFeedBackResponses>();

            var listFilter = _peerFeedbackService.PeerFeedBackGetDashboardFilter(string.Empty);

            var courses = school != "0" ? listFilter.Where(x => x.ACAD_GROUP == school).Select(x => x.CourseId).ToList() : listFilter.Select(x => x.CourseId).ToList();

            log.Info("------ start PeerFeedBackSessionGetListSessionIdByListStrm ------");
            log.Info($"param: listStrm = {strms.ToJson()}");
            //var sessions = _peerFeedbackService.PeerFeedBackSessionGetListSessionIdByListStrm(strms);
            var sessions = _peerFeedbackService.PeerFeedBackSessionGetListSessionIdFiltered(strms, school, courses);
            log.Info("------ end PeerFeedBackSessionGetListSessionIdByListStrm ------");
            var peerFeedBackIds = sessions.Select(x => x.PeerFeedbackId).Distinct().ToList();
            var sessionIds = sessions.Select(x => x.Id).ToList();
            var allTargets = _peerFeedbackService.PeerFeedBackTargetsGetUserIdBySessionList(sessionIds, courses);
            log.Info("------ end PeerFeedBackTargetsGetUserIdBySessionList ------");
            var usersInGroup = allTargets.Select(x => x.UserId).Distinct().ToList();
            var categoyIds = allTargets.Select(x => x.OrgUnitId).Distinct().ToList();
            totalCountUser = _userGroupService.GetUserIdByCategoryIds(categoyIds).Count;
            log.Info("------ start PeerFeedBackResponsesGetPeerFeedBackId ------");
            log.Info($"param: categoyIds = {categoyIds.ToJson()}, sessionIds = {sessionIds.ToJson()}, peerFeedBackIds = {peerFeedBackIds.ToJson()}, peerFeedBackQuestionId = {peerFeedBackQuestionId}");
            allResponses = _peerFeedbackService.PeerFeedBackResponsesGetPeerFeedBackId(categoyIds, sessionIds, peerFeedBackIds, peerFeedBackQuestionId);
            log.Info("------ end PeerFeedBackResponsesGetPeerFeedBackId ------");
            log.Info("get complete evaluation");
            allResponses = GetCompleteEvaluation(allResponses);

            var question = _peerFeedbackService.GetPeerFeedbackQuestionById(peerFeedBackQuestionId);

            var chartItem = new PeerFeedBackChartItem
            {
                Data = new List<int>(),
                BackgroundColor = new List<string>(),
                Labels = new List<string>()
            };
            var model = new PeerFeedBackResultDetailQuestionStatisticViewModel
            {
                ChartData = JsonConvert.SerializeObject(chartItem),
                ChartId = Guid.NewGuid().ToString(),
                TotalUserInGroup = totalCountUser,
                RatingResponse = new List<PeerFeedbackRatingOptionModel>(),
                ResourceId = Guid.NewGuid().ToString(),
                QuestionTitle = question.Title
            };
            log.Info("start fill data to response model");
            if (allResponses.Any())
            {
                allResponses = allResponses.Where(x => x.EvaluatorUserId != x.TargetUserId).ToList();
                model.CountUserComplete = allResponses.Select(x => x.EvaluatorUserId).Distinct().Count(x => usersInGroup.Contains(x));
                log.Info("------ start GetListPeerFeedbackRatingOptions ------");
                var allOptions = _peerFeedbackService.GetListPeerFeedbackRatingOptions();
                log.Info("------ end GetListPeerFeedbackRatingOptions ------");
                var optionModel = new List<PeerFeedbackRatingOptionModel>();
                log.Info("------ start GetListPeerFeedbackRatingQuestions ------");
                var ratingQuestions = _peerFeedbackService.GetListPeerFeedbackRatingQuestions();
                log.Info("------ end GetListPeerFeedbackRatingQuestions ------");
                var ratingColorProvider = new RatingRGBProvider(ratingQuestions);
                log.Info("------ start GetFeedbackQuestionRatingMapByQuestionId ------");
                log.Info($"param: peerFeedBackQuestionId = {peerFeedBackQuestionId}");
                var questionRatingMap = _peerFeedbackService.GetFeedbackQuestionRatingMapByQuestionId(peerFeedBackQuestionId);
                log.Info("------ end GetFeedbackQuestionRatingMapByQuestionId ------");
                var ratingMeetsExpectation = ratingQuestions.Where(x => string.Equals(x.Name, Constants.MeetsExpectations, StringComparison.OrdinalIgnoreCase)).First();
                log.Info("------ start AddDefaultExceedsToMeetExpectations ------");
                log.Info($"param: allResponses = {allResponses}, ratingQuestions = {ratingQuestions.ToJson()}, questionRatingMap = {questionRatingMap.ToJson()}");
                //get meets expectation item for check calculate
                var targetResponses = AddDefaultExceedsToMeetExpectations(allResponses, ratingQuestions, questionRatingMap);
                log.Info("------ end AddDefaultExceedsToMeetExpectations ------");
                log.Info("Add chart items");
                targetResponses = targetResponses.Where(x => x.EvaluatorUserId != x.TargetUserId).ToList();
                for (var index = 0; index < ratingQuestions.Count; index++)
                {
                    var ratingQuestion = ratingQuestions[index];
                    var ratingCount = targetResponses.Where(x => x.PeerFeedBackRatingId == ratingQuestion.Id).Count();
                    chartItem.Labels.Add(ratingQuestion.Name);
                    chartItem.BackgroundColor.Add(ratingColorProvider.GetRatingColorCodes(ratingQuestion.Id));
                    chartItem.Data.Add(ratingCount);
                }
                var ratingQuestionsModel = new List<PeerFeedbackRatingQuestionModel>();
                model.ChartData = JsonConvert.SerializeObject(chartItem);
                log.Info("Add chart data");
                foreach (var ratingQuestion in ratingQuestions)
                {
                    //flag for check calculate for meets expectations
                    var isCalculateForMeets = ratingMeetsExpectation.Id == ratingQuestion.Id;
                    var items = questionRatingMap.Where(x => x.RatingQuestionId == ratingQuestion.Id).ToList();
                    //filter responses by ratingId
                    var responseByRating = targetResponses.Where(x => x.PeerFeedBackRatingId == ratingQuestion.Id).ToList();
                    if (items.Any() && responseByRating.Any())
                    {
                        var rtModel = new PeerFeedbackRatingQuestionModel
                        {
                            Id = ratingQuestion.Id,
                            Name = ratingQuestion.Name,
                            RatingOptions = new List<PeerFeedbackRatingOptionModel>()
                        };
                        foreach (var item in items)
                        {
                            var option = allOptions.First(x => x.Id == item.RatingOptionId);
                            double responseCount = responseByRating.Count(r => r.PeerFeedBackOptionId == option.Id);
                            var opt = new PeerFeedbackRatingOptionModel { OptionName = option.Name, RatingOptionId = option.Id };
                            if (responseCount > 0)
                            {
                                opt.ResponseCount = Convert.ToInt32(responseCount);
                                opt.ColorCode = ratingColorProvider.GetRatingColorCodes(item.RatingQuestionId);
                                //if is calculate for meets expectations we need to distinct by EvaluatorUserId, PeerFeedBackOptionId
                                responseCount = responseByRating
                                                     .Where(r => r.PeerFeedBackOptionId == option.Id)
                                                     .Select(x => new { x.PeerFeedBackOptionId, x.EvaluatorUserId, x.PeerFeedBackGroupId })
                                                     .Distinct()
                                                     .Count();
                                opt.Progress = responseCount / model.TotalUserInGroup * 100;
                            }
                            else
                            {
                                opt.Progress = 0;
                                opt.ResponseCount = 0;
                                opt.ColorCode = string.Empty;
                            }
                            rtModel.RatingOptions.Add(opt);
                        }
                        ratingQuestionsModel.Add(rtModel);
                    }
                }
                model.RatingResponse.AddRange(optionModel);
                model.RatingQuestion = ratingQuestionsModel;
            }
            log.Info("finish adding chart data");
            log.Info("**************** END DashboardDetail ****************");
            return PartialView("_DashboardDetail", model);
        }

        private List<PeerFeedbackRatingOptionModel> GetDataAllGroup(
            int peerFeedBackId,
            int peerFeedBackPairingId,
            int peerFeedBackQuestionId,
            int peerFeedBackSessionId,
            List<PeerFeedbackRatingOption> allOptions,
            Dictionary<int, string> dicColorCode)
        {
            log.Info("**************** START GetDataAllGroup ****************");
            var optionModel = new List<PeerFeedbackRatingOptionModel>();
            log.Info("------ start PeerFeedBackTargetsGetGroup ------");
            log.Info($"param: peerFeedBackPairingId = {peerFeedBackPairingId}");
            var groupIds = _peerFeedbackService.PeerFeedBackTargetsGetGroup(peerFeedBackPairingId);
            log.Info("------ end PeerFeedBackTargetsGetGroup ------");

            log.Info("------ start PeerFeedBackResponsesGetData ------");
            log.Info($"param: peerFeedBackId = {peerFeedBackId}, peerFeedBackQuestionId = {peerFeedBackQuestionId}, peerFeedBackSessionId = {peerFeedBackSessionId}");
            var allGroupResponses = _peerFeedbackService.PeerFeedBackResponsesGetData(peerFeedBackId, peerFeedBackQuestionId, peerFeedBackSessionId);
            log.Info("------ end PeerFeedBackResponsesGetData ------");

            log.Info("start get data to list entity responses");
            foreach (var peerFeedBackGroupId in groupIds)
            {
                var usersInGroup = _userService.GetByCategoryGroupId(peerFeedBackGroupId).Where(x => x.Id != UserInfo.UserId).ToList();
                var allResponses = allGroupResponses.Where(x => x.PeerFeedBackGroupId == peerFeedBackGroupId).ToList();
                Dictionary<int, int> dicOption = new Dictionary<int, int>();
                var questionRatingMap = _peerFeedbackService.GetFeedbackQuestionRatingMapByQuestionId(peerFeedBackQuestionId);

                if (allResponses.Any())
                {
                    foreach (var it in questionRatingMap)
                    {
                        var responseCount = allResponses.Count(r => r.TargetUserId == UserInfo.UserId && r.EvaluatorUserId != UserInfo.UserId &&
                                r.PeerFeedBackRatingId == it.RatingQuestionId &&
                                r.PeerFeedBackOptionId == it.RatingOptionId);
                        if (responseCount > 0)
                        {
                            if (dicOption.ContainsKey(it.RatingOptionId))
                            {
                                var count = dicOption[it.RatingOptionId];
                                dicOption[it.RatingOptionId] = count + responseCount;
                            }
                            else
                            {
                                dicOption.Add(it.RatingOptionId, responseCount);
                            }
                        }
                    }
                }
                foreach (var item in questionRatingMap)
                {
                    var option = allOptions.First(x => x.Id == item.RatingOptionId);
                    var response = allResponses.Where(r =>
                        r.PeerFeedBackOptionId == option.Id &&
                        r.PeerFeedBackRatingId == item.RatingQuestionId &&
                        r.TargetUserId == UserInfo.UserId && r.EvaluatorUserId != UserInfo.UserId).ToList();
                    if (response.Any())
                    {
                        double responseCount = response.Count;
                        if (responseCount > 0)
                        {
                            var opt = new PeerFeedbackRatingOptionModel
                            {
                                RatingOptionId = option.Id,
                                OptionName = option.Name,
                                QuestionId = item.QuestionId,
                                RatingQuestionId = item.RatingQuestionId,
                                ResponseCount = Convert.ToInt32(responseCount),
                                StatisticDataType = (int)PeerFeedBackStatisticDataType.RateEachOther,
                                Progress = dicOption.ContainsKey(option.Id) ? (responseCount / (allOptions.Count(x => x.Id == item.RatingQuestionId) * (usersInGroup.Count))) * 100 : 0,
                                ColorCode = dicColorCode.ContainsKey(item.RatingQuestionId) ? dicColorCode[item.RatingQuestionId] : string.Empty,
                            };
                            optionModel.Add(opt);
                        }
                    }
                }
            }
            var result = new List<PeerFeedbackRatingOptionModel>();
            foreach (var option in allOptions)
            {
                var data = optionModel.Where(x => x.RatingOptionId == option.Id).ToList();
                if (data.Any())
                {
                    var model = data.First();
                    model.ResponseCount = data.Sum(x => x.ResponseCount);
                    model.Progress = data.Sum(x => x.Progress) / data.Count;
                    result.Add(model);
                }
            }
            log.Info("end get data to list entity responses");
            log.Info("**************** END GetDataAllGroup ****************");
            return result;
        }
        [PeerFeedBackAuthorize(Role = "student")]
        public ActionResult PeerFeedBackResultQuestionStatistic(string key, int peerFeedBackId, int peerFeedBackSessionId, int peerFeedBackGroupId, int peerFeedBackQuestionId, int peerFeedBackPairingId)
        {
            log.Info("**************** START PeerFeedBackResultQuestionStatistic ****************");
            log.Info("Check paramKey");
            var paramKey = $"{peerFeedBackId}{peerFeedBackSessionId}{peerFeedBackGroupId}".ToEncrypt();
            if (string.IsNullOrEmpty(key) || !string.Equals(key, paramKey))
            {
                log.Warn($"Param key is not correct - issue with param.PeerFeedBackId = {peerFeedBackId}, param.PeerFeedBackSessionId = {peerFeedBackSessionId}");
                return RedirectToAction("Index", "Error");
            }
            log.Info("------ start GetListPeerFeedbackRatingOptions ------");
            var allOptions = _peerFeedbackService.GetListPeerFeedbackRatingOptions();
            log.Info("------ end GetListPeerFeedbackRatingOptions ------");
            var optionModel = new List<PeerFeedbackRatingOptionModel>();
            var usersInGroup = _userService.GetByCategoryGroupId(peerFeedBackGroupId).Where(x => x.Id != UserInfo.UserId).ToList();
            log.Info("------ start PeerFeedBackResponsesGetList ------");
            var allResponses = _peerFeedbackService.PeerFeedBackResponsesGetList(peerFeedBackId, peerFeedBackQuestionId, peerFeedBackSessionId, peerFeedBackGroupId);
            allResponses = GetCompleteEvaluation(allResponses, peerFeedBackId, peerFeedBackQuestionId, peerFeedBackSessionId, peerFeedBackGroupId);
            log.Info("------ end PeerFeedBackResponsesGetList ------");
            log.Info("------ start GetFeedbackQuestionRatingMapByQuestionId ------");
            var questionRatingMap = _peerFeedbackService.GetFeedbackQuestionRatingMapByQuestionId(peerFeedBackQuestionId);
            log.Info("------ end GetFeedbackQuestionRatingMapByQuestionId ------");
            log.Info("------ start GetPeerFeedbackQuestionById ------");
            var question = _peerFeedbackService.GetPeerFeedbackQuestionById(peerFeedBackQuestionId);
            log.Info("------ end GetPeerFeedbackQuestionById ------");
            if (question == null) return RedirectToAction("Index", "Error");
            int countUserComplete = 0;
            if (allResponses.Any())
            {
                allResponses = allResponses.Where(x => x.TargetUserId == UserInfo.UserId).ToList();
                foreach (var user in usersInGroup)
                {
                    if (allResponses.Any(r => r.EvaluatorUserId == user.Id))
                    {
                        countUserComplete++;
                    }
                }
            }
            log.Info("------ start GetListPeerFeedbackRatingQuestions ------");
            var ratingQuestions = _peerFeedbackService.GetListPeerFeedbackRatingQuestions();
            log.Info("------ end GetListPeerFeedbackRatingQuestions ------");
            log.Info("start adding chart data");
            var chartItem = new PeerFeedBackChartItem
            {
                Data = new List<int>(),
                BackgroundColor = new List<string>(),
                Labels = new List<string>()
            };
            var ratingColorProvider = new RatingRGBProvider(ratingQuestions);
            for (var index = 0; index < ratingQuestions.Count; index++)
            {
                var ratingQuestion = ratingQuestions[index];
                var ratingCount = allResponses.Where(x => x.PeerFeedBackRatingId == ratingQuestion.Id && x.EvaluatorUserId != UserInfo.UserId).Select(x => x.EvaluatorUserId).Distinct().Count();
                if (ratingCount > 0)
                {
                    chartItem.Labels.Add(ratingQuestion.Name);
                    chartItem.BackgroundColor.Add(ratingColorProvider.GetRatingColorCodes(ratingQuestion.Id));
                    chartItem.Data.Add(ratingCount);
                }
            }
            log.Info("add DetailQuestionStatistic");
            var model = new PeerFeedBackResultDetailQuestionStatisticViewModel
            {
                QuestionTitle = question.Title,
                ChartData = JsonConvert.SerializeObject(chartItem),
                ChartId = DateTime.UtcNow.Ticks.ToString(),
                TotalUserInGroup = usersInGroup.Count,
                CountUserComplete = countUserComplete,
                ResourceId = Guid.NewGuid().ToString(),
                GroupBy = Convert.ToInt32(PeerFeedBackResultGroupBy.AssignedGroup),
                RatingResponse = new List<PeerFeedbackRatingOptionModel>()
            };
            var ratingQuestionsModel = new List<PeerFeedbackRatingQuestionModel>();
            var responsesMySelf = allResponses.Where(r => r.EvaluatorUserId == UserInfo.UserId).Select(x => x.PeerFeedBackRatingId).Distinct().FirstOrDefault();
            var targetResponses = allResponses.Where(x => x.EvaluatorUserId != UserInfo.UserId).ToList();
            //get meets expectation item for calculate 
            var ratingMeetsExpectation = ratingQuestions.Where(x => string.Equals(x.Name, Constants.MeetsExpectations, StringComparison.OrdinalIgnoreCase)).First();
            log.Info("Add DefaultExceedsToMeetExpectations");
            targetResponses = AddDefaultExceedsToMeetExpectations(targetResponses, ratingQuestions, questionRatingMap);
            log.Info("Loop through ratingQuestions");
            foreach (var ratingQuestion in ratingQuestions)
            {
                //flag for check calculate for meets expectations
                var isCalculateForMeets = ratingMeetsExpectation.Id == ratingQuestion.Id;
                var items = questionRatingMap.Where(x => x.RatingQuestionId == ratingQuestion.Id).ToList();
                //filter responses by ratingId
                var responseByRating = targetResponses.Where(x => x.PeerFeedBackRatingId == ratingQuestion.Id).ToList();
                if (items.Any() && responseByRating.Any())
                {
                    var rtModel = new PeerFeedbackRatingQuestionModel
                    {
                        Id = ratingQuestion.Id,
                        Name = ratingQuestion.Name,
                        RatingOptions = new List<PeerFeedbackRatingOptionModel>()
                    };
                    foreach (var item in items)
                    {
                        var option = allOptions.First(x => x.Id == item.RatingOptionId);
                        double responseCount = responseByRating.Count(r => r.PeerFeedBackOptionId == option.Id);
                        var opt = new PeerFeedbackRatingOptionModel { OptionName = option.Name, RatingOptionId = option.Id };
                        if (responseCount > 0)
                        {
                            opt.ResponseCount = Convert.ToInt32(responseCount);
                            opt.ColorCode = ratingColorProvider.GetRatingColorCodes(item.RatingQuestionId);
                            //if is calculate for meets expectations we need to distinct by EvaluatorUserId, PeerFeedBackOptionId
                            responseCount = responseByRating
                                                 .Where(r => r.PeerFeedBackOptionId == option.Id)
                                                 .Select(x => new { x.PeerFeedBackOptionId, x.EvaluatorUserId, x.PeerFeedBackGroupId })
                                                 .Distinct()
                                                 .Count();
                            opt.Progress = responseCount / model.TotalUserInGroup * 100;
                        }
                        else
                        {
                            opt.Progress = 0;
                            opt.ResponseCount = 0;
                            opt.ColorCode = string.Empty;
                        }
                        rtModel.RatingOptions.Add(opt);
                    }
                    ratingQuestionsModel.Add(rtModel);
                }
            }
            log.Info($"Check response Myself has value: {responsesMySelf.HasValue}");
            if (responsesMySelf.HasValue)
            {
                var myresponse = ratingQuestions.First(r => r.Id == responsesMySelf.Value);
                var mySelfResponse = new TextColorValue
                {
                    Text = myresponse.Name,
                    ColorCode = ratingColorProvider.GetRatingColorCodes(myresponse.Id)
                };
                model.SelfValue = mySelfResponse;
            }
            model.RatingQuestion = ratingQuestionsModel;
            log.Info("finish adding chart data");
            log.Info("**************** END PeerFeedBackResultQuestionStatistic ****************");
            return PartialView("_PeerFeedBackResultQuestionStatistic", model);
        }
        [PeerFeedBackAuthorize(Role = "student")]
        public ActionResult PeerFeedBackResultQuestionStatisticAllCourses(int peerFeedBackQuestionId)
        {
            log.Info("**************** START PeerFeedBackResultQuestionStatisticAllCourses ****************");
            log.Info("------ start GetListPeerFeedbackRatingOptions ------");
            var allOptions = _peerFeedbackService.GetListPeerFeedbackRatingOptions();
            log.Info("------ end GetListPeerFeedbackRatingOptions ------");
            var optionModel = new List<PeerFeedbackRatingOptionModel>();
            log.Info("------ start GetPeerFeedbackQuestionById ------");
            log.Info($"param: peerFeedBackQuestionId = {peerFeedBackQuestionId}");
            var question = _peerFeedbackService.GetPeerFeedbackQuestionById(peerFeedBackQuestionId);
            log.Info("------ end GetPeerFeedbackQuestionById ------");
            if (question == null)
            {
                log.Warn($"question == null - peerFeedBackQuestionId = {peerFeedBackQuestionId}");
                return RedirectToAction("Index", "Error");
            }
            log.Info("------ start GetFeedbackQuestionRatingMapByQuestionId ------");
            log.Info($"param: peerFeedBackQuestionId = {peerFeedBackQuestionId}");
            var questionRatingMap = _peerFeedbackService.GetFeedbackQuestionRatingMapByQuestionId(peerFeedBackQuestionId);
            log.Info("------ end GetFeedbackQuestionRatingMapByQuestionId ------");
            log.Info("------ start PeerFeedBackResponsesGetByQuestion ------");
            log.Info($"param: peerFeedBackQuestionId: {peerFeedBackQuestionId}, UserId = {UserInfo.UserId}");
            var allResponses = GetCompleteEvaluation(null, 0, peerFeedBackQuestionId, 0, 0, true);
            allResponses = allResponses.Where(x => x.TargetUserId == UserInfo.UserId).ToList();

            List<PeerFeedbackResultModel> peerFeedbackResultModelList = GetPeerFeedbackResultModelList().Where(x => x.Session.PeerFeedbackResultSessionStatus == PeerFeedbackResultSessionStatus.Closed).ToList();
            allResponses = allResponses.Where(x => peerFeedbackResultModelList.Any(y => y.PeerFeedBackSessionId == x.PeerFeedbackSessionId)).ToList();

            log.Info("------ end PeerFeedBackResponsesGetByQuestion ------");
            List<int> users = new List<int>();
            if (allResponses.Any())
            {
                var filters = allResponses.Select(x => new PeerFeedBackResponseGroupByModel
                {
                    PeerFeedBackGroupId = x.PeerFeedBackGroupId,
                    PeerFeedbackId = x.PeerFeedbackId,
                    PeerFeedbackSessionId = x.PeerFeedbackSessionId
                }).DistinctBy(x => new
                {
                    x.PeerFeedbackId,
                    x.PeerFeedBackGroupId,
                    x.PeerFeedbackSessionId
                }).ToList();
                log.Info("Start adding data");
                foreach (var filter in filters)
                {
                    var session = _peerFeedbackService.PeerFeedbackSessionsGetById(filter.PeerFeedbackSessionId);
                    if (session.EntryCloseTime >= DateTime.UtcNow) continue;

                    var allResponsesByGroup = _peerFeedbackService.PeerFeedBackResponsesGetList(filter.PeerFeedbackId, filter.PeerFeedbackSessionId, filter.PeerFeedBackGroupId);
                    double progress = 0;
                    var responses = allResponsesByGroup.Where(x => x.EvaluatorUserId == UserInfo.UserId).ToList();
                    if (responses.Any())
                    {
                        double userCompleteCount = 0;
                        var peerFeedBackQuestions = _peerFeedbackService.PeerFeedbackQuestionMapList(filter.PeerFeedbackId);
                        var usersInGroup = _userService.GetByCategoryGroupId(filter.PeerFeedBackGroupId).ToList();
                        foreach (var user in usersInGroup)
                        {
                            double questionCompleteCount = 0;

                            foreach (var peerFeedBackQuestion in peerFeedBackQuestions)
                            {
                                HybridDictionary dicRatingComplete = new HybridDictionary();
                                var items = responses.Where(x => x.TargetUserId == user.Id && x.PeerFeedbackQuestionId == peerFeedBackQuestion.Id).ToList();
                                if (items.Any())
                                {
                                    var questionRatingMapByGroup = _peerFeedbackService.GetFeedbackQuestionRatingMapByQuestionId(peerFeedBackQuestion.Id);
                                    foreach (var it in questionRatingMapByGroup)
                                    {
                                        if (items.Any(r => r.PeerFeedBackRatingId == it.RatingQuestionId) && !dicRatingComplete.Contains(it.RatingQuestionId))
                                            dicRatingComplete.Add(it.RatingQuestionId, true);
                                    }
                                    if (dicRatingComplete.Count > 0)
                                        questionCompleteCount++;
                                }
                            }
                            if (questionCompleteCount.Equals(peerFeedBackQuestions.Count))
                                userCompleteCount++;
                        }
                        progress = (userCompleteCount / (usersInGroup.Count)) * 100;
                        if (progress < 100)
                        {
                            allResponses.RemoveAll(x => x.PeerFeedbackId == filter.PeerFeedbackId && x.PeerFeedbackSessionId == filter.PeerFeedbackSessionId && x.PeerFeedBackGroupId == filter.PeerFeedBackGroupId);
                        }
                        else
                        {
                            users.AddRange(usersInGroup.Where(x => x.Id != UserInfo.UserId).Select(x => x.Id).ToList());
                        }
                    }
                }
            }
            var ratingQuestions = _peerFeedbackService.GetListPeerFeedbackRatingQuestions();
            log.Info("------ end GetListPeerFeedbackRatingQuestions ------");
            var chartItem = new PeerFeedBackChartItem
            {
                Data = new List<int>(),
                BackgroundColor = new List<string>(),
                Labels = new List<string>()
            };
            var ratingColorProvider = new RatingRGBProvider(ratingQuestions);
            var ratingQuestionsModel = new List<PeerFeedbackRatingQuestionModel>();
            var responsesMySelf = allResponses.Where(r => r.EvaluatorUserId == UserInfo.UserId).Select(x => x.PeerFeedBackRatingId).Distinct().FirstOrDefault();
            var targetResponses = allResponses.Where(x => x.EvaluatorUserId != UserInfo.UserId).ToList();

            for (var index = 0; index < ratingQuestions.Count; index++)
            {
                var ratingQuestion = ratingQuestions[index];
                var ratingCount = targetResponses.Where(x => x.PeerFeedBackRatingId == ratingQuestion.Id && x.EvaluatorUserId != UserInfo.UserId).Select(x => new { x.EvaluatorUserId, x.PeerFeedbackSessionId }).Distinct().Count();
                if (ratingCount > 0)
                {
                    chartItem.Labels.Add(ratingQuestion.Name);
                    chartItem.BackgroundColor.Add(ratingColorProvider.GetRatingColorCodes(ratingQuestion.Id));
                    chartItem.Data.Add(ratingCount);
                }
            }
            var model = new PeerFeedBackResultDetailQuestionStatisticViewModel
            {
                QuestionTitle = question.Title,
                ChartData = JsonConvert.SerializeObject(chartItem),
                ChartId = DateTime.UtcNow.Ticks.ToString(),
                ResourceId = Guid.NewGuid().ToString(),
                TotalUserInGroup = users.Count,
                RatingResponse = new List<PeerFeedbackRatingOptionModel>()
            };

            //get meets expectation item for check calculate
            var ratingMeetsExpectation = ratingQuestions.Where(x => string.Equals(x.Name, Constants.MeetsExpectations, StringComparison.OrdinalIgnoreCase)).First();
            log.Info("------ start AddDefaultExceedsToMeetExpectations ------");
            log.Info($"param: targetResponses = {targetResponses.ToJson()}, ratingQuestions = {ratingQuestions.ToJson()}, questionRatingMap = {questionRatingMap.ToJson()}");
            targetResponses = AddDefaultExceedsToMeetExpectations(targetResponses, ratingQuestions, questionRatingMap);
            log.Info("------ end AddDefaultExceedsToMeetExpectations ------");

            log.Info("loop through ratingQuestions");
            foreach (var ratingQuestion in ratingQuestions)
            {
                //flag for check calculate for meets expectations
                var isCalculateForMeets = ratingMeetsExpectation.Id == ratingQuestion.Id;
                var items = questionRatingMap.Where(x => x.RatingQuestionId == ratingQuestion.Id).ToList();
                //filter responses by ratingId
                var responseByRating = targetResponses.Where(x => x.PeerFeedBackRatingId == ratingQuestion.Id).ToList();
                if (items.Any() && responseByRating.Any())
                {
                    var rtModel = new PeerFeedbackRatingQuestionModel
                    {
                        Id = ratingQuestion.Id,
                        Name = ratingQuestion.Name,
                        RatingOptions = new List<PeerFeedbackRatingOptionModel>()
                    };
                    foreach (var item in items)
                    {
                        var option = allOptions.First(x => x.Id == item.RatingOptionId);
                        double responseCount = responseByRating.Count(r => r.PeerFeedBackOptionId == option.Id);
                        var opt = new PeerFeedbackRatingOptionModel { OptionName = option.Name, RatingOptionId = option.Id };
                        if (responseCount > 0)
                        {
                            opt.ResponseCount = Convert.ToInt32(responseCount);
                            opt.ColorCode = ratingColorProvider.GetRatingColorCodes(item.RatingQuestionId);
                            //if is calculate for meets expectations we need to distinct by EvaluatorUserId, PeerFeedBackOptionId
                            responseCount = responseByRating
                                                  .Where(r => r.PeerFeedBackOptionId == option.Id)
                                                  .Select(x => new { x.PeerFeedBackOptionId, x.EvaluatorUserId, x.PeerFeedBackGroupId, x.PeerFeedbackSessionId, x.PeerFeedbackId })
                                                  .Distinct()
                                                  .Count();
                            opt.Progress = responseCount / model.TotalUserInGroup * 100;
                        }
                        else
                        {
                            opt.Progress = 0;
                            opt.ResponseCount = 0;
                            opt.ColorCode = string.Empty;
                        }
                        rtModel.RatingOptions.Add(opt);
                    }
                    ratingQuestionsModel.Add(rtModel);
                }
            }
            log.Info("finish adding chart data");
            model.GroupBy = (int)PeerFeedBackResultGroupBy.AllCourses;
            model.RatingQuestion = ratingQuestionsModel;
            log.Info("**************** END PeerFeedBackResultQuestionStatisticAllCourses ****************");
            return PartialView("_PeerFeedBackResultQuestionStatistic", model);
        }
        public ActionResult SelfDirectedLearningResources(string resourceId, string questionTitle)
        {
            log.Info("**************** START SelfDirectedLearningResources ****************");

            var model = new SelfDirectedLearningResourcesModel();
            var templateFile = $"{Server.MapPath(Constants.StaticFilesFolder)}/SelfDirectedLearningResources.json";
            var content = System.IO.File.ReadAllText(templateFile);
            if (!string.IsNullOrEmpty(content))
            {
                log.Info("json convert to List<SelfDirectedLearningResourcesModel>");
                var items = JsonConvert.DeserializeObject<List<SelfDirectedLearningResourcesModel>>(content);
                if (string.Compare(questionTitle, Constants.ResponsibilityQuestionText, true) == 0)
                {
                    model = items.First(x => x.ItemType == 1);
                }
                else if (string.Compare(questionTitle, Constants.MeetsExpectationsQuestionText, true) == 0)
                {
                    model = items.First(x => x.ItemType == 2);
                }
                else if (string.Compare(questionTitle, Constants.ExceedsExpectationsQuestionText, true) == 0)
                {
                    model = items.First(x => x.ItemType == 3);
                }
                model.QuestionName = questionTitle;
                model.ResourceId = resourceId;
            }
            log.Info("**************** END SelfDirectedLearningResources ****************");
            return PartialView("_SelfDirectedLearningResources", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AuditUserAction([Bind(Include = "Question,CourseId,ResourceId")] AuditActionModel model)
        {
            log.Info("**************** START AuditUserAction ****************");

            log.Info("Get resource Id.");
            if (model.Question.ToLower() == Constants.ResponsibilityQuestionText.ToLower())
                model.ResourceId = AuditResourceId.ClickResponsibilityAndCommitment;
            else if (model.Question.ToLower() == Constants.MeetsExpectationsQuestionText.ToLower())
                model.ResourceId = AuditResourceId.ClickContributionTowardsTeamEffectiveness;
            else if (model.Question.ToLower() == Constants.ExceedsExpectationsQuestionText.ToLower())
                model.ResourceId = AuditResourceId.ClickContributionTowardsTeamDeliverables;
            log.Info($"ResourceId: {model.ResourceId}");
            var audit = new AuditEntry
            {
                AuditType = AuditType.Click,
                UserId = UserInfo.UserId.ToString(),
                ToolId = _webHelper.CategoryName,
                ResourceId = (int)model.ResourceId,
                OrgUnitId = model.CourseId
            };

            log.Info("------ start AuditUserActrion ------");
            _loggingService.AuditUserAction(audit);
            log.Info("------ end AuditUserActrion ------");
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetResponses"></param>
        /// <param name="ratingQuestions"></param>
        /// <param name="questionRatingMap"></param>
        /// <returns></returns>
        private List<PeerFeedBackResponses> AddDefaultExceedsToMeetExpectations(List<PeerFeedBackResponses> targetResponses, List<PeerFeedbackRatingQuestion> ratingQuestions, List<PeerFeedbackQuestionRatingMap> questionRatingMap)
        {
            //get question Meets expectations by question name
            var meetsExpectationsItem = ratingQuestions.Where(x => string.Equals(x.Name, Constants.MeetsExpectations, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            //get question Exceeds expectations by question name
            var exceedsExpectationsItem = ratingQuestions.Where(x => string.Equals(x.Name, Constants.ExceedsExpectations, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (exceedsExpectationsItem != null && meetsExpectationsItem != null)
            {
                //get list question Exceeds expectations in the list targetResponses by question id
                var exceedsExpectations = targetResponses
                                            .Where(x => x.PeerFeedBackRatingId == exceedsExpectationsItem.Id)
                                            .DistinctBy(x => new
                                            {
                                                x.TargetUserId,
                                                x.PeerFeedbackId,
                                                x.EvaluatorUserId,
                                                x.PeerFeedBackGroupId,
                                                x.PeerFeedBackRatingId,
                                                x.PeerFeedbackSessionId,
                                                x.PeerFeedbackQuestionId
                                            })
                                            .ToList();
                if (exceedsExpectations != null && exceedsExpectations.Any())
                {
                    //get list PeerFeedbackQuestionRatingMap by question Meets expectations
                    var maps = questionRatingMap.Where(x => x.RatingQuestionId == meetsExpectationsItem.Id).ToList();
                    foreach (var exceedItem in exceedsExpectations)
                    {
                        foreach (var map in maps)
                        {
                            //clone Exceeds expectations item foreach option
                            var newItem = new PeerFeedBackResponses
                            {
                                PeerFeedBackOptionId = map.RatingOptionId,
                                PeerFeedbackQuestionId = map.QuestionId,
                                PeerFeedBackRatingId = map.RatingQuestionId,
                                TargetUserId = exceedItem.TargetUserId,
                                EvaluatorUserId = exceedItem.EvaluatorUserId,
                                PeerFeedBackGroupId = exceedItem.PeerFeedBackGroupId,
                                PeerFeedbackSessionId = exceedItem.PeerFeedbackSessionId,
                                LastUpdateTime = exceedItem.LastUpdateTime,
                                PeerFeedbackId = exceedItem.PeerFeedbackId
                            };
                            targetResponses.Add(newItem);
                        }
                    }
                }
            }
            return targetResponses;
        }

        private List<PeerFeedBackResponses> GetCompleteEvaluation(List<PeerFeedBackResponses> allResponses,
            int peerFeedBackId = 0,
            int peerFeedBackQuestionId = 0,
            int peerFeedBackSessionId = 0,
            int peerFeedBackGroupId = 0, bool isAllCourseStats = false)
        {
            log.Info("------ start GetCompleteEvaluation ------");
            List<PeerFeedBackSessionGroupModel> filterResponse = new List<PeerFeedBackSessionGroupModel>();
            if (peerFeedBackId > 0)
            {
                log.Info($"PeerFeedbackId = {peerFeedBackId}");
                filterResponse.Add(new PeerFeedBackSessionGroupModel
                {
                    PeerFeedBackGroupId = peerFeedBackGroupId,
                    PeerFeedBackId = peerFeedBackId,
                    PeerFeedBackSessionId = peerFeedBackSessionId,
                    PeerFeedBackQuestionId = peerFeedBackQuestionId
                });
            }
            else
            {
                if (isAllCourseStats)
                {
                    log.Info("------ start PeerFeedBackResponsesGetByQuestion ------");
                    log.Info($"Param: peerFeedBackQuestionId = {peerFeedBackQuestionId}, UserId = {UserInfo.UserId}");
                    allResponses = _peerFeedbackService.PeerFeedBackResponsesGetByQuestion(peerFeedBackQuestionId, UserInfo.UserId);
                    log.Info("------ start PeerFeedBackResponsesGetByQuestion ------");
                }
                filterResponse = allResponses.Select(x => new PeerFeedBackSessionGroupModel
                {
                    PeerFeedBackGroupId = x.PeerFeedBackGroupId,
                    PeerFeedBackId = x.PeerFeedbackId,
                    PeerFeedBackSessionId = x.PeerFeedbackSessionId,
                    PeerFeedBackQuestionId = x.PeerFeedbackQuestionId
                }).DistinctBy(x => new
                {
                    x.PeerFeedBackGroupId,
                    x.PeerFeedBackId,
                    x.PeerFeedBackSessionId,
                    x.PeerFeedBackQuestionId
                }).ToList();
            }
            if (filterResponse.Any())
            {
                List<PeerFeedBackSessionGroupModel> itemToRemove = new List<PeerFeedBackSessionGroupModel>();
                log.Info("------ start GetListPeerFeedbackQuestions ------");
                var peerFeedBackQuestions = _peerFeedbackService.GetListPeerFeedbackQuestions();
                log.Info("------ start GetListPeerFeedbackQuestions ------");
                foreach (var filter in filterResponse)
                {
                    var targets = _peerFeedbackService.PeerFeedbackTargetsGetByPairingGroupSession(filter.PeerFeedBackSessionId, filter.PeerFeedBackGroupId);
                    int userCount = targets.Select(x => x.UserId).Distinct().Count();
                    var session = _peerFeedbackService.PeerFeedbackSessionsGetById(filter.PeerFeedBackSessionId);
                    if (session == null || session.EntryCloseTime >= DateTime.UtcNow) continue;

                    var responsesGroup = _peerFeedbackService.PeerFeedBackResponsesGetList(filter.PeerFeedBackId, filter.PeerFeedBackSessionId, filter.PeerFeedBackGroupId);
                    foreach (var user in targets)
                    {
                        var usersWithQuestion = responsesGroup
                           .Where(x => x.EvaluatorUserId == user.UserId)
                           .Select(x => new PeerFeedBackResponses
                           {
                               TargetUserId = x.TargetUserId,
                               PeerFeedbackQuestionId = x.PeerFeedbackQuestionId
                           }).Distinct().ToList();
                        var userGroupCount = usersWithQuestion.Select(x => x.TargetUserId).Distinct().Count();
                        var questionGroupCount = usersWithQuestion.GroupBy(p => p.TargetUserId, p => p.PeerFeedbackQuestionId, (targetUserId, questions) => new { userId = targetUserId, QuestionCount = questions.Distinct().Count() }).Select(x => x.QuestionCount).Sum();

                        if (userGroupCount != userCount || questionGroupCount != peerFeedBackQuestions.Count * userCount)
                        {
                            itemToRemove.Add(new PeerFeedBackSessionGroupModel
                            {
                                EvaluatorUserId = user.UserId,
                                PeerFeedBackGroupId = filter.PeerFeedBackGroupId,
                                PeerFeedBackSessionId = filter.PeerFeedBackSessionId,
                                PeerFeedBackId = filter.PeerFeedBackId
                            });
                        }
                    }
                }
                if (itemToRemove.Any())
                {
                    log.Info("Have item need to remove");
                    foreach (var item in itemToRemove)
                    {
                        allResponses.RemoveAll(x => x.PeerFeedbackId == item.PeerFeedBackId
                                                                   && x.PeerFeedBackGroupId == item.PeerFeedBackGroupId
                                                                   && x.EvaluatorUserId == item.EvaluatorUserId
                                                                   && x.PeerFeedbackSessionId == item.PeerFeedBackSessionId);
                    }
                }
            }
            log.Info("------ end GetCompleteEvaluation ------");
            return allResponses;
        }

        private List<PeerFeedBackResponses> GetCompleteEvaluationCsv(List<PeerFeedBackResponses> allResponses,
            List<PeerFeedBackResponses> allDataQuestionResponse, List<PeerFeedbackTargetsDto> allTarget, List<PeerFeedbackQuestion> peerFeedBackQuestions)
        {
            log.Info("------ start GetCompleteEvaluationCsv ------");
            var filterResponse = new List<PeerFeedBackSessionGroupModel>();

            filterResponse = allResponses.Select(x => new PeerFeedBackSessionGroupModel
            {
                PeerFeedBackGroupId = x.PeerFeedBackGroupId,
                PeerFeedBackId = x.PeerFeedbackId,
                PeerFeedBackSessionId = x.PeerFeedbackSessionId,
                PeerFeedBackQuestionId = x.PeerFeedbackQuestionId
            }).DistinctBy(x => new
            {
                x.PeerFeedBackGroupId,
                x.PeerFeedBackId,
                x.PeerFeedBackSessionId,
                x.PeerFeedBackQuestionId
            }).ToList();

            if (filterResponse.Any())
            {
                List<PeerFeedBackSessionGroupModel> itemToRemove = new List<PeerFeedBackSessionGroupModel>();
                foreach (var filter in filterResponse)
                {
                    var targets = allTarget.Where(x => x.OrgUnitId == filter.PeerFeedBackGroupId && x.PeerFeedBackSessionId == filter.PeerFeedBackSessionId).ToList();
                    int userCount = targets.Select(x => x.UserId).Distinct().Count();

                    var responsesGroup = allDataQuestionResponse.Where(x =>
                                                                        x.PeerFeedbackId == filter.PeerFeedBackId &&
                                                                        x.PeerFeedbackSessionId == filter.PeerFeedBackSessionId &&
                                                                        x.PeerFeedBackGroupId == filter.PeerFeedBackGroupId).ToList();
                    foreach (var user in targets)
                    {
                        var usersWithQuestion = responsesGroup
                           .Where(x => x.EvaluatorUserId == user.UserId)
                           .Select(x => new PeerFeedBackResponses
                           {
                               TargetUserId = x.TargetUserId,
                               PeerFeedbackQuestionId = x.PeerFeedbackQuestionId
                           }).Distinct().ToList();
                        var userGroupCount = usersWithQuestion.Select(x => x.TargetUserId).Distinct().Count();
                        var questionGroupCount = usersWithQuestion.GroupBy(p => p.TargetUserId, p => p.PeerFeedbackQuestionId, (targetUserId, questions) => new { userId = targetUserId, QuestionCount = questions.Distinct().Count() }).Select(x => x.QuestionCount).Sum();

                        if (userGroupCount != userCount || questionGroupCount != peerFeedBackQuestions.Count * userCount)
                        {
                            itemToRemove.Add(new PeerFeedBackSessionGroupModel
                            {
                                EvaluatorUserId = user.UserId,
                                PeerFeedBackGroupId = filter.PeerFeedBackGroupId,
                                PeerFeedBackSessionId = filter.PeerFeedBackSessionId,
                                PeerFeedBackId = filter.PeerFeedBackId
                            });
                        }
                    }
                }
                if (itemToRemove.Any())
                {
                    log.Info("Have item need to remove");
                    foreach (var item in itemToRemove)
                    {
                        allResponses.RemoveAll(x => x.PeerFeedbackId == item.PeerFeedBackId
                                                                   && x.PeerFeedBackGroupId == item.PeerFeedBackGroupId
                                                                   && x.EvaluatorUserId == item.EvaluatorUserId
                                                                   && x.PeerFeedbackSessionId == item.PeerFeedBackSessionId);
                    }
                }
            }
            log.Info("------ end GetCompleteEvaluationCsv ------");
            return allResponses;
        }
        #region UTIL

        private List<int> StringToIntList(string input)
        {
            var separator = new[] { ',' };
            var output = new List<int>();

            if (!string.IsNullOrWhiteSpace(input))
            {
                var tokens = input.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                // for each token try to parse into int, if fail, do not insert into output
                foreach (var token in tokens)
                    if (int.TryParse(token, out var outint))
                        output.Add(outint);
            }

            return output;
        }

        #endregion

        #region SERVICE

        private readonly ICourseService _courseService;
        private readonly IUserService _userService;
        private readonly ISemesterService _semesterService;
        private readonly IPeerFeedbackService _peerFeedbackService;
        private readonly ICategoryGroupService _categoryGroupService;
        private readonly IUserEnrollmentService _userEnrollmentService;
        private readonly IUserGroupService _userGroupService;
        private readonly ICourseCategoryService _courseCategoryService;
        private readonly ILoggingService _loggingService;
        private readonly IAuditService _auditService;
        private readonly IValenceService _valenceService;
        #endregion

        #region RATING Q & A
        [PeerFeedBackAuthorize(Role = "admin")]
        public ActionResult Rating()
        {
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PeerFeedBackAuthorize(Role = "admin")]
        public ActionResult RatingQuestionCreate([Bind(Include = "Name,DisplayOrder")] RatingQuestionModel model)
        {
            if(ModelState.IsValid)
            {
                var rationQuestion = new PeerFeedbackRatingQuestion
                {
                    Deleted = false,
                    Name = model.Name,
                    DisplayOrder = model.DisplayOrder
                };
                _peerFeedbackService.InsertPeerFeedbackRatingQuestion(rationQuestion);
                return Json("");
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, nameof(RatingQuestionModel));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PeerFeedBackAuthorize(Role = "admin")]
        public ActionResult RatingQuestionUpdate([Bind(Include = "Id,Name,DisplayOrder")] RatingQuestionModel model)
        {
            if (ModelState.IsValid)
            {
                var ratingQuestion = _peerFeedbackService.GetPeerFeedbackRatingQuestionById(model.Id.GetValueOrDefault());
                if (ratingQuestion == null) return Json("not found");
                ratingQuestion.Name = model.Name;
                ratingQuestion.DisplayOrder = model.DisplayOrder;

                _peerFeedbackService.UpdatePeerFeedbackRatingQuestion(ratingQuestion);
                return Json("");
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, nameof(RatingQuestionModel));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PeerFeedBackAuthorize(Role = "admin")]
        public ActionResult RatingQuestionDelete([Required] int id)
        {
            if(ModelState.IsValid)
            {
                var ratingQuestion = _peerFeedbackService.GetPeerFeedbackRatingQuestionById(id);
                if (ratingQuestion == null) return Json("not found");
                ratingQuestion.Deleted = true;
                _peerFeedbackService.UpdatePeerFeedbackRatingQuestion(ratingQuestion);

                return Json("");
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, nameof(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PeerFeedBackAuthorize(Role = "admin")]
        public ActionResult RatingQuestionList()
        {
            var results = _peerFeedbackService.GetListPeerFeedbackRatingQuestions();
            var dataSource = new DataSourceResult
            {
                Data = results,
                Total = results.Count
            };
            return Json(dataSource);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PeerFeedBackAuthorize(Role = "admin")]
        public ActionResult RatingAnswerCreate([Bind(Include = "Name")] RatingAnswerModel model)
        {
            if (ModelState.IsValid)
            {
                var rationOption = new PeerFeedbackRatingOption
                {
                    Deleted = false,
                    Name = model.Name
                };
                _peerFeedbackService.InsertPeerFeedbackRatingOption(rationOption);
                return Json("");
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, nameof(RatingAnswerModel));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PeerFeedBackAuthorize(Role = "admin")]
        public ActionResult RatingAnswerUpdate([Bind(Include = "Id,Name")] RatingAnswerModel model)
        {
            if (ModelState.IsValid)
            {
                var ratingOption = _peerFeedbackService.GetPeerFeedbackRatingOptionById(model.Id.GetValueOrDefault());
                if (ratingOption == null) return Json("not found");
                ratingOption.Name = model.Name;
                ratingOption.Deleted = false;
                _peerFeedbackService.UpdatePeerFeedbackRatingOption(ratingOption);
                return Json("");
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, nameof(RatingAnswerModel));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PeerFeedBackAuthorize(Role = "admin")]
        public ActionResult RatingAnswerDelete([Required] int id)
        {
            if (ModelState.IsValid)
            {
                var ratingOption = _peerFeedbackService.GetPeerFeedbackRatingOptionById(id);
                if (ratingOption == null) return Json("not found");
                ratingOption.Deleted = true;
                _peerFeedbackService.UpdatePeerFeedbackRatingOption(ratingOption);

                return Json("");
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, nameof(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PeerFeedBackAuthorize(Role = "admin")]
        public ActionResult RatingAnswerList()
        {
            var results = _peerFeedbackService.GetListPeerFeedbackRatingOptions();
            var dataSource = new DataSourceResult
            {
                Data = results,
                Total = results.Count
            };
            return Json(dataSource);
        }

        #endregion

        #region QUESTION
        [PeerFeedBackAuthorize(Role = "admin")]
        public ActionResult Question()
        {
            return PartialView();
        }

        [HttpPost]
        [PeerFeedBackAuthorize(Role = "admin")]
        public ActionResult CreateOrUpdateQuestion(int? questionId)
        {
            var model = new PeerFeedbackQuestionViewModel();
            if (questionId.HasValue && questionId.Value > 0)
            {
                var question = _peerFeedbackService.GetPeerFeedbackQuestionById(questionId.Value);
                model.Id = question.Id;
                model.Title = question.Title;
                model.Description = question.Description;
            }

            return PartialView("_CreateOrUpdateQuestion", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PeerFeedBackAuthorize(Role = "admin")]
        public ActionResult PeerFeedbackQuestionSave([Bind(Include = "Id,Title,Description")] PeerFeedbackQuestionViewModel model)
        {
            var question = new PeerFeedbackQuestion();
            if (ModelState.IsValid)
            {
                if (model.Id.HasValue && model.Id.Value > 0)
                {
                    question = _peerFeedbackService.GetPeerFeedbackQuestionById(model.Id.Value);
                    if (question == null)
                        return Json("not found");
                }

                question.Title = model.Title;
                question.Deleted = false;
                question.Description = model.Description;
                if (question.Id > 0)
                {
                    _peerFeedbackService.UpdatePeerFeedbackQuestion(question);
                }
                else
                {
                    _peerFeedbackService.InsertPeerFeedbackQuestion(question);
                    model.Id = question.Id;
                    return PartialView("_CreateOrUpdateQuestion", model);
                }
            }

            return Json("");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PeerFeedBackAuthorize(Role = "admin")]
        public ActionResult PeerFeedbackQuestionDelete(int id)
        {
            var question = _peerFeedbackService.GetPeerFeedbackQuestionById(id);
            if (question == null)
                return Json("not found");
            question.Deleted = true;
            _peerFeedbackService.UpdatePeerFeedbackQuestion(question);
            _peerFeedbackService.DeletePeerFeedbackQuestionRatingMapByQuestionId(id);
            return Json("");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PeerFeedBackAuthorize(Role = "admin")]
        public ActionResult PeerFeedbackQuestionList()
        {
            var results = _peerFeedbackService.GetListPeerFeedbackQuestions();
            var dataSource = new DataSourceResult
            {
                Data = results,
                Total = results.Count
            };
            return Json(dataSource);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PeerFeedBackAuthorize(Role = "admin")]
        public ActionResult PeerFeedbackQuestionRatingMapCreate(int questionId, int ratingQuestionId,
            ICollection<int> options)
        {
            var question = _peerFeedbackService.GetPeerFeedbackQuestionById(questionId);
            if (question == null) return Json("not found");
            _peerFeedbackService.DeletePeerFeedbackQuestionRatingMapByRatingQuestionId(questionId, ratingQuestionId);
            if (options != null)
                foreach (var optionId in options)
                {
                    var questionRatingMap = new PeerFeedbackQuestionRatingMap
                    {
                        QuestionId = questionId,
                        RatingOptionId = optionId,
                        RatingQuestionId = ratingQuestionId
                    };
                    _peerFeedbackService.InsertPeerFeedbackQuestionRatingMap(questionRatingMap);
                }

            return Json("");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PeerFeedBackAuthorize(Role = "admin")]
        public ActionResult QuestionRatingList(int questionId)
        {
            var results = _peerFeedbackService.GetListPeerFeedbackRatingQuestionsWithOptions(questionId);
            var dataSource = new DataSourceResult
            {
                Data = results,
                Total = results.Count
            };
            return Json(dataSource);
        }

        #endregion

        #region MANAGE PEERFEEDBACK
        [PeerFeedBackAuthorize(Role = "admin")]
        public ActionResult ManagePeerFeedback()
        {
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PeerFeedBackAuthorize(Role = "admin")]
        public ActionResult PeerFeedbackGetList()
        {
            var list = _peerFeedbackService.PeerFeedbackGetList();
            var model = list.Select(x => new ManagePeerFeedbackModel
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description
            }).ToList();
            var dataSource = new DataSourceResult
            {
                Data = model,
                Total = model.Count
            };
            return Json(dataSource);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PeerFeedBackAuthorize(Role = "admin")]
        public ActionResult PeerFeedbackDelete(int peerFeedbackId)
        {
            log.Info("**************** START PeerFeedbackDelete ****************");
            log.Info($"PeerFeedbackGetById peerFeedbackId = {peerFeedbackId}");
            var peerFeedBack = _peerFeedbackService.PeerFeedbackGetById(peerFeedbackId);
            log.Info($"PeerFeedbackGetById Ok response = {JsonConvert.SerializeObject(peerFeedBack)}");
            if (peerFeedBack == null)
            {
                log.Warn($"peerFeedBack == null, peerFeedbackId = {peerFeedbackId} not found.");
                return Json("Error");
            }
            log.Info($"PeerFeedbackDelete with paramters peerFeedbackId = {peerFeedbackId}, UserInfo.UserId = {UserInfo.UserId}");
            _peerFeedbackService.PeerFeedbackDelete(peerFeedBack.Id, UserInfo.UserId);
            log.Info($"PeerFeedbackDelete OK.");
            log.Info("**************** END PeerFeedbackDelete ****************");
            return Json("");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PeerFeedBackAuthorize(Role = "admin")]
        public ActionResult PeerFeedbackCreateOrUpdate(int? peerFeedbackId)
        {
            log.Info("**************** START PeerFeedbackCreateOrUpdate ****************");
            var model = new ManagePeerFeedbackModel();
            if (peerFeedbackId > 0)
            {
                log.Info($"PeerFeedbackGetById peerFeedbackId = {peerFeedbackId.Value}");
                var peerFeedBack = _peerFeedbackService.PeerFeedbackGetById(peerFeedbackId.Value);
                log.Info($"PeerFeedbackGetById Ok response = {JsonConvert.SerializeObject(peerFeedBack)}");
                if (peerFeedBack == null)
                {
                    log.Warn($"peerFeedBack == null, peerFeedbackId = {peerFeedbackId} not found.");
                    return RedirectToAction("Index", "Error");
                }
                model.Id = peerFeedBack.Id;
                model.Name = peerFeedBack.Name;
                model.Description = peerFeedBack.Description;
            }
            log.Info("**************** END PeerFeedbackCreateOrUpdate ****************");
            return PartialView("_PeerFeedback_Create", model);
        }
        [PeerFeedBackAuthorize(Role = "admin")]
        public ActionResult PeerFeedbackStep(int? peerFeedbackId, int step)
        {
            log.Info("**************** PeerFeedbackStep ****************");
            var model = new ManagePeerFeedbackModel();
            if (peerFeedbackId > 0)
            {
                log.Info($"PeerFeedbackGetById peerFeedbackId = {peerFeedbackId.Value}");
                var peerFeedBack = _peerFeedbackService.PeerFeedbackGetById(peerFeedbackId.Value);
                log.Info($"PeerFeedbackGetById Ok response = {JsonConvert.SerializeObject(peerFeedBack)}");
                if (peerFeedBack == null)
                {
                    log.Warn($"peerFeedBack == null, peerFeedbackId = {peerFeedbackId} not found.");
                    return RedirectToAction("Index", "Error");
                }
                model.Id = peerFeedBack.Id;
                model.Name = peerFeedBack.Name;
                model.Description = peerFeedBack.Description;
            }

            switch (step)
            {
                case 1:
                    log.Info($"STEP 1 VIEW _PeerFeedback_CreateOrUpdate_Questions");
                    return PartialView("_PeerFeedback_CreateOrUpdate_Questions", model);
                case 2:
                    log.Info($"STEP 2 VIEW _PeerFeedback_CreateOrUpdate_EvaluatorAndTarget");
                    return PartialView("_PeerFeedback_CreateOrUpdate_EvaluatorAndTarget", model);
                case 3:
                    log.Info($"STEP 3 VIEW _PeerFeedback_CreateOrUpdate_Sessions");
                    return PartialView("_PeerFeedback_CreateOrUpdate_Sessions", model);
                default:
                    log.Info($"STEP 4 VIEW _PeerFeedback_CreateOrUpdate_Sessions");
                    return PartialView("_PeerFeedback_CreateOrUpdate_Properties", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PeerFeedBackAuthorize(Role = "admin")]
        public ActionResult PeerFeedbackSave([Bind(Include = "Id,Name,Description")] ManagePeerFeedbackModel model)
        {
            log.Info("**************** START PeerFeedbackSave ****************");
            var peerFeedback = new PeerFeedback();
            if (ModelState.IsValid)
            {
                log.Info("Check peerfeedBackId");
                if (model.Id > 0)
                {
                    log.Info($"update peerfeedBackId = {model.Id}");
                    log.Info($"PeerFeedbackGetById peerFeedbackId = {model.Id}");
                    peerFeedback = _peerFeedbackService.PeerFeedbackGetById(model.Id.Value);
                    log.Info($"PeerFeedbackGetById Ok response = {JsonConvert.SerializeObject(peerFeedback)}");
                    if (peerFeedback == null)
                    {
                        log.Warn($"peerFeedBack == null, peerFeedbackId = {model.Id} not found.");
                        return Json("Not found");
                    }
                    peerFeedback.LastUpdatedBy = UserInfo.UserId;
                    peerFeedback.LastUpdatedTime = DateTime.UtcNow;
                }
                else
                {
                    log.Info($"insert peerfeedBackId data");
                    peerFeedback.TypeId = (int)PeerFeedbackType.StudentsEvaluateOwnGroupMembers;
                    peerFeedback.IsDeleted = false;
                    peerFeedback.CreatedTime = DateTime.UtcNow;
                }

                peerFeedback.Description = model.Description;
                peerFeedback.Name = model.Name;
                if (peerFeedback.Id > 0)
                {
                    log.Info($"PeerFeedbackUpdate with paramters = {JsonConvert.SerializeObject(peerFeedback)}");
                    _peerFeedbackService.PeerFeedbackUpdate(peerFeedback);
                    log.Info($"PeerFeedbackUpdate OK.");
                }
                else
                {
                    log.Info("------ start PeerFeedbackInsert ------");
                    log.Info($"PeerFeedbackInsert with paramters = {JsonConvert.SerializeObject(peerFeedback)}");
                    _peerFeedbackService.PeerFeedbackInsert(peerFeedback);
                    log.Info($"PeerFeedbackInsert OK.");
                    log.Info("------ end PeerFeedbackInsert ------");

                    log.Info("------ start PeerFeedbackQuestionMapDeleteByPeerFeedBackId ------");
                    log.Info($"PeerFeedbackQuestionMapDeleteByPeerFeedBackId peerFeedbackId = {peerFeedback.Id}");
                    _peerFeedbackService.PeerFeedbackQuestionMapDeleteByPeerFeedBackId(peerFeedback.Id);
                    log.Info($"PeerFeedbackQuestionMapDeleteByPeerFeedBackId OK.");
                    log.Info("------ end PeerFeedbackQuestionMapDeleteByPeerFeedBackId ------");

                    var questions = _peerFeedbackService.GetListPeerFeedbackQuestions();
                    log.Info($"GetListPeerFeedbackQuestions OK response = {JsonConvert.SerializeObject(questions)}");
                    if (questions != null)
                        foreach (var question in questions)
                        {
                            var peerFeedbackQuestionMap = new PeerFeedbackQuestionMap
                            {
                                PeerFeedbackId = peerFeedback.Id,
                                PeerFeedbackQuestionId = question.Id
                            };
                            log.Info("------ start PeerFeedbackQuestionMapInsert ------");
                            _peerFeedbackService.PeerFeedbackQuestionMapInsert(peerFeedbackQuestionMap);
                            log.Info("------ end PeerFeedbackQuestionMapInsert ------");
                        }
                    }
                }
            log.Info("**************** END PeerFeedbackSave ****************");
            return Json(peerFeedback);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PeerFeedBackAuthorize(Role = "admin")]
        public ActionResult PeerFeedbackQuestionMapList(int peerFeedbackId)
        {
            var results = _peerFeedbackService.PeerFeedbackQuestionMapList(peerFeedbackId);
            var dataSource = new DataSourceResult
            {
                Data = results,
                Total = results.Count
            };
            return Json(dataSource);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PeerFeedBackAuthorize(Role = "admin")]
        public ActionResult PeerFeedbackQuestionMapCreate(int peerFeedbackId, ICollection<int> questions)
        {
            var peerFeedback = _peerFeedbackService.PeerFeedbackGetById(peerFeedbackId);
            if (peerFeedback == null) return Json("not found");
            _peerFeedbackService.PeerFeedbackQuestionMapDeleteByPeerFeedBackId(peerFeedbackId);
            if (questions != null)
                foreach (var questionId in questions)
                {
                    var peerFeedbackQuestionMap = new PeerFeedbackQuestionMap
                    {
                        PeerFeedbackId = peerFeedbackId,
                        PeerFeedbackQuestionId = questionId
                    };
                    _peerFeedbackService.PeerFeedbackQuestionMapInsert(peerFeedbackQuestionMap);
                }

            return Json("");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PeerFeedBackAuthorize(Role = "admin")]
        public ActionResult PeerFeedbackQuestionMapDelete(int id)
        {
            var map = _peerFeedbackService.PeerFeedbackQuestionMapGetById(id);
            if (map == null)
                return Json("not found");
            _peerFeedbackService.PeerFeedbackQuestionMapDeleteById(map);
            return Json("");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrUpdateEvaluatorAndTarget(int peerFeedbackId, int peerFeedBackSessionId,
            int? peerFeedBackPairingId)
        {
            var peerFeedback = _peerFeedbackService.PeerFeedbackGetById(peerFeedbackId);
            var session = _peerFeedbackService.PeerFeedbackSessionsGetById(peerFeedBackSessionId);
            var lstTarget = _peerFeedbackService.PeerFeedbackTargetGetTargets(peerFeedbackId, session.CourseOfferingCode.Split(',').ToList())
                .OrderBy(x => x.Name);
            var targets = peerFeedBackPairingId > 0
                ? _peerFeedbackService.PeerFeedbackTargetsGetByPairingId(peerFeedBackPairingId.Value)
                : null;
            var sbEvaluator = new StringBuilder();
            var isCourseGroup = false;
            int? currentCourseGroup = null;
            var sbTarget = new StringBuilder();
            if (lstTarget.Any())
            {
                // special handling for student evaluate own group member. user may only edit by group
                if (peerFeedback.TypeId == (int)PeerFeedbackType.StudentsEvaluateOwnGroupMembers)
                    if (targets?.Count > 0)
                        targets = targets.Select(x => new ItemDto
                        {
                            Id = x.GroupId
                        }).Distinct().ToList();

                var courseGroups = lstTarget.GroupBy(x => x.GroupId);
                // Display course group if any
                if (courseGroups.Any() && courseGroups.First().First().GroupId > 0)
                {
                    // find selected course group for edit
                    var evaluationPairings =
                        _peerFeedbackService.PeerFeedbackPairingsGetByPeerFeedBackId(peerFeedbackId);
                    if (peerFeedBackPairingId > 0)
                    {
                        var targetsPairing =
                            _peerFeedbackService.PeerFeedbackTargetsGetByPairingId(peerFeedBackPairingId.Value);
                        if (targetsPairing.Any())
                        {
                            // special handling for student evaluate own group member. user may only edit by group
                            if (peerFeedback.TypeId == (int)PeerFeedbackType.StudentsEvaluateOwnGroupMembers)
                                currentCourseGroup = lstTarget
                                    .FirstOrDefault(x => x.Id == targetsPairing.First().GroupId)?.GroupId;
                            else
                                currentCourseGroup = lstTarget.FirstOrDefault(x => x.Id == targetsPairing.First().Id)
                                    ?.GroupId;
                        }
                    }

                    isCourseGroup = true;
                    foreach (var group in courseGroups)
                    {
                        sbTarget.Append("{");
                        sbTarget.Append("\"GroupId\":").Append($"{group.First().GroupId}").Append(",");
                        sbTarget.Append("\"GroupName\":")
                            .Append(
                                $"{System.Web.Helpers.Json.Encode(HttpUtility.HtmlEncode(group.First().DisplayName))}")
                            .Append(",");
                        sbTarget.Append("\"Target\": [");
                        foreach (var item in group)
                        {
                            // check if target is checked
                            var result = targets == null ? 0 :
                                targets.FirstOrDefault(x => x.Id == item.Id) == null ? 0 : 1;

                            sbTarget.Append("{");
                            sbTarget.Append($"\"{nameof(item.Id)}\":").Append($"{item.Id}").Append(",");
                            sbTarget.Append($"\"{nameof(item.Name)}\":")
                                .Append($"{System.Web.Helpers.Json.Encode(HttpUtility.HtmlEncode(item.Name))}")
                                .Append(",");
                            sbTarget.Append("\"IsChecked\":").Append($"{result}");
                            sbTarget.Append("}").Append(",");
                        }

                        sbTarget.Length--;
                        sbTarget.Append("]");
                        sbTarget.Append("}").Append(",");
                    }
                }
                else
                {
                    foreach (var item in lstTarget)
                    {
                        // check if target is checked
                        var result = targets == null ? 0 : targets.FirstOrDefault(x => x.Id == item.Id) == null ? 0 : 1;

                        sbTarget.Append("{");
                        sbTarget.Append($"\"{nameof(item.Id)}\":").Append($"{item.Id}").Append(",");
                        sbTarget.Append($"\"{nameof(item.Name)}\":")
                            .Append($"{System.Web.Helpers.Json.Encode(HttpUtility.HtmlEncode(item.Name))}").Append(",");
                        sbTarget.Append($"\"{nameof(item.MemberGroup)}\":")
                            .Append($"{System.Web.Helpers.Json.Encode(HttpUtility.HtmlEncode(item.MemberGroup))}")
                            .Append(",");
                        sbTarget.Append("\"IsChecked\":").Append($"{result}");
                        sbTarget.Append("}").Append(",");
                    }
                }

                sbTarget.Length--;
            }

            //var model = new EvaluatorTargetModel
            //{
            //    Evaluator = sbEvaluator.ToString(),
            //    Target = sbTarget.ToString(),
            //    PeerFeedBackId = peerFeedbackId,
            //    PeerFeedBackPairingId = peerFeedBackPairingId.GetValueOrDefault(),
            //    TypeId = peerFeedBackTypeId,
            //    IsCourseGroup = isCourseGroup,
            //    CurrentCourseGroup = currentCourseGroup ?? 0,
            //    PeerFeedBackSessionId = peerFeedBackSessionId
            //};
            return PartialView("_Evaluator_Target", null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PeerFeedBackAuthorize(Role = "admin")]
        public ActionResult PeerFeedbackSessionAddOrUpdate(int peerFeedbackId, int? peerFeedbackSessionId)
        {
            log.Info("**************** START PeerFeedbackSessionAddOrUpdate ****************");
            log.Info($"PeerFeedbackGetById peerFeedbackId = {peerFeedbackId}");
            var peerFeedback = _peerFeedbackService.PeerFeedbackGetById(peerFeedbackId);
            log.Info($"PeerFeedbackGetById Ok response = {JsonConvert.SerializeObject(peerFeedback)}");
            // Validation
            if (peerFeedback == null)
            {
                log.Warn($"peerFeedBack == null, peerFeedbackId = {peerFeedbackId} not found.");
                return RedirectToAction("Index", "Error");
            }
            var model = new PeerFeedbackSessionAddOrEditViewModel
            {
                Session = new PeerFeedbackSessionViewModel
                {
                    PeerFeedbackId = peerFeedbackId,
                    Id = 0,
                    StartTotalMilliseconds = DateTime.UtcNow.TotalMilliseconds(),
                    EndTotalMilliseconds = DateTime.UtcNow.TotalMilliseconds()
                }
            };
            log.Info($"check peerFeedbackSessionId = {peerFeedbackSessionId}");
            if (peerFeedbackSessionId > 0)
            {
                log.Info($"PeerFeedbackSessionsGetById peerFeedbackSessionId = {peerFeedbackSessionId}");
                var session = _peerFeedbackService.PeerFeedbackSessionsGetById(peerFeedbackSessionId.Value);
                log.Info($"PeerFeedbackSessionsGetById Ok response = {JsonConvert.SerializeObject(session)}");
                if (session == null)
                {
                    log.Warn($"session == null, peerFeedbackSessionId = {peerFeedbackSessionId} not found.");
                    return RedirectToAction("Index", "Error");
                }
                model.Session = new PeerFeedbackSessionViewModel
                {
                    Id = session.Id,
                    PeerFeedbackId = session.PeerFeedbackId,
                    StartTotalMilliseconds = session.EntryStartTime.TotalMilliseconds(),
                    EndTotalMilliseconds = session.EntryCloseTime.TotalMilliseconds(),
                    Label = session.Label,
                    Strm = session.Strm,
                    CourseOfferingCode = string.IsNullOrEmpty(session.CourseOfferingCode) ? new List<string>() : session.CourseOfferingCode.Split(',').ToList()
                };
            }
            log.Info("**************** END PeerFeedbackSessionAddOrUpdate ****************");
            return PartialView("_PeerFeedback_CreateOrUpdate_Sessions_AddOrUpdate", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PeerFeedBackAuthorize(Role = "admin")]
        public ActionResult PeerFeedbackSessionDelete(int peerFeedbackSessionId)
        {
            log.Info("**************** START PeerFeedbackSessionDelete ****************");
            log.Info($"PeerFeedbackSessionsGetById peerFeedbackSessionId = {peerFeedbackSessionId}");
            var session = _peerFeedbackService.PeerFeedbackSessionsGetById(peerFeedbackSessionId);
            log.Info($"PeerFeedbackSessionsGetById Ok response = {JsonConvert.SerializeObject(session)}");
            if (session != null)
            {
                session.IsDeleted = true;
                session.LastUpdatedBy = UserInfo.UserId;
                session.LastUpdatedTime = DateTime.UtcNow;
                log.Info("------ start PeerFeedbackSessionsUpdate ------");
                log.Info($"parameters = {JsonConvert.SerializeObject(session)}");
                _peerFeedbackService.PeerFeedbackSessionsUpdate(session);
                log.Info("------ end PeerFeedbackSessionsUpdate ------");
            }
            log.Info("**************** END PeerFeedbackSessionDelete ****************");
            return Json("");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PeerFeedBackAuthorize(Role = "admin")]
        public ActionResult PeerFeedbackSessionGetById(int peerFeedbackSessionId)
        {
            var session = _peerFeedbackService.PeerFeedbackSessionsGetById(peerFeedbackSessionId);
            var model = new PeerFeedbackSessionViewModel
            {
                Id = session.Id,
                PeerFeedbackId = session.PeerFeedbackId,
                StartTotalMilliseconds = session.EntryStartTime.TotalMilliseconds(),
                EndTotalMilliseconds = session.EntryCloseTime.TotalMilliseconds(),
                Label = session.Label,
                Strm = session.Strm
            };
            return Json(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PeerFeedBackAuthorize(Role = "admin")]
        public async Task<ActionResult> PeerFeedbackSessionList(int peerFeedbackId)
        {
            var sessions = _peerFeedbackService.PeerFeedbackSessionsGetByPeerFeedbackId(peerFeedbackId);
            var terms = await _peerFeedbackService.PeerFeedbackGetDefaultSelectedStrm(sessions.Select(x => x.Strm).ToList());
            var model = sessions.Select(x => new PeerFeedbackSessionViewModel
            {
                Id = x.Id,
                PeerFeedbackId = x.PeerFeedbackId,
                StartTotalMilliseconds = x.EntryStartTime.TotalMilliseconds(),
                EndTotalMilliseconds = x.EntryCloseTime.TotalMilliseconds(),
                Label = x.Label,
                Strm = x.Strm,
                Term = terms.First(a => a.Value == x.Strm).Text
            }).ToList();
            return Json(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PeerFeedBackAuthorize(Role = "admin")]
        public ActionResult PeerFeedbackSessionSave([Bind(Include = "Id,PeerFeedbackId,EntryStartTime,EntryCloseTime,Label,Strm,CourseOfferingCode")] PeerFeedbackSessionViewModel model)
        {
            log.Info("**************** START PeerFeedbackResponseSave ****************");
            log.Info($" Get PeerFeedBack By Id = {model.PeerFeedbackId.Value}");
            var peerFeedback = _peerFeedbackService.PeerFeedbackGetById(model.PeerFeedbackId.Value);
            log.Info($" Get PeerFeedBack Ok response = {JsonConvert.SerializeObject(peerFeedback)}");
            if (peerFeedback == null)
            {
                log.Warn($"peerFeedback == null; PeerFeedbackId = {model.PeerFeedbackId.Value} not found");
                return RedirectToAction("Index", "Error");
            }
            var session = new PeerFeedbackSessions();
            log.Info($"check PeerFeedBackSessionId = {model.Id} for insert or update");
            if (model.Id > 0)
            {
                log.Info($" Get PeerFeedBackSession By Id = {model.Id.Value}");
                session = _peerFeedbackService.PeerFeedbackSessionsGetById(model.Id.Value);
                log.Info($" Get PeerFeedBackSession Ok response = {JsonConvert.SerializeObject(session)}");
                if (session == null)
                {
                    log.Warn($"session == null; PeerFeedbackSessionId = {model.Id.Value} not found");
                    return RedirectToAction("Index", "Error");
                }
            }
            if (model.EntryStartTime >= model.EntryCloseTime)
            {
                ModelState.AddModelError("", "End Date cannot be before Start Date!");
            }
            if (model.EntryCloseTime < DateTime.UtcNow)
            {
                ModelState.AddModelError("", "End Date cannot be before today’s date!");
            }
            log.Info($"ModelState.IsValid = {ModelState.IsValid}");
            if (ModelState.IsValid)
            {
                session.PeerFeedbackId = model.PeerFeedbackId.Value;
                session.EntryStartTime = model.EntryStartTime;
                session.EntryCloseTime = model.EntryCloseTime;
                session.Label = model.Label;
                session.Strm = model.Strm;
                session.LastUpdatedBy = UserInfo.UserId;
                session.LastUpdatedTime = DateTime.UtcNow;
                session.CourseOfferingCode = string.Join(",", model.CourseOfferingCode);
                log.Info("Check session timeframe");
                bool isExists = _peerFeedbackService.PeerFeedbackSessionsCheckByCondition(session);
                if (isExists)
                {
                    string msg = "A session with timeframe already exists. Please check again.";
                    log.Info(msg);
                    ModelState.AddModelError(string.Empty, msg);
                    var errorList = ModelState.Values.SelectMany(m => m.Errors)
                                     .Select(e => e.ErrorMessage)
                                     .ToList();
                    return Json(errorList);
                }
                else
                {
                    if (model.Id > 0)
                    {
                        log.Info("------ start PeerFeedbackSessionsUpdate ------");
                        log.Info($"update session with param = {JsonConvert.SerializeObject(session)}");
                        _peerFeedbackService.PeerFeedbackSessionsUpdate(session);
                        log.Info($"update session OK");
                        log.Info("------ end PeerFeedbackSessionsUpdate ------");
                    }
                    else
                    {
                        log.Info("------ start PeerFeedbackSessionsInsert ------");
                        log.Info($"insert session with param = {JsonConvert.SerializeObject(session)}");
                        _peerFeedbackService.PeerFeedbackSessionsInsert(session);
                        log.Info($"insert session OK");
                        log.Info("------ end PeerFeedbackSessionsInsert ------");
                        // generate pairings
                        log.Info("------ start GeneratePairings ------");
                        var userId = UserInfo.UserId;
                        HostingEnvironment.QueueBackgroundWorkItem(async ct =>
                        {
                            await _peerFeedbackService.GeneratePairings(session, userId);
                        });
                        log.Info("------ end GeneratePairings ------");
                    }
                }
                return Json(string.Empty);
            }
            log.Info("**************** END PeerFeedbackResponseSave ****************");
            return Json(ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage).ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PeerFeedBackAuthorize(Role = "admin")]
        public ActionResult PeerFeedBackTargetsGetTargetPairingList(int peerFeedbackId)
        {
            var peerFeedBack = _peerFeedbackService.PeerFeedbackGetById(peerFeedbackId);
            var result = new List<PeerFeedBackSessionsPairingModel>();

            var sessions = _peerFeedbackService.PeerFeedbackSessionsGetByPeerFeedBackId(peerFeedbackId);
            foreach (var session in sessions)
            {
                var resultTargets = new List<Item>();
                var sessionModel = new PeerFeedbackSessionViewModel
                {
                    Id = session.Id,
                    PeerFeedbackId = session.PeerFeedbackId,
                    EntryStartTime = session.EntryStartTime,
                    StartTotalMilliseconds = session.EntryStartTime.TotalMilliseconds(),
                    EntryCloseTime = session.EntryCloseTime,
                    EndTotalMilliseconds = session.EntryCloseTime.TotalMilliseconds(),
                    Label = session.Label
                };
                var sessionPairings = _peerFeedbackService.PeerFeedBackPairingSessionsGetBySessionId(session.Id);
                if (sessionPairings != null && sessionPairings.Any())
                {
                    foreach (var sessionPairing in sessionPairings)
                    {
                        var pairing = _peerFeedbackService.PeerFeedbackPairingsGetById(sessionPairing.PeerFeedBackPairingId);
                        var targets = _peerFeedbackService.PeerFeedbackTargetsGetByPairingId(pairing.Id);
                        // special handling for student evaluate own group member.
                        // data is stored as singulre target, but selection must be in done in group mode. 
                        // hence we are returning group data together with target info
                        if (peerFeedBack.TypeId == (int)PeerFeedbackType.StudentsEvaluateOwnGroupMembers)
                        {
                            var groupDictionary = targets.GroupBy(x => x.GroupId);
                            var targetGroups = new List<Item>();
                            foreach (var group in groupDictionary)
                            {
                                var targetGroup = new Item
                                {
                                    Id = group.Key,
                                    Name = group.First().Group,
                                    Description = string.Join(", ", group.Select(x => x.Name).ToList()),
                                    Type = 1 // group
                                };
                                targetGroups.Add(targetGroup);
                            }

                            resultTargets = targetGroups.OrderBy(x => x.Name).ToList();
                            sessionModel.Targets.AddRange(resultTargets);
                        }
                    }
                }

                var model = new PeerFeedBackSessionsPairingModel
                {
                    Session = sessionModel
                };
                result.Add(model);
            }

            ViewBag.TypeId = peerFeedBack.TypeId;
            var sessionsPairingModel = new PeerFeedBackSessionsPairingListModel
            {
                PeerFeedBackId = peerFeedbackId,
                SessionPairings = result
            };
            return PartialView("_PeerFeedback_CreateOrUpdate_EvaluatorAndTarget_PairingList", sessionsPairingModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PeerFeedBackAuthorize(Role = "admin")]
        public ActionResult PeerFeedBackPairingSave([Bind(Include = "PeerFeedBackId,PeerFeedBackSessionId,PeerFeedBackPairingId,Target")] PeerFeedBackPairingModel evaluationPairingModel)
        {
            // VALIDATE: param must exist
            if (evaluationPairingModel == null) return RedirectToAction("Index", "Error");
            var lstTargetId = StringToIntList(evaluationPairingModel?.Target?.FirstOrDefault());

            // VALIDATE: valid evaluationId in param
            var evaluation = _peerFeedbackService.PeerFeedbackGetById(evaluationPairingModel.PeerFeedBackId);
            if (evaluation == null) return RedirectToAction("Index", "Error");

            if (evaluation.TypeId == (int)PeerFeedbackType.StudentsEvaluateOwnGroupMembers)
            {
                // VALIDATE: must have group selected
                if (lstTargetId.Count == 0)
                    return Json(new
                    {
                        StatusCode = (int)StatusCode.AlreadyExists,
                        Message = "Please select at least 1 group in a pairing."
                    });

                // Find target in other pairing
                var evaluationPairingListEntity =
                    _peerFeedbackService.PeerFeedbackPairingsGetByPeerFeedBackId(evaluation.Id);
                var targetInPairing = new List<ItemDto>();
                var lstResult = new List<ItemDto>(); // groupIds that exist in other pairing
                foreach (var evaluationPairing in evaluationPairingListEntity)
                {
                    var targets = _peerFeedbackService.PeerFeedbackTargetsGetByPairingId(evaluationPairing.Id);

                    if (evaluationPairing.Id == evaluationPairingModel.PeerFeedBackPairingId)
                        targetInPairing.AddRange(targets);
                    else
                        lstResult.AddRange(targets);
                    }

                // Pairing edit, need to add existing to exception
                var lstExcept = new List<int>();
                if (evaluationPairingModel.PeerFeedBackPairingId > 0)
                {
                    var lstCurrentTarget =
                        _peerFeedbackService.PeerFeedbackTargetsGetByPairingId(evaluationPairingModel
                        .PeerFeedBackPairingId);
                    // lstExcept are group that is going to be removed from pairing
                    lstExcept = lstCurrentTarget.Where(x => !lstTargetId.Contains(x.GroupId)).Select(x => x.GroupId)
                        .ToList();

                    // following line should not be needed. since lstResult should not have any evaluator in evaluationPairingModel.EvaluationPairingId
                    lstResult = lstResult
                        .Where(x => x.EvaluationPairingId != evaluationPairingModel.PeerFeedBackPairingId).ToList();
                }

                // VALIDATE: one evaluator can only appear 1x in pairing, 1 group or 2 groups does not matter.
                // if have duplicate means evaluator appears more than 1 time in pairing
                var evaluatorsInPairing = _userService.GetUserByGroupId(lstTargetId);
                var duplicateUsers = evaluatorsInPairing.GroupBy(usr => usr.Id)
                    .Where(grp => grp.Count() > 1)
                    .SelectMany(grp => grp.Select(usr => usr.DisplayName))
                    .Distinct()
                    .ToList();
                if (duplicateUsers.Count() > 0)
                    return Json(new
                    {
                        StatusCode = (int)StatusCode.AlreadyExists,
                        Message =
                            $"This target [<strong><i>{string.Join(", ", duplicateUsers)}</i></strong>] appears more than once in this pairing. Please separate into 2 pairings."
                    });

                // VALIDATE: same group can only be dded 1time
                var result = lstResult
                    .FirstOrDefault(x =>
                        lstTargetId.Contains(x
                            .GroupId) // new groupId (lstTarget) can not exist in other pairing (lstResult)
                        || (lstExcept.Count > 0 &&
                            lstExcept.Contains(x
                                .GroupId))); // incase of dirty data, we allow removal of groupId that exist in 2 pairing
                if (result != null)
                    return Json(new
                    {
                        StatusCode = (int)StatusCode.AlreadyExists,
                        Message =
                            $"This target [<strong><i>{result.Group}</i></strong>] is already set up in another pairing. Please do not duplicate the target."
                    });
            }

            var evaluationPairingEntity = _peerFeedbackService.PeerFeedbackPairingsSave(
                evaluationPairingModel.PeerFeedBackId, evaluationPairingModel.PeerFeedBackSessionId,
                evaluationPairingModel.PeerFeedBackPairingId, UserInfo.UserId, null, lstTargetId);

            return Json(evaluationPairingEntity);
        }

        #endregion

        #region TEMPLATE

        public List<PeerFeedbackQuestionModel> PeerFeedbackQuestionTemplate(int peerFeedBackId)
        {
            var lstQuestion = peerFeedBackId > 0
                ? _peerFeedbackService.PeerFeedbackQuestionMapList(peerFeedBackId)
                : _peerFeedbackService.GetListPeerFeedbackQuestions();
            var questions = lstQuestion.Select(x => new PeerFeedbackQuestionModel
            {
                Title = x.Title,
                Id = x.Id,
                Description = x.Description,
                RatingQuestion = _peerFeedbackService.GetListPeerFeedbackRatingQuestionsByQuestionId(x.Id).Select(
                    (a, index) =>
                    {
                        var model = new PeerFeedbackRatingQuestionModel
                        {
                            Name = a.Name,
                            Id = a.Id,
                            Selected = index == 0
                        };
                        model.RatingOptions = _peerFeedbackService
                            .GetListPeerFeedbackRatingOptionsByQuestionId(x.Id, a.Id)
                            .Select(b => new PeerFeedbackRatingOptionModel
                            {
                                OptionName = b.OptionName,
                                RatingOptionId = b.RatingOptionId,
                                RatingQuestionId = b.RatingQuestionId,
                                QuestionId = b.QuestionId,
                                Display = model.Selected
                            }).ToList();
                        return model;
                    }).ToList()
            }).ToList();
            return questions;
        }
        #endregion

        #region SEED DATA
        [PeerFeedBackAuthorize(Role = "admin")]
        public ActionResult SeedData()
        {
            var seedData = _peerFeedbackService.GetSeedData();
            var seedDataQuery = SeedDataHelper.GenerateSeedQuery(seedData);
            if (!string.IsNullOrEmpty(seedDataQuery))
            {
                var filePath = @"D:\Workspaces\SMU\Github\eLearnApps\SourceCode\LMSTools_db_scripts\2022\PeerFeedback\1_1_init_table_seed_data.sql";
                System.IO.File.WriteAllText(filePath, seedDataQuery);
            }
            var json = JsonConvert.SerializeObject(seedData);
            var seedFile = $"{Server.MapPath(Constants.StaticFilesFolder)}/seed-data-file-{DateTime.Now.ToString("dd-MM-yyyy HH-mm")}.json";
            System.IO.File.WriteAllText(seedFile, json);
            return Json(string.Empty);
        }
        #endregion

        #region SESSION
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PeerFeedBackAuthorize(Role = "admin")]
        public ActionResult GetPaticipatingCourse([Required] string term)
        {
            log.Info("**************** START GetPaticipatingCourse ****************");
            if (string.IsNullOrEmpty(term))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "term is not valid.");
            }
            log.Info("------ start PeerFeedbackSessionsGetCourseByIdAndTerm ------");
            var result = _peerFeedbackService.PeerFeedbackSessionsGetCourseByIdAndTerm(term);
            log.Info("**************** END GetPaticipatingCourse ****************");
            return Json(result);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PeerFeedBackAuthorize(Role = "admin")]
        public ActionResult ExportCourseByTermToCsv([Required] string term, string timeZone = "Asia/Singapore")
        {
            log.Info("**************** START ExportCourseByTermToCsv ****************");
            if (string.IsNullOrEmpty(term))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "parameters is not valid.");
            }
            var terms = _peerFeedbackService.PeerFeedbackGetWhitelistedTerm();
            var selectedTerm = terms.FirstOrDefault(x => x.Value == term);
            log.Info($"selectedTerm {selectedTerm.ToJson()}");
            if (selectedTerm == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "term is not found.");
            }
            var codes = selectedTerm.Items.Select(x => x.Value).ToList();
            var result = new List<ExportParticipatingCourseCsvModel>();
            if (codes != null && codes.Any())
            {
                log.Info("------ start GetGroupReadinessData ------");
                log.Info($"codes {codes.ToJson()}");
                var data = _peerFeedbackService.GetGroupReadinessData(codes);
                log.Info($"GetGroupReadinessData result {data.ToJson()}");
                result = data.Select(x => new ExportParticipatingCourseCsvModel
                {
                    AcadGroup = x.AcadGroup,
                    CourseCode = x.CourseCode,
                    CourseName = x.CourseName,
                    Duplicates = x.Duplicates,
                    GroupCount = x.GroupCount,
                    GroupNames = x.GroupNames,
                    Instructor = x.Instructor,
                    StudentCount = x.StudentCount,
                    CreatedInPSFS = x.CreatedInPSFS,
                    HasMultipleGroup = x.MultipleCategoryGroups,
                    UnassignedStudentCount = x.UnassignedStudentCount
                }).ToList();
            }
            string csv = result == null || !result.Any() ? "No record(s) found" : result.ToCsv();
            log.Info("**************** END ExportCourseByTermToCsv ****************");
            var tz = TZConvert.IanaToWindows(timeZone);
            var clientTz = TimeZoneInfo.FindSystemTimeZoneById(tz);
            var currentTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, clientTz);
            var fileName = $"GroupReadinessReport_{currentTime.ToString("yyyyMMdd")}_{currentTime.ToString("HHmmss")}.csv";
            log.Info($"export file name {fileName}");
            return File(new UTF8Encoding().GetBytes(csv), "text/csv", fileName);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PeerFeedBackAuthorize(Role = "admin")]
        public ActionResult GetGroupReadiness([Required] string term, List<string> codes)
        {
            log.Info("**************** START GetGroupReadiness ****************");
            if (string.IsNullOrEmpty(term))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "term is not valid.");
            }
            log.Info("------ start PeerFeedbackSessionsCategoryGroup ------");
            var result = _peerFeedbackService.PeerFeedbackSessionsGetCourseInfoPreview(codes, new List<string> { term });
            log.Info($"PeerFeedbackSessionsCategoryGroup result {result.ToJson()}");
            var dataSource = new DataSourceResult
            {
                Data = result,
                Total = result.Count
            };
            log.Info("**************** END GetGroupReadiness ****************");
            ViewBag.Datasource = dataSource;
            return Json(dataSource);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PeerFeedBackAuthorize(Role = "admin")]
        public ActionResult ExportGroupReadinessReport([Bind(Include = "TimeZone,Terms")] ExportGroupReadinessReportRequestModel model)
        {
            log.Info("**************** START ExportGroupReadinessReport ****************");
            log.Info($"ExportGroupReadinessReport parameters {model.ToJson()}");
            if (ModelState.IsValid)
            {
                string sheetName = "psfs-groupreadiness_courselist";
                var tz = TZConvert.IanaToWindows(model.TimeZone);
                var clientTz = TimeZoneInfo.FindSystemTimeZoneById(tz);
                var currentTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, clientTz);
                var fileName = $"GroupReadinessReport_{currentTime.ToString("yyyyMMdd")}_{currentTime.ToString("HHmmss")}.xlsx";
                log.Info($"export file name {fileName}");
                var codes = new List<string>();
                if (model != null && model.Terms != null && model.Terms.Any())
                {
                    var terms = _peerFeedbackService.PeerFeedbackGetWhitelistedTerm();
                    var values = terms.Where(x => model.Terms.Contains(x.Value)).SelectMany(i => i.Items).ToList();
                    if (values != null && values.Any())
                    {
                        codes = values.Select(x => x.Value).ToList();
                    }
                    log.Info($"ExportGroupReadinessReport codes {codes.ToJson()}");
                }
                log.Info("------ start GetGroupReadinessData ------");
                var result = _peerFeedbackService.GetGroupReadinessData(codes);
                if (result != null && result.Any())
                {
                    log.Info($"GetGroupReadinessData result {result.ToJson()}");
                    ExcelPackage excel = new ExcelPackage();
                    var workSheet = excel.Workbook.Worksheets.Add(sheetName);

                    // Setting the properties 
                    // of the first row 
                    workSheet.Row(1).Height = 20;
                    workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Row(1).Style.Font.Bold = true;

                    // Header of the Excel sheet 
                    workSheet.Cells[1, 1].Value = "Offering School";
                    workSheet.Cells[1, 2].Value = "Course Code";
                    workSheet.Cells[1, 3].Value = "Course Title";
                    workSheet.Cells[1, 4].Value = "Instructor";
                    workSheet.Cells[1, 5].Value = "Total Participants";
                    workSheet.Cells[1, 6].Value = "Group Names";
                    workSheet.Cells[1, 7].Value = "Number of Groups";
                    workSheet.Cells[1, 8].Value = "Unassigned Student Count";
                    workSheet.Cells[1, 9].Value = "Has Multiple Group (Y/N)";
                    workSheet.Cells[1, 10].Value = "Duplicate Enrolment (Y/N)";
                    workSheet.Cells[1, 11].Value = "Created In PSFS";
                    int recordIndex = 2;
                    foreach (var item in result)
                    {
                        workSheet.Cells[recordIndex, 1].Value = item.AcadGroup;
                        workSheet.Cells[recordIndex, 2].Value = item.CourseCode;
                        workSheet.Cells[recordIndex, 3].Value = item.CourseName;
                        workSheet.Cells[recordIndex, 4].Value = item.Instructor;
                        workSheet.Cells[recordIndex, 5].Value = item.StudentCount;
                        workSheet.Cells[recordIndex, 6].Value = item.GroupNames;
                        workSheet.Cells[recordIndex, 7].Value = item.GroupCount;
                        workSheet.Cells[recordIndex, 8].Value = item.UnassignedStudentCount;
                        workSheet.Cells[recordIndex, 9].Value = item.MultipleCategoryGroups;
                        workSheet.Cells[recordIndex, 10].Value = item.Duplicates;
                        workSheet.Cells[recordIndex, 11].Value = item.CreatedInPSFS;
                        recordIndex++;
                    }
                    log.Info("**************** END ExportGroupReadinessReport ****************");
                    return File(excel.GetAsByteArray(), "application/vnd.ms-excel", fileName);
                }
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Not Found.");
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "term is not valid.");
        }

        [HttpPost, ValidateAntiForgeryToken, PeerFeedBackAuthorize(Role = "admin")]
        public async Task<ActionResult> GetTerms([Bind(Include = "Page,PageSize,Filter")] KendoUIRequestModel request)
        {
            var takeRecentTerms = Constants.PeerFeedbackGetTermsRecentTermsSize;
            string filter = string.Empty;
            if(request.Filter != null && request.Filter.Filters != null && request.Filter.Filters.Any())
            {
                var filterValue = request.Filter.Filters.FirstOrDefault();
                if(filterValue != null && filterValue.Value != null)
                {
                    string[] arr = ((IEnumerable)filterValue.Value).Cast<object>()
                                   .Select(x => x.ToString())
                                   .ToArray();
                    filter = arr.FirstOrDefault();
                }
            }
            (int TotalCount, IList<CourseOfferingDto> Terms) = await _peerFeedbackService.PeerFeedbackGetWhitelistedTermPagingAsync(
                request.Page, 
                request.PageSize, 
                filter, Constants.UseFullDbName, 
                Constants.PeerFeedbackGetTermsEnableUGPG);
            log.Info($"get terms OK response = {JsonConvert.SerializeObject(Terms)}");
            var response = Terms.Select(item => new TextValue
            {
                Value = $"{item.STRM}",
                Text = $"{item.ACADEMIC_YEAR} T{item.ACADEMIC_TERM}"
            }).DistinctBy(x => new { x.Value, x.Text }).OrderByDescending(x => x.Text).Take(takeRecentTerms).ToList();
            var result = new
            {
                filter = filter,
                data = response,
                total = TotalCount > takeRecentTerms ? takeRecentTerms : TotalCount
            };
            return Json(result);
        }
        [HttpPost, ValidateAntiForgeryToken, PeerFeedBackAuthorize(Role = "admin")]
        public async Task<ActionResult> GetCourseOfferingCodeByStrm([Required] string strm)
        {
            var results = await _peerFeedbackService.PeerFeedbackGetCourseOfferingCodeByTerm(strm);
            return Json(results);
        }
        [HttpPost, ValidateAntiForgeryToken, PeerFeedBackAuthorize(Role = "admin")]
        public async Task<ActionResult> GetDefaultSelectedStrm([Required] string strm)
        {
            var result = await _peerFeedbackService.PeerFeedbackGetDefaultSelectedStrm(strm);
            var model = new
            {
                data = new List<TextValue> { result },
                total = 1
            };
            return Json(model);
        }
        [HttpPost, ValidateAntiForgeryToken, PeerFeedBackAuthorize(Role = "admin")]
        public ActionResult SyncGroupEnrollment(List<int> selectedCourseId)
        {
            if (selectedCourseId != null && selectedCourseId.Any())
            {
                _valenceService.SyncEnrollmentOnly(selectedCourseId);
            }
            return Json("");
        }
        #endregion

        #region GROUP READINESS
        [PeerFeedBackAuthorize(Role = "admin")]
        public ActionResult GroupReadiness()
        {
            return PartialView();
        }
        #endregion

        #region REPORT
        [HttpPost, ValidateAntiForgeryToken]
        [PeerFeedBackAuthorize(Role = "admin,instructor")]
        public ActionResult GetReportFilterSession()
        {
            var sessions = _peerFeedbackService.PeerFeedbackSessionsGetList();
            if (!UserInfo.HasAdmin)
            {
                var filterSession = new List<PeerFeedbackSessionsDto>();

                foreach (var session in sessions)
                {
                    if (!string.IsNullOrWhiteSpace(session.CourseOfferingCode))
                    {
                        var codes = session.CourseOfferingCode.IndexOf(",") > -1
                                ? session.CourseOfferingCode.Split(',').ToList()
                                : new List<string> { session.CourseOfferingCode };

                        var exists = UserInfo.CurrentLoadedCourses.Any(x =>
                                            codes.Contains(x.CourseCode)
                                            && string.Equals(x.RoleName, RoleName.Instructor.ToString(), StringComparison.OrdinalIgnoreCase));

                        if (exists)
                        {
                            filterSession.Add(session);
                        }
                    }
                }
                return Json(filterSession);
            }
            return Json(sessions);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [PeerFeedBackAuthorize(Role = "admin,instructor")]
        public ActionResult ExportPeerFeedBackToExcel([Bind(Include = "Sessions,GroupBy,ReportType,TimeZone,SessionNames")] ExportPeerFeedBackToExcelModel model)
        {
            if (ModelState.IsValid)
            {
                log.Info("------ start ExportPeerFeedBackToExcel ------");
                var peerFeedbackReportType = model.ReportType;
                log.Info($"Get PeerFeedBackReportType = {peerFeedbackReportType}");
                var peerFeedbackReportGroupBy = model.GroupBy;
                log.Info($"Get PeerFeedBackReportGroupBy = {peerFeedbackReportGroupBy.ToJson()}");
                var sessions = model.Sessions;
                log.Info($"Get sessions = {sessions.ToJson()}, sessionNames = {model.SessionNames}");
                var requestId = DateTime.Now.ToString("yyyyMMddHHmmssfffffff");
                log.Info($"Set date request = {requestId}");
                var courseOfferingCodes = _peerFeedbackService.PeerFeedBackSessionGetCourseOfferingCodeBySessionIds(sessions);
                if (courseOfferingCodes == null || !courseOfferingCodes.Any())
                {
                    log.Info($"sessions courseOfferingCode is EMPTY.");
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                log.Info($"sessions courseOfferingCode {courseOfferingCodes.ToJson()}");
                var courses = new List<int>();
                if (UserInfo.HasAdmin)
                {
                    courses = _courseService.GetCourseIdByListCode(courseOfferingCodes);
                }
                else if (UserInfo.IsInstructor)
                {
                    courses = UserInfo.CurrentLoadedCourses.Where(x =>
                                courseOfferingCodes.Contains(x.CourseCode)
                                && string.Equals(x.RoleName, RoleName.Instructor.ToString(), StringComparison.OrdinalIgnoreCase))
                                .Select(x => x.CourseId).ToList();
                }
                if (courses == null || !courses.Any())
                {
                    log.Info($"courses is EMPTY.");
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var logDetails = new
                {
                    sessionIds = sessions,
                    userInfo = new
                    {
                        courseIds = courses,
                        hasAdmin = UserInfo.HasAdmin,
                        isInstructors = UserInfo.IsInstructor,
                        enrolledCourse = UserInfo.CurrentLoadedCourses
                    },
                    CourseOfferingCodeBySelectedSessions = courseOfferingCodes,
                };
                log.Info($"ExportPeerFeedBackToExcel logDetails {logDetails.ToJson()}");
                log.Info($"Get Course by UserId, Session = {courses.ToJson()}");
                var userInfo = UserInfo;
                var peerFeedbackReportJob = new PeerFeedbackReportJob(_userService, _cacheManager, _loggingService);
                log.Info($"Get peerFeedbackReportJob = {peerFeedbackReportJob.ToJson()}");
                var psfsReportBaseFolder = $@"{Server.MapPath(Constants.StaticFilesFolder)}\PeerFeedBack\Report";
                log.Info($"Get psfsReportBaseFolder = {psfsReportBaseFolder}");
                HostingEnvironment.QueueBackgroundWorkItem(
                    ct => peerFeedbackReportJob.Run(
                        peerFeedbackReportType,
                        peerFeedbackReportGroupBy,
                        sessions,
                        model.SessionNames,
                        courses,
                        psfsReportBaseFolder,
                        requestId,
                        userInfo, model.TimeZone));
                log.Info("------ end ExportPeerFeedBackToExcel ------");
                return Json(requestId);
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        #region PREPARE DATA EXPORT

        #region OVERALL RESPONSE RATE
        [NonAction]
        private byte[] PrepareReportOverallResponseRate(ExportPeerFeedBackToExcelModel model)
        {
            byte[] data = null;
            var fullFileName = GetPeerFeedBackTemplateFileName(model);
            var fi = new FileInfo(fullFileName);
            using (var package = new ExcelPackage(fi))
            {
                data = package.GetAsByteArray();
            }
            return data;
        }
        #endregion

        #region OVERALL DESCRIPTOR
        [NonAction]
        private byte[] PrepareReportOverallDescriptorResult(ExportPeerFeedBackToExcelModel model)
        {
            byte[] data = null;
            var fullFileName = GetPeerFeedBackTemplateFileName(model);
            var fi = new FileInfo(fullFileName);
            using (var package = new ExcelPackage(fi))
            {
                data = package.GetAsByteArray();
            }
            return data;
        }
        #endregion

        #region OVERALL MEAN SCORE
        [NonAction]
        private byte[] PrepareReportOverallMeanScoreResult(ExportPeerFeedBackToExcelModel model)
        {
            byte[] data = null;
            var fullFileName = GetPeerFeedBackTemplateFileName(model);
            var fi = new FileInfo(fullFileName);
            using (var package = new ExcelPackage(fi))
            {
                data = package.GetAsByteArray();
            }
            return data;
        }
        #endregion

        #region INDIVIDUAL STUDENT
        [NonAction]
        private byte[] PrepareReportIndividualStudentResult(ExportPeerFeedBackToExcelModel model)
        {
            byte[] data = null;
            var fullFileName = GetPeerFeedBackTemplateFileName(model);
            var fi = new FileInfo(fullFileName);
            using (var package = new ExcelPackage(fi))
            {
                data = package.GetAsByteArray();
            }
            return data;
        }
        #endregion

        #endregion

        #region PEERFEEDBACK TEMPLATE FILE NAME
        private string GetPeerFeedBackTemplateFileName(ExportPeerFeedBackToExcelModel model)
        {
            string fileName = string.Empty;
            var reportType = model.ReportType;
            var groupBy = model.GroupBy == PeerFeedBackReportGroupBy.CourseSection ? "GroupByCourseSection" : "GroupByStudentSchool";
            switch (reportType)
            {
                case PeerFeedBackReportType.IndividualStudentResult:
                    fileName = $"{PeerFeedBackReportType.IndividualStudentResult}";
                    break;
                case PeerFeedBackReportType.OverallResponseRate:
                    fileName = $"{PeerFeedBackReportType.OverallResponseRate}";
                    break;
                case PeerFeedBackReportType.OverallDescriptorResult:
                    fileName = $"{PeerFeedBackReportType.OverallDescriptorResult}";
                    break;
                case PeerFeedBackReportType.OverallMeanScoreResult:
                    fileName = $"{PeerFeedBackReportType.OverallMeanScoreResult}";
                    break;
            }
            var path = $@"{Server.MapPath(Constants.StaticFilesFolder)}\PeerFeedBack\Report";
            return $@"{path}\{fileName}_{groupBy}.xlsx";
        }
        private string GetPeerFeedBackDownloadFileName(ExportPeerFeedBackToExcelModel model)
        {
            string fileName = string.Empty;
            var reportType = (PeerFeedBackReportType)model.ReportType;
            switch (reportType)
            {
                case PeerFeedBackReportType.IndividualStudentResult:
                    fileName = "individual-student-report.xlsx";
                    break;
                case PeerFeedBackReportType.OverallResponseRate:
                    fileName = "overall-response-rate.xlsx";
                    break;
                case PeerFeedBackReportType.OverallDescriptorResult:
                    fileName = "section-overall-descriptor-result.xlsx";
                    break;
                case PeerFeedBackReportType.OverallMeanScoreResult:
                    fileName = "overall-mean-score-result.xlsx";
                    break;
            }
            return fileName;
        }
        #endregion

        #endregion
    }
}