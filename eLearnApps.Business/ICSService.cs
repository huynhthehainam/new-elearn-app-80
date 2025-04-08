#region USING

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using eLearnApps.Business.Interface;
using eLearnApps.Core;
using eLearnApps.Core.Cryptography;
using eLearnApps.Data;
using eLearnApps.Data.Interface;
using eLearnApps.Data.Interface.LmsTool;
using eLearnApps.Entity.LmsIsis;
using eLearnApps.Entity.LmsTools;
using eLearnApps.Entity.LmsTools.Dto;
using eLearnApps.ViewModel.ICS;
using eLearnApps.ViewModel.ISIS;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace eLearnApps.Business
{
    public class IcsService : IIcsService
    {
        #region CTOR
        private readonly LMSIsisContext _isisDbContext;
        public IcsService(
            IServiceProvider serviceProvider,
             IDaoFactory factory,
             LMSIsisContext isisContext,
            IUserEnrollmentService userEnrollmentService)
        {
            _icsSessionRepository = serviceProvider.GetRequiredKeyedService<IRepository<ICSSession>>("default"); ;
            _questionRepository = serviceProvider.GetRequiredKeyedService<IRepository<Question>>("default"); ;
            _learningPointRepository = serviceProvider.GetRequiredKeyedService<IRepository<LearningPoint>>("default"); ;
            _learningPointCheckRepository = serviceProvider.GetRequiredKeyedService<IRepository<LearningPointCheck>>("default"); ;
            _icsSessionUserSenseRepository = serviceProvider.GetRequiredKeyedService<IRepository<ICSSessionUserSense>>("default"); ;
            _userEnrollmentRepository = serviceProvider.GetRequiredKeyedService<IRepository<UserEnrollment>>("default"); ;
            _courseRepository = serviceProvider.GetRequiredKeyedService<IRepository<Course>>("default"); ;
            _userRepository = serviceProvider.GetRequiredKeyedService<IRepository<User>>("default"); ;
            _userEnrollmentService = userEnrollmentService;

            _isisDbContext = isisContext;
            _icsSessionDao = factory.IcsSessionDao;
            _icsSessionUserSenseDao = factory.IcsSessionUserSenseDao;
            _questionDao = factory.QuestionDao;
            _learningPointDao = factory.LearningPointDao;
            _learningPointCheckDao = factory.LearningPointCheckDao;
        }

        #endregion

        #region SERVICE

        private readonly IRepository<Course> _courseRepository;
        private readonly IRepository<ICSSession> _icsSessionRepository;
        private readonly IRepository<ICSSessionUserSense> _icsSessionUserSenseRepository;
        private readonly IRepository<LearningPointCheck> _learningPointCheckRepository;
        private readonly IRepository<LearningPoint> _learningPointRepository;
        private readonly IRepository<Question> _questionRepository;
        private readonly IRepository<UserEnrollment> _userEnrollmentRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IIcsSessionDao _icsSessionDao;
        private readonly IUserEnrollmentService _userEnrollmentService;
        private readonly IIcsSessionUserSenseDao _icsSessionUserSenseDao;
        private readonly IQuestionDao _questionDao;
        private readonly ILearningPointDao _learningPointDao;
        private readonly ILearningPointCheckDao _learningPointCheckDao;

        #endregion

        #region Interface Implementation

        public List<SessionViewModel> GetSessions(int courseId)
        {
            var allUserIds = _userEnrollmentRepository.Table.Where(ue => ue.CourseId == courseId && ue.IsClasslist)
                .Select(ue => ue.UserId.Value).ToList();
            var vm = _icsSessionRepository.Table
                .Where(ics => ics.CourseId == courseId && ics.RecordStatus == (int)RecordStatus.Active).Select(ics =>
                   new SessionViewModel
                   {
                       Id = ics.Id,
                       StartDate = ics.SessionDate,
                       StartTime = ics.StartTime,
                       EndTime = ics.EndTime,
                       CourseId = ics.CourseId
                   })
                .ToList();
            foreach (var session in vm)
            {
                var userSensesReceived = GetSenses(session.Id);
                var questions = GetQuestions(session.Id);
                var learningPoints = GetLearningPointBySessionId(session.Id);
                var learningPointIds = learningPoints.Select(lp => lp.Id).ToList();
                var learningPointChecks = GetLPChecks(learningPointIds);

                CalculateLpProgress(learningPoints, learningPointChecks, allUserIds);
                session.IsEditable =
                    IsSessionEditable(userSensesReceived.Count, questions.Count, learningPointChecks.Count);
                session.Progress = CalculateClassProgress(learningPoints);
            }

            return vm;
        }

        public List<SessionViewModel> GetListSession(int courseId)
        {
            var allUserIds = _userEnrollmentService.GetUserIdEnrollByCourse(courseId);
            var vm = _icsSessionDao.GetByCourse(courseId);
            var lstSessionId = vm.Select(x => x.Id.ToString()).ToList();
            var userSensesReceived = _icsSessionUserSenseDao.GetByListSessionId(lstSessionId);
            var questions = _questionDao.GetByListSession(lstSessionId);
            var learningPoints = _learningPointDao.GetByListSession(lstSessionId);
            var learningPointIds = learningPoints.Select(lp => lp.Id.ToString()).ToList();
            var learningPointChecks = _learningPointCheckDao.GetByListLearningPoint(learningPointIds);

            var totalUserCount = allUserIds.Count;
            var lstModel = new List<SessionViewModel>();
            foreach (var item in vm)
            {
                var learningPointFilter = learningPoints.Where(x => x.ICSSessionId == item.Id).ToList();
                var learningPointFilterIds = learningPointFilter.Select(x => x.Id).ToList();
                var learningPointCheckFilter = learningPointChecks
                    .Where(x => learningPointFilterIds.Contains(x.LearningPointId)).ToList();
                var userSensesReceivedCount = userSensesReceived.Count(x => x.ICSSessionId == item.Id);
                var questionCount = questions.Count(x => x.ICSSessionId == item.Id);
                // calculate progress if there are  feedback received
                if (learningPointCheckFilter.Count > 0)
                    // get total student in class
                    foreach (var lp in learningPointFilter)
                    {
                        var progress = 0;
                        if (totalUserCount > 0)
                        {
                            var count = learningPointCheckFilter.Count(lpc =>
                                lpc.LearningPointId == lp.Id && allUserIds.Contains(lpc.UserId));
                            progress = Convert.ToInt32(Math.Round((double)count / totalUserCount * 100,
                                MidpointRounding.AwayFromZero));
                        }

                        lp.Progress = progress;
                    }

                var isEditable =
                    IsSessionEditable(userSensesReceivedCount, questionCount, learningPointCheckFilter.Count);

                var countLearningPoint = learningPointFilter.Count;
                var classProgress = countLearningPoint == 0
                    ? 0
                    : Convert.ToInt32(Math.Round(
                        (double)learningPointFilter.Select(lp => lp.Progress).Sum() / countLearningPoint,
                        MidpointRounding.AwayFromZero));

                lstModel.Add(new SessionViewModel
                {
                    Id = item.Id,
                    StartDate = item.SessionDate,
                    StartTime = item.StartTime,
                    EndTime = item.EndTime,
                    CourseId = item.CourseId,
                    IsEditable = isEditable,
                    Progress = classProgress,
                    Title = item.Title
                });
            }

            return lstModel;
        }

        /// <summary>
        ///     Check user can delete session
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public int CanDeleteSession(int sessionId)
        {
            return _icsSessionDao.CanDeleteSession(sessionId);
        }

        /// <summary>
        ///     Check exists ics session between start time - end time
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="sessionDate"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public List<ICSSession> GetSessionsWithinTimeRange(int courseId, DateTime sessionDate, TimeSpan startTime, TimeSpan endTime)
        {
            var sessionWithinRange = _icsSessionDao.CheckRangeStartTimeEndTime(courseId, sessionDate, startTime, endTime);
            return sessionWithinRange;
        }

        public SessionDetailViewModel GetSessionDetail(int sessionId)
        {
            var session = _icsSessionRepository.GetById(sessionId);
            var allUserIds = _userEnrollmentRepository.Table
                .Where(ue => ue.CourseId == session.CourseId && ue.IsClasslist).Select(ue => ue.UserId.Value).ToList();

            var learningPoints = GetLearningPointBySessionId(sessionId);
            var questions = GetQuestions(sessionId);
            var userSenses = GetSenses(sessionId);
            var userLpChecks = GetLPChecks(learningPoints.Select(lp => lp.Id).ToList());

            CalculateLpProgress(learningPoints, userLpChecks, allUserIds);

            var vm = new SessionDetailViewModel
            {
                Session = new SessionViewModel
                {
                    Id = session.Id,
                    StartDate = session.SessionDate,
                    StartTime = session.StartTime,
                    EndTime = session.EndTime,
                    CourseId = session.CourseId,
                    IsEditable = IsSessionEditable(userSenses.Count, questions.Count, userLpChecks.Count),
                    Progress = CalculateClassProgress(learningPoints),
                    TotalUserInThisCourse = allUserIds.Count,
                    Title = session.Title
                },
                LearningPoints = learningPoints,
                Questions = questions,
                UserSenses = userSenses,
                LearningPointChecks = userLpChecks
            };

            return vm;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public SessionDetailViewModel GetSessionLearningPointAndQuestion(int sessionId)
        {
            var learningPoints = GetLearningPointBySessionId(sessionId);
            var questions = GetQuestions(sessionId);
            var vm = new SessionDetailViewModel
            {
                LearningPoints = learningPoints,
                Questions = questions
            };
            return vm;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public SessionDetailViewModel GetSessionQuestion(int sessionId)
        {
            var questions = GetQuestions(sessionId);
            var vm = new SessionDetailViewModel
            {
                Questions = questions
            };
            return vm;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<LearningPoint> GetLearningPointBySessionAndId(int sessionId, int id)
        {
            var learningPoint = await _learningPointRepository.Table
                .FirstOrDefaultAsync(lp =>
                    lp.ICSSessionId == sessionId && lp.RecordStatus == (int)RecordStatus.Active && lp.Id == id);

            return learningPoint;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="sessionDate"></param>
        /// <param name="startTime"></param>
        /// <returns></returns>
        public async Task<ICSSession> GetSessionBySessionDateTime(int courseId, DateTime sessionDate, TimeSpan startTime)
        {
            var query = _icsSessionRepository.Table.Where(s => s.CourseId == courseId && s.RecordStatus == (int)RecordStatus.Active && DbFunctions.TruncateTime(s.SessionDate) == sessionDate && s.StartTime == startTime);
            var session = await query.FirstOrDefaultAsync();
            return session;
        }
        public SessionDetailViewModel GetUserSessionDetail(int sessionId, int userId)
        {
            var session = _icsSessionRepository.GetById(sessionId);
            var learningPoints = GetLearningPointBySessionId(sessionId);
            var questions = GetQuestions(sessionId);
            var userSenses = GetUserSenses(sessionId, userId);
            var userLpChecks = GetUserLPChecks(learningPoints.Select(lp => lp.Id).ToList(), userId);
            var allUserIds = _userEnrollmentRepository.Table
                .Where(ue => ue.CourseId == session.CourseId && ue.IsClasslist).Select(ue => ue.UserId.Value).ToList();
            CalculateLpProgress(learningPoints, userLpChecks, allUserIds);

            var vm = new SessionDetailViewModel
            {
                Session = new SessionViewModel
                {
                    Id = session.Id,
                    StartDate = session.SessionDate,
                    StartTime = session.StartTime,
                    EndTime = session.EndTime,
                    CourseId = session.CourseId,
                    Title = session.Title,
                    IsEditable = IsSessionEditable(userSenses.Count(), questions.Count, userLpChecks.Count)
                },
                LearningPoints = learningPoints,
                Questions = questions,
                UserSenses = userSenses,
                LearningPointChecks = userLpChecks
            };

            return vm;
        }

        /// <summary>
        /// </summary>
        /// <param name="learningPoints"></param>
        /// <param name="userLpChecks"></param>
        /// <param name="validUserIds">
        ///     If user is part of "IsInClass" user then their votes count. if not, their votes is not
        ///     counted
        /// </param>
        private static void CalculateLpProgress(List<LearningPointViewModel> learningPoints,
            List<LearningPointCheckViewModel> userLpChecks, List<int> validUserIds)
        {
            var totalUserCount = validUserIds.Count();

            // calculate progress if there are  feedback received
            if (userLpChecks.Count > 0)
            {
                // get total student in class
                var progress = 0;
                foreach (var lp in learningPoints)
                {
                    if (validUserIds.Count > 0)
                    {
                        var count = userLpChecks.Count(lpc =>
                            lpc.LearningPointId == lp.Id && validUserIds.Contains(lpc.UserId));
                        progress = Convert.ToInt32((double)count / totalUserCount * 100);
                    }
                    else
                    {
                        progress = 0;
                    }

                    lp.Progress = progress;
                }
            }
        }

        public int CreateSession(DateTime sessionDate, TimeSpan starTime, TimeSpan endTime, int courseId, string title)
        {
            var newSession = new ICSSession
            {
                SessionDate = sessionDate,
                StartTime = starTime,
                EndTime = endTime,
                CourseId = courseId,
                RecordStatus = (int)RecordStatus.Active,
                CreatedOn = DateTime.UtcNow,
                LastUpdatedOn = null,
                Title = title
            };

            _icsSessionRepository.Insert(newSession);

            return newSession.Id;
        }

        public int CreateSessionFromClassSchedule(int courseId)
        {
            var isisCourseOfferingRepo = new Repository<TL_CourseOfferings>(_isisDbContext);
            var isisClassScheduleRepo = new Repository<PS_SIS_LMS_SCHED_V>(_isisDbContext);

            // get course offering code 
            var selectedCourse = _courseRepository.Table
                .FirstOrDefault(c => c.Id == courseId);

            if (selectedCourse == null) return 0;

            var courseOfferingCode = selectedCourse.Code;
            var courseDetail = isisCourseOfferingRepo.Table
                .FirstOrDefault(c => c.CourseOfferingCode == courseOfferingCode);

            if (courseDetail == null) return 0;

            var strm = courseDetail.STRM;
            var classNbr = Convert.ToInt32(courseDetail.CLASS_NBR.Value);

            // get class schedule from isis
            var classSchedules = isisClassScheduleRepo.Table
                .Where(cls => cls.STRM == strm && cls.CLASS_NBR == classNbr)
                .ToList();

            var schedules = extractSchedules(classSchedules);

            var totalCreated = 0;

            // cycle through each day from course start to course end date
            var diff = courseDetail.END_DATE.Value - courseDetail.START_DATE.Value;
            var days = diff.Days;
            for (var i = 0; i <= days; i++)
            {
                // for each day, check if there is any class scheduled
                var tempDate = courseDetail.START_DATE.Value.AddDays(i);
                var classesScheduledForTempDate = schedules.Where(s => s.SelectedDays.Contains(tempDate.DayOfWeek));

                // create session based on class schedule
                foreach (var classScheduled in classesScheduledForTempDate)
                {
                    var newSession = new ICSSession
                    {
                        SessionDate = tempDate,
                        StartTime = classScheduled.Start,
                        EndTime = classScheduled.End,
                        CourseId = courseId,
                        RecordStatus = (int)RecordStatus.Active,
                        CreatedOn = DateTime.Now,
                        LastUpdatedOn = null
                    };

                    _icsSessionRepository.Insert(newSession);

                    totalCreated += 1;
                }
            }

            return totalCreated;
        }

        public void UpdateSession(int sessionId, DateTime sessionDate, TimeSpan startTime, TimeSpan endTime, string title)
        {
            var session = _icsSessionRepository.GetById(sessionId);

            session.SessionDate = sessionDate;
            session.StartTime = startTime;
            session.EndTime = endTime;
            session.Title = title;

            _icsSessionRepository.Update(session);
        }

        public void DeleteSession(int sessionId)
        {
            var session = _icsSessionRepository.GetById(sessionId);

            session.RecordStatus = (int)RecordStatus.Deleted;

            _icsSessionRepository.Update(session);
        }

        public List<LearningPointViewModel> GetLearningPointBySessionId(int sessionId)
        {
            var learningPoints = _learningPointRepository.Table
                .Where(lp => lp.ICSSessionId == sessionId && lp.RecordStatus == (int)RecordStatus.Active)
                .ToList();

            var vm = learningPoints
                .Select(lp => new LearningPointViewModel
                {
                    Id = lp.Id,
                    SessionId = lp.ICSSessionId,
                    Description = lp.Description
                })
                .ToList();

            return vm;
        }

        public LearningPointViewModel GetLearningPoint(int learningPointId)
        {
            var learningPoint = _learningPointRepository.Table
                .Where(l => l.Id == learningPointId)
                .Select(l => new LearningPointViewModel
                {
                    Id = l.Id,
                    SessionId = l.ICSSessionId,
                    Description = l.Description
                })
                .FirstOrDefault();


            return learningPoint;
        }

        public LearningPointViewModel AddLearningPoint(int sessionId, string description)
        {
            var newLearningPoint = new LearningPoint
            {
                ICSSessionId = sessionId,
                Description = description,
                RecordStatus = (int)RecordStatus.Active,
                CreatedOn = DateTime.UtcNow,
                LastUpdatedOn = DateTime.UtcNow
            };

            _learningPointRepository.Insert(newLearningPoint);

            var vm = new LearningPointViewModel
            {
                Id = newLearningPoint.Id,
                SessionId = newLearningPoint.ICSSessionId,
                Description = newLearningPoint.Description
            };

            return vm;
        }

        public void DeleteLearningPoint(int learningPointId)
        {
            var lpToDelete = _learningPointRepository.GetById(learningPointId);

            lpToDelete.RecordStatus = (int)RecordStatus.Deleted;
            lpToDelete.LastUpdatedOn = DateTime.UtcNow;

            _learningPointRepository.Update(lpToDelete);
        }

        public void UpdateLearningPoint(int learningPointId, string description)
        {
            var lpToUpdate = _learningPointRepository.GetById(learningPointId);

            lpToUpdate.Description = description;
            lpToUpdate.LastUpdatedOn = DateTime.UtcNow;

            _learningPointRepository.Update(lpToUpdate);
        }

        public List<QuestionViewModel> GetQuestions(int sessionId)
        {
            var questions = _questionRepository.Table
                .Where(q => q.ICSSessionId == sessionId && q.RecordStatus == (int)RecordStatus.Active)
                .Select(q => new QuestionViewModel
                {
                    Id = q.Id,
                    UserId = q.UserId,
                    SessionId = q.ICSSessionId,
                    Content = q.Content,
                    Addressed = q.Addressed
                })
                .ToList();
            questions = questions.Select(x =>
            {
                x.SessionKey = AesEncrypt.Encrypt(x.SessionId.ToString());
                x.QuestionKey = AesEncrypt.Encrypt(x.Id.ToString());
                return x;
            }).OrderBy(x => x.Addressed).ToList();
            return questions;
        }

        public QuestionViewModel AddQuestion(int sessionId, int userId, string question)
        {
            var newQuestion = new Question
            {
                ICSSessionId = sessionId,
                Content = question,
                UserId = userId,
                RecordStatus = (int)RecordStatus.Active,
                CreatedOn = DateTime.UtcNow,
                LastUpdatedOn = DateTime.UtcNow
            };

            _questionRepository.Insert(newQuestion);

            var vm = new QuestionViewModel
            {
                Id = newQuestion.Id,
                SessionId = newQuestion.ICSSessionId,
                UserId = newQuestion.UserId,
                Content = newQuestion.Content
            };

            return vm;
        }

        public void DeleteQuestion(int questionId)
        {
            var questionToDelete = _questionRepository.GetById(questionId);

            questionToDelete.RecordStatus = (int)RecordStatus.Deleted;
            questionToDelete.LastUpdatedOn = DateTime.UtcNow;

            _questionRepository.Update(questionToDelete);
        }

        public void EditQuestion(int questionId, string question)
        {
            var questionToUpdate = _questionRepository.GetById(questionId);

            questionToUpdate.Content = question;
            questionToUpdate.LastUpdatedOn = DateTime.UtcNow;

            _questionRepository.Update(questionToUpdate);
        }

        public void AddSense(ICSSessionUserSense userSense)
        {
            _icsSessionUserSenseRepository.Insert(userSense);
        }

        public void SetLearningPointCheck(int learningPointId, int userId, bool checkLearningPoint)
        {
            var learningPointCheck = _learningPointCheckRepository.Table
                .FirstOrDefault(lpc => lpc.LearningPointId == learningPointId && lpc.UserId == userId);

            if (checkLearningPoint)
            {
                // to check an LP, we need a record of learningpointcheck
                // if check already exist then leave it, else add the check
                if (learningPointCheck == null)
                    _learningPointCheckRepository.Insert(new LearningPointCheck
                    {
                        LearningPointId = learningPointId,
                        UserId = userId,
                        CreatedOn = DateTime.UtcNow
                    });
            }
            else
            {
                // to uncheck an LP, we need remove record of check. if no record existed, no action needed, else we need to delete it
                if (learningPointCheck != null) _learningPointCheckRepository.Delete(learningPointCheck);
            }
        }

        public SessionChartViewModel GetClassSensingSummary(int sessionId)
        {
            // every user is assumed to be "doingwell"
            var session = _icsSessionRepository.GetById(sessionId);
            var allUserSense = _userEnrollmentRepository.Table
                .Where(ue => ue.CourseId == session.CourseId && ue.IsClasslist)
                .Select(ue => new UserSenseViewModel
                {
                    Sessionid = sessionId,
                    UserId = ue.UserId.Value,
                    Sense = Senses.DoingWell,
                    TimeStamp = DateTime.UtcNow
                })
                .ToList();

            // get latest feeling received for each user. any user without any input is assumed "unchanged", hence "doing well"
            var userSenses = GetSenses(sessionId)
                .GroupBy(us => us.UserId)
                .Select(grp => new
                {
                    sessionId,
                    UserId = grp.Key,
                    grp.OrderByDescending(item => item.TimeStamp).First().Sense
                })
                .ToList();

            // update allUserSense with latest feeling received.
            foreach (var us in userSenses)
            {
                var currentUser = allUserSense.Where(tu => tu.UserId == us.UserId).FirstOrDefault();
                if (currentUser != null)
                    currentUser.Sense = us.Sense;
            }

            // construct return value
            var model = new SessionChartViewModel
            {
                StartDate = session.SessionDate,
                StartTime = session.StartTime,
                EndTime = session.EndTime,
                Senses = allUserSense
            };

            return model;
        }

        public SessionLineChart GetAllClassSensing(int sessionId)
        {
            // every user is assumed to be "doingwell"
            var session = _icsSessionRepository.GetById(sessionId);
            var users = (from u in _userRepository.Table
                         join ue in _userEnrollmentRepository.Table on u.Id equals ue.UserId
                         where ue.CourseId == session.CourseId && ue.IsClasslist
                         select u).ToList();

            // get latest feeling received for each user. any user without any input is assumed "unchanged", hence "doing well"
            var userIds = users.Select(u => u.Id).ToList();
            var sessionSenses = from us in _icsSessionUserSenseRepository.Table
                                where us.ICSSessionId == sessionId && userIds.Contains(us.UserId)
                                select us;

            // construct return value
            var model = new SessionLineChart
            {
                Users = users.ToList(),
                Session = session,
                Senses = sessionSenses.ToList()
            };

            return model;
        }

        /// <summary>
        ///     Get ics session by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ICSSession GetIcsSessionById(int id)
        {
            return _icsSessionDao.GetById(id);
        }

        public async Task UpdateQuestionAddressed(Question question)
        {
            if (question.Addressed == false) question.Addressed = null;

            await _questionDao.UpdateQuestionAddressed(question);
        }
        public async Task<bool> IsExistsQuestionById(int id)
        {
            return await _questionRepository.TableNoTracking.Where(x => x.Id == id && x.RecordStatus == (int)RecordStatus.Active)
                .AnyAsync();
        }
        public async Task UpdateDoingWellBySessionId(int sessionId)
        {
            await _icsSessionUserSenseDao.UpdateDoingWellBySessionId(sessionId).ConfigureAwait(false);
        }
        public async Task DeleteByCondition(ICSSessionUserSense userSense)
        {
            await _icsSessionUserSenseDao.DeleteByCondition(userSense).ConfigureAwait(false);
        }
        #endregion

        #region Private Support Function

        private bool IsSessionEditable(int senseReceivedCount, int questionReceivedCount, int lpcReceivedCount)
        {
            // editable only if there is no question and no lp checked
            if (senseReceivedCount == 0 && questionReceivedCount == 0 && lpcReceivedCount == 0)
                return true;
            return false;
        }

        private int CalculateClassProgress(List<LearningPointViewModel> learningPoints)
        {
            var classProgress = 0;
            if (learningPoints.Count == 0)
                classProgress = 0;
            else
                classProgress = Convert.ToInt32(Math.Round(
                    (double)learningPoints.Select(lp => lp.Progress).Sum() / learningPoints.Count(),
                    MidpointRounding.AwayFromZero));

            return classProgress;
        }

        private List<ClassScheduleViewModel> extractSchedules(IEnumerable<PS_SIS_LMS_SCHED_V> classSchedules)
        {
            var schedules = new List<ClassScheduleViewModel>();

            foreach (var classSchedule in classSchedules)
            {
                var start = new TimeSpan(classSchedule.MEETING_TIME_START.Value.Hour,
                    classSchedule.MEETING_TIME_START.Value.Minute, classSchedule.MEETING_TIME_START.Value.Second);
                var end = new TimeSpan(classSchedule.MEETING_TIME_END.Value.Hour,
                    classSchedule.MEETING_TIME_END.Value.Minute, classSchedule.MEETING_TIME_END.Value.Second);
                var tempSchedule = new ClassScheduleViewModel
                {
                    SelectedDays = new List<DayOfWeek>(),
                    Start = start,
                    End = end
                };

                if (classSchedule.SUN.Trim().ToLower() == "y")
                    tempSchedule.SelectedDays.Add(DayOfWeek.Sunday);

                if (classSchedule.MON.Trim().ToLower() == "y")
                    tempSchedule.SelectedDays.Add(DayOfWeek.Monday);

                if (classSchedule.TUES.Trim().ToLower() == "y")
                    tempSchedule.SelectedDays.Add(DayOfWeek.Tuesday);

                if (classSchedule.WED.Trim().ToLower() == "y")
                    tempSchedule.SelectedDays.Add(DayOfWeek.Wednesday);

                if (classSchedule.THURS.Trim().ToLower() == "y")
                    tempSchedule.SelectedDays.Add(DayOfWeek.Thursday);

                if (classSchedule.FRI.Trim().ToLower() == "y")
                    tempSchedule.SelectedDays.Add(DayOfWeek.Friday);

                if (classSchedule.SAT.Trim().ToLower() == "y")
                    tempSchedule.SelectedDays.Add(DayOfWeek.Saturday);

                schedules.Add(tempSchedule);
            }

            return schedules;
        }

        private List<UserSenseViewModel> GetUserSenses(int sessionId)
        {
            return _icsSessionUserSenseRepository.Table
                .Where(r => r.ICSSessionId == sessionId)
                .Select(r => new UserSenseViewModel
                {
                    Id = r.Id,
                    Sessionid = r.ICSSessionId,
                    UserId = r.UserId,
                    Sense = r.Sense
                })
                .ToList();
        }

        private List<UserSenseViewModel> GetUserSenses(int sessionId, int userId)
        {
            return _icsSessionUserSenseRepository.Table
                .Where(r => r.ICSSessionId == sessionId && r.UserId == userId)
                .Select(r => new UserSenseViewModel
                {
                    Id = r.Id,
                    Sessionid = r.ICSSessionId,
                    UserId = r.UserId,
                    Sense = r.Sense
                })
                .ToList();
        }

        private List<UserSenseViewModel> GetSenses(int sessionId)
        {
            return _icsSessionUserSenseRepository.Table
                .Where(r => r.ICSSessionId == sessionId)
                .Select(r => new UserSenseViewModel
                {
                    Id = r.Id,
                    Sessionid = r.ICSSessionId,
                    UserId = r.UserId,
                    Sense = r.Sense
                })
                .ToList();
        }

        private List<LearningPointCheckViewModel> GetUserLPChecks(List<int> learningPointIds, int userId)
        {
            return _learningPointCheckRepository.Table
                .Where(lpc => learningPointIds.Contains(lpc.LearningPointId) && lpc.UserId == userId)
                .Select(lpc => new LearningPointCheckViewModel
                {
                    Id = lpc.Id,
                    LearningPointId = lpc.LearningPointId,
                    UserId = lpc.UserId
                })
                .ToList();
        }

        private List<LearningPointCheckViewModel> GetLPChecks(List<int> learningPointIds)
        {
            return _learningPointCheckRepository.Table
                .Where(lpc => learningPointIds.Contains(lpc.LearningPointId))
                .Select(lpc => new LearningPointCheckViewModel
                {
                    Id = lpc.Id,
                    LearningPointId = lpc.LearningPointId,
                    UserId = lpc.UserId
                })
                .ToList();
        }

        #endregion
    }
}