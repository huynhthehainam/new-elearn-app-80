using eLearnApps.Business.Interface;
using eLearnApps.Core;
using eLearnApps.Data;
using eLearnApps.Data.Interface;
using eLearnApps.Data.Interface.Lmsisis;
using eLearnApps.Data.Interface.LmsTool;
using eLearnApps.Entity.LmsIsis;
using eLearnApps.Entity.LmsIsis.Dto;
using eLearnApps.Entity.LmsTools;
using eLearnApps.Entity.LmsTools.Dto;
using eLearnApps.ViewModel.RPT;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Linq;
using System.Threading.Tasks;

namespace eLearnApps.Business
{
    public class PeerFeedbackService : IPeerFeedbackService
    {
        private readonly IRepository<PeerFeedback> _repositoryPeerFeedback;
        private readonly IRepository<PeerFeedbackEvaluators> _repositoryPeerFeedbackEvaluators;
        private readonly IRepository<PeerFeedbackPairings> _repositoryPeerFeedbackPairings;
        private readonly IRepository<PeerFeedbackQuestion> _repositoryPeerFeedbackQuestion;
        private readonly IRepository<PeerFeedbackQuestionRatingMap> _repositoryPeerFeedbackQuestionRatingMap;
        private readonly IRepository<PeerFeedbackRatingOption> _repositoryPeerFeedbackRatingOption;
        private readonly IRepository<PeerFeedbackRatingQuestion> _repositoryPeerFeedbackRatingQuestion;
        private readonly IRepository<PeerFeedbackSessions> _repositoryPeerFeedbackSessions;
        private readonly IRepository<PeerFeedbackTargets> _repositoryPeerFeedbackTargets;
        private readonly IRepository<PeerFeedbackQuestionMap> _repositoryPeerFeedbackQuestionMap;
        private readonly IRepository<PeerFeedBackPairingSessions> _repositoryPeerFeedBackPairingSessions;
        private readonly IRepository<PeerFeedBackResponses> _repositoryPeerFeedBackResponses;
        private readonly IRepository<PeerFeedBackResponseRemarks> _repositoryPeerFeedBackResponseRemarks;
        private readonly IRepository<Role> _repositoryRoles;

        private readonly IRepository<UserEnrollment> _userEnrollmentRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserGroup> _userGroupRepository;
        private readonly IRepository<CategoryGroup> _categoryGroupRepository;
        private readonly IRepository<eLearnApps.Entity.LmsTools.CourseCategory> _courseCategoryRepository;
        private readonly IRepository<TL_CourseOfferings> _repositoryTlCourseOfferings;
        private readonly IRepository<Course> _repositoryCourse;
        private readonly IRepository<UserGroup> _repositoryUserGroup;

        private readonly IDbContext _context;
        private readonly IDbContext _isisContext;
        private readonly IPeerFeedBackResponsesDao _peerFeedBackResponsesDao;
        private readonly IPeerFeedBackResponseRemarksDao _peerFeedBackResponseRemarksDao;
        private readonly ITLCourseOfferingDao _tLCourseOfferingDao;
        private readonly IPeerFeedBackDao _peerFeedBackDao;
        private readonly ICategoryGroupDao _categoryGroupDao;
        private readonly IRepository<PS_SIS_LMS_CLASS_V> _repositoryPS_SIS_LMS_CLASS_V;

        public PeerFeedbackService(IRepository<PeerFeedbackRatingQuestion> repositoryPeerFeedbackRatingQuestion,
            IRepository<PeerFeedbackRatingOption> repositoryPeerFeedbackRatingOption,
            IRepository<PeerFeedbackQuestion> repositoryPeerFeedbackQuestion,
            IRepository<PeerFeedbackQuestionRatingMap> repositoryPeerFeedbackQuestionRatingMap,
            IRepository<PeerFeedback> repositoryPeerFeedback,
            IRepository<PeerFeedbackSessions> repositoryPeerFeedbackSessions,
            IRepository<PeerFeedbackEvaluators> repositoryPeerFeedbackEvaluators,
            IRepository<PeerFeedbackPairings> repositoryPeerFeedbackPairings,
            IRepository<PeerFeedbackTargets> repositoryPeerFeedbackTargets,
            IRepository<PeerFeedbackQuestionMap> repositoryPeerFeedbackQuestionMap,
            IRepository<UserEnrollment> userEnrollmentRepository,
            IRepository<User> userRepository,
            IRepository<UserGroup> userGroupRepository,
            IRepository<CategoryGroup> categoryGroupRepository,
            IRepository<eLearnApps.Entity.LmsTools.CourseCategory> courseCategoryRepository,
            IRepository<Course> repositoryCourse,
            IRepository<PeerFeedBackPairingSessions> repositoryPeerFeedBackPairingSessions,
            IDbContext context,
            IRepository<UserGroup> repositoryUserGroup,
            IRepository<PeerFeedBackResponses> repositoryPeerFeedBackResponses,
            IRepository<PeerFeedBackResponseRemarks> repositoryPeerFeedBackResponseRemarks,
            IRepository<Role> repositoryRoles,
               IDaoFactory factory,
                 LMSIsisContext isisContext
        )
        {
            _repositoryPeerFeedbackRatingQuestion = repositoryPeerFeedbackRatingQuestion;
            _repositoryPeerFeedbackRatingOption = repositoryPeerFeedbackRatingOption;
            _repositoryPeerFeedbackQuestion = repositoryPeerFeedbackQuestion;
            _repositoryPeerFeedbackQuestionRatingMap = repositoryPeerFeedbackQuestionRatingMap;

            _repositoryPeerFeedback = repositoryPeerFeedback;
            _repositoryPeerFeedbackEvaluators = repositoryPeerFeedbackEvaluators;
            _repositoryPeerFeedbackPairings = repositoryPeerFeedbackPairings;
            _repositoryPeerFeedbackSessions = repositoryPeerFeedbackSessions;
            _repositoryPeerFeedbackTargets = repositoryPeerFeedbackTargets;
            _repositoryPeerFeedbackQuestionMap = repositoryPeerFeedbackQuestionMap;

            _userEnrollmentRepository = userEnrollmentRepository;
            _userRepository = userRepository;
            _userGroupRepository = userGroupRepository;
            _categoryGroupRepository = categoryGroupRepository;
            _courseCategoryRepository = courseCategoryRepository;
            _repositoryPeerFeedBackPairingSessions = repositoryPeerFeedBackPairingSessions;
            _repositoryPeerFeedBackResponses = repositoryPeerFeedBackResponses;
            _repositoryPeerFeedBackResponseRemarks = repositoryPeerFeedBackResponseRemarks;
            _repositoryUserGroup = repositoryUserGroup;
            _repositoryRoles = repositoryRoles;
            _context = context;

            _isisContext = isisContext;
            _repositoryTlCourseOfferings = new Repository<TL_CourseOfferings>(_isisContext);
            _repositoryCourse = repositoryCourse;
            _repositoryPS_SIS_LMS_CLASS_V = new Repository<PS_SIS_LMS_CLASS_V>(_isisContext);

            _peerFeedBackResponsesDao = factory.PeerFeedBackResponsesDao;
            _peerFeedBackResponseRemarksDao = factory.PeerFeedBackResponseRemarksDao;
            _tLCourseOfferingDao = factory.TLCourseOfferingDao;
            _peerFeedBackDao = factory.PeerFeedBackDao;
            _categoryGroupDao = factory.CategoryGroupDao;
        }

        #region RATING

        public void InsertPeerFeedbackRatingQuestion(PeerFeedbackRatingQuestion feedbackRatingQuestion)
        {
            _repositoryPeerFeedbackRatingQuestion.Insert(feedbackRatingQuestion);
        }

        public void UpdatePeerFeedbackRatingQuestion(PeerFeedbackRatingQuestion feedbackRatingQuestion)
        {
            _repositoryPeerFeedbackRatingQuestion.Update(feedbackRatingQuestion);
        }

        public void DeletePeerFeedbackRatingQuestion(PeerFeedbackRatingQuestion feedbackRatingQuestion)
        {
            _repositoryPeerFeedbackRatingQuestion.Delete(feedbackRatingQuestion);
        }

        public PeerFeedbackRatingQuestion GetPeerFeedbackRatingQuestionById(int id)
        {
            var question = _repositoryPeerFeedbackRatingQuestion.Table
                .FirstOrDefault(x => x.Id == id && x.Deleted == false);
            return question;
        }

        public List<PeerFeedbackRatingQuestion> GetListPeerFeedbackRatingQuestions()
        {
            var result = _repositoryPeerFeedbackRatingQuestion.Table.OrderBy(x => x.DisplayOrder).ToList();
            return result;
        }

        public List<PeerFeedbackRatingQuestion> GetListPeerFeedbackRatingQuestionsByQuestionId(int questionId)
        {
            var query = from a in _repositoryPeerFeedbackRatingQuestion.TableNoTracking.Where(x => x.Deleted == false)
                        join b in _repositoryPeerFeedbackQuestionRatingMap.TableNoTracking on a.Id equals b.RatingQuestionId
                        where b.QuestionId == questionId
                        select a;
            return query.DistinctBy(x => x.Id).OrderBy(x => x.DisplayOrder).ToList();
        }

        public void InsertPeerFeedbackRatingOption(PeerFeedbackRatingOption feedbackRatingOption)
        {
            _repositoryPeerFeedbackRatingOption.Insert(feedbackRatingOption);
        }

        public void UpdatePeerFeedbackRatingOption(PeerFeedbackRatingOption feedbackRatingOption)
        {
            _repositoryPeerFeedbackRatingOption.Update(feedbackRatingOption);
        }

        public void DeletePeerFeedbackRatingOption(PeerFeedbackRatingOption feedbackRatingOption)
        {
            _repositoryPeerFeedbackRatingOption.Delete(feedbackRatingOption);
        }

        public PeerFeedbackRatingOption GetPeerFeedbackRatingOptionById(int id)
        {
            var option = _repositoryPeerFeedbackRatingOption.Table
                .FirstOrDefault(x => x.Id == id && x.Deleted == false);
            return option;
        }

        public List<PeerFeedbackRatingOption> GetListPeerFeedbackRatingOptions()
        {
            var result = _repositoryPeerFeedbackRatingOption.TableNoTracking.Where(x => x.Deleted == false).ToList();
            return result;
        }

        public List<PeerFeedbackRatingOptionDto> GetListPeerFeedbackRatingOptionsByQuestionId(int questionId,
            int ratingQuestionId)
        {
            var query = from a in _repositoryPeerFeedbackRatingOption.TableNoTracking.Where(x => x.Deleted == false)
                        join b in _repositoryPeerFeedbackQuestionRatingMap.TableNoTracking on a.Id equals b.RatingOptionId
                        where b.QuestionId == questionId && b.RatingQuestionId == ratingQuestionId
                        select new PeerFeedbackRatingOptionDto
                        {
                            OptionName = a.Name,
                            RatingOptionId = a.Id,
                            QuestionId = b.QuestionId,
                            RatingQuestionId = b.RatingQuestionId
                        };
            return query.DistinctBy(x => x.RatingOptionId).ToList();
        }

        public List<Instructor> GetInstructorName(List<int> courseIds)
        {
            var roleId = _repositoryRoles.Table.FirstOrDefault(x => x.Name.ToLower() == RoleName.INSTRUCTOR).Id;

            var query = from d in _userEnrollmentRepository.Table
                        join c in _userRepository.Table on d.UserId equals c.Id
                        join s in courseIds on d.CourseId equals s
                        where d.RoleId == roleId
                        select new Instructor
                        {
                            CourseId = d.CourseId,
                            Name = c.DisplayName
                        };
            var instructorNames = query.ToList();
            List<Instructor> result = new List<Instructor>();
            foreach (var course in courseIds)
            {
                result.Add(new Instructor
                {
                    CourseId = course,
                    Name = string.Join(" | ", instructorNames.Where(x => x.CourseId == course).Select(x => x.Name).ToList())
                });
            }

            return result;
        }

        public int GetCourseId(int peerFeedbackGroupId)
        {
            return _categoryGroupRepository.TableNoTracking.Where(x => x.Id == peerFeedbackGroupId).Join(
                _courseCategoryRepository.TableNoTracking,
                g => g.CourseCategoryId, c => c.Id, (g, c) => c.CourseId).FirstOrDefault() ?? default(int);
        }
        #endregion

        #region QUESTION
        public void InsertPeerFeedbackQuestion(PeerFeedbackQuestion feedbackQuestion)
        {
            _repositoryPeerFeedbackQuestion.Insert(feedbackQuestion);
        }

        public void UpdatePeerFeedbackQuestion(PeerFeedbackQuestion feedbackQuestion)
        {
            _repositoryPeerFeedbackQuestion.Update(feedbackQuestion);
        }

        public void DeletePeerFeedbackQuestion(PeerFeedbackQuestion feedbackQuestion)
        {
            _repositoryPeerFeedbackQuestion.Delete(feedbackQuestion);
        }

        public PeerFeedbackQuestion GetPeerFeedbackQuestionById(int id)
        {
            return _repositoryPeerFeedbackQuestion.GetById(id);
        }

        public List<PeerFeedbackQuestion> GetListPeerFeedbackQuestions()
        {
            return _repositoryPeerFeedbackQuestion.TableNoTracking.Where(x => x.Deleted == false).ToList();
        }

        public void InsertPeerFeedbackQuestionRatingMap(PeerFeedbackQuestionRatingMap feedbackQuestionRatingMap)
        {
            _repositoryPeerFeedbackQuestionRatingMap.Insert(feedbackQuestionRatingMap);
        }
        #endregion

        #region QUESTION RATING MAP
        public void UpdatePeerFeedbackQuestionRatingMap(PeerFeedbackQuestionRatingMap feedbackQuestionRatingMap)
        {
            _repositoryPeerFeedbackQuestionRatingMap.Update(feedbackQuestionRatingMap);
        }

        public void DeletePeerFeedbackQuestionRatingMapByQuestionId(int questionId)
        {
            var result = _repositoryPeerFeedbackQuestionRatingMap.Table.Where(x => x.QuestionId == questionId).ToList();
            if (result.Count > 0) _repositoryPeerFeedbackQuestionRatingMap.Delete(result);
        }
        public void DeletePeerFeedbackQuestionRatingMapByRatingQuestionId(int questionId, int ratingQuestionId)
        {
            var result = _repositoryPeerFeedbackQuestionRatingMap.Table
                .Where(x => x.QuestionId == questionId && x.RatingQuestionId == ratingQuestionId).ToList();
            if (result.Count > 0) _repositoryPeerFeedbackQuestionRatingMap.Delete(result);
        }
        public PeerFeedbackQuestionRatingMap GetPeerFeedbackQuestionRatingMapById(int id)
        {
            return _repositoryPeerFeedbackQuestionRatingMap.GetById(id);
        }
        public List<PeerFeedbackQuestionRatingMap> GetFeedbackQuestionRatingMapByQuestionId(int questionId)
        {
            return _repositoryPeerFeedbackQuestionRatingMap.TableNoTracking.Where(x => x.QuestionId == questionId)
                .ToList();
        }
        public List<PeerFeedbackQuestionRatingMap> GetFeedbackQuestionRatingMapList()
        {
            return _repositoryPeerFeedbackQuestionRatingMap.TableNoTracking.ToList();
        }
        public List<PeerFeedbackRatingQuestionMapOptionDto> GetListPeerFeedbackRatingQuestionsWithOptions(
            int questionId)
        {
            var result = new List<PeerFeedbackRatingQuestionMapOptionDto>();
            var mapping =
                _repositoryPeerFeedbackQuestionRatingMap.TableNoTracking.Where(x => x.QuestionId == questionId);
            var options = _repositoryPeerFeedbackRatingOption.TableNoTracking.Where(x => x.Deleted == false);
            var optionsMap = (from a in options
                              join b in mapping on a.Id equals b.RatingOptionId
                              select new PeerFeedbackRatingOptionDto
                              {
                                  OptionName = a.Name,
                                  RatingOptionId = a.Id,
                                  RatingQuestionId = b.RatingQuestionId
                              }).ToList();
            var query = _repositoryPeerFeedbackRatingQuestion.TableNoTracking.Where(x => x.Deleted == false)
                .ToList();
            foreach (var item in query)
            {
                var map = optionsMap.Where(x => x.RatingQuestionId == item.Id).ToList();
                var dto = new PeerFeedbackRatingQuestionMapOptionDto
                {
                    Name = item.Name,
                    Id = item.Id,
                    Options = map
                };
                result.Add(dto);
            }

            return result;
        }

        #endregion

        #region PEER_FEEDBACK
        public void PeerFeedbackInsert(PeerFeedback peerFeedback)
        {
            _repositoryPeerFeedback.Insert(peerFeedback);
        }

        public void PeerFeedbackUpdate(PeerFeedback peerFeedback)
        {
            _repositoryPeerFeedback.Update(peerFeedback);
        }

        public void PeerFeedbackDelete(PeerFeedback peerFeedback)
        {
            peerFeedback.IsDeleted = true;
            _repositoryPeerFeedback.Update(peerFeedback);
        }
        /// <summary>
        /// Delete PeerFeedBack (with sql query)
        /// will update delete flag in tables
        /// 1, PeerFeedback 
        /// 2, PeerFeedBackResponses
        /// 3, PeerFeedbackPairings
        /// 4, PeerFeedbackSessions
        /// 5, PeerFeedbackEvaluators
        /// 6, PeerFeedbackTargets
        /// </summary>
        /// <param name="peerFeedBackId"></param>
        /// <param name="lastUpdatedBy"></param>
        public void PeerFeedbackDelete(int peerFeedBackId, int lastUpdatedBy)
        {
            _peerFeedBackDao.Delete(peerFeedBackId, lastUpdatedBy);
        }
        /// <summary>
        /// Delete PeerFeedBack(using EF & Linq)
        /// will update delete flag in tables
        /// 1, PeerFeedback 
        /// 2, PeerFeedBackResponses
        /// 3, PeerFeedbackPairings
        /// 4, PeerFeedbackSessions
        /// 5, PeerFeedbackEvaluators
        /// 6, PeerFeedbackTargets
        /// </summary>
        /// <param name="peerfeedbackId"></param>
        /// <param name="updatedBy"></param>
        /// <param name="updatedTime"></param>
        public void PeerFeedbackDelete(int peerfeedbackId, int updatedBy, DateTime updatedTime)
        {
            if (peerfeedbackId < 0)
                throw new ApplicationException("Invalid peerfeedbackId parameter used.");

            var peerfeedbackToDelete = PeerFeedbackGetById(peerfeedbackId);
            if (peerfeedbackToDelete == null)
                throw new ApplicationException($"No such peerfeedback specified by peerfeedbackId:{peerfeedbackId}");

            using (var transaction = _context.BeginTransaction())
            {
                peerfeedbackToDelete.IsDeleted = true;
                peerfeedbackToDelete.LastUpdatedBy = updatedBy;
                peerfeedbackToDelete.LastUpdatedTime = updatedTime;
                _repositoryPeerFeedback.Update(peerfeedbackToDelete);

                // delete sessions
                var peerfeedbackSessions = PeerFeedbackSessionsGetByPeerFeedbackId(peerfeedbackId);
                foreach (var session in peerfeedbackSessions)
                {
                    session.IsDeleted = true;
                    session.LastUpdatedBy = updatedBy;
                    session.LastUpdatedTime = updatedTime;
                    _repositoryPeerFeedbackSessions.Update(session);
                }

                // delete pairing, evaluator and target
                var peerfeedbackPairings = PeerFeedbackPairingsGetByPeerFeedBackId(peerfeedbackId);
                foreach (var pairing in peerfeedbackPairings)
                {
                    pairing.IsDeleted = true;
                    pairing.LastUpdatedBy = updatedBy;
                    pairing.LastUpdatedTime = updatedTime;
                    _repositoryPeerFeedbackPairings.Update(pairing);

                    // delete target
                    var peerfeedbackTargets = _repositoryPeerFeedbackTargets
                        .Table.Where(r => r.PeerFeedbackPairingId == pairing.Id).ToList();
                    foreach (var target in peerfeedbackTargets)
                    {
                        target.IsDeleted = true;
                        target.LastUpdatedBy = updatedBy;
                        target.LastUpdatedTime = updatedTime;
                        _repositoryPeerFeedbackTargets.Update(target);
                    }

                    // delete evaluator
                    var peerfeedbackEvaluators = _repositoryPeerFeedbackEvaluators.Table
                        .Where(r => r.PeerFeedbackPairingId == pairing.Id).ToList();
                    foreach (var evaluator in peerfeedbackEvaluators)
                    {
                        evaluator.IsDeleted = true;
                        evaluator.LastUpdatedBy = updatedBy;
                        evaluator.LastUpdatedTime = updatedTime;
                        _repositoryPeerFeedbackEvaluators.Update(evaluator);
                    }
                }

                // delete responses
                var responses = _repositoryPeerFeedBackResponses.Table
                    .Where(r => r.PeerFeedbackId == peerfeedbackId).ToList();
                foreach (var response in responses)
                {
                    response.IsDeleted = true;
                    response.LastUpdateTime = updatedTime;
                    _repositoryPeerFeedBackResponses.Update(response);
                }

                transaction.Commit();
            }
        }

        public PeerFeedback PeerFeedbackGetById(int id)
        {
            return _repositoryPeerFeedback.GetById(id);
        }

        public List<PeerFeedback> PeerFeedbackGetList()
        {
            return _repositoryPeerFeedback.TableNoTracking.Where(x => x.IsDeleted == false).ToList();
        }

        private string GetCourseNameByCategoryId(int orgUnitId)
        {
            var course =
                (from a in _courseCategoryRepository.Table.Where(x => x.Id == orgUnitId)
                 join b in _repositoryCourse.Table on a.CourseId equals b.Id
                 select b).First();
            return course.Name;
        }

        public List<PeerFeedBackEvaluationDto> PeerFeedBackEvaluation()
        {
            var query = from a in _repositoryPeerFeedBackResponses.Table
                        join b in _categoryGroupRepository.Table on a.PeerFeedBackGroupId equals b.Id
                        join c in _repositoryPeerFeedBackPairingSessions.Table on a.PeerFeedbackSessionId equals c.PeerFeedBackSessionId
                        join d in _repositoryPeerFeedbackSessions.Table.Where(x => x.IsDeleted == false) on c.PeerFeedBackSessionId equals d.Id
                        join e in _courseCategoryRepository.Table on b.CourseCategoryId equals e.Id
                        join f in _repositoryCourse.Table on e.CourseId equals f.Id
                        select new PeerFeedBackEvaluationDto
                        {
                            Name = b.Name,
                            From = d.EntryStartTime,
                            To = d.EntryCloseTime,
                            PeerFeedBackId = d.PeerFeedbackId,
                            CourseName = f.Name,
                            PeerFeedBackPairingId = c.PeerFeedBackPairingId,
                            PeerFeedBackSessionId = d.Id,
                            PeerFeedBackGroupId = a.PeerFeedBackGroupId,
                            SessionName = d.Label
                        };
            return query.Distinct().ToList();
        }
        public List<PeerFeedBackEvaluationDto> PeerFeedBackEvaluationList(int userId)
        {
            var query = from a in _repositoryPeerFeedbackTargets.Table
                        join b in _categoryGroupRepository.Table on a.OrgUnitId equals b.Id
                        join c in _repositoryPeerFeedBackPairingSessions.Table on a.PeerFeedbackPairingId equals c
                            .PeerFeedBackPairingId
                        join d in _repositoryPeerFeedbackSessions.Table.Where(x => x.IsDeleted == false) on c.PeerFeedBackSessionId equals d.Id
                        join e in _courseCategoryRepository.Table on b.CourseCategoryId equals e.Id
                        join f in _repositoryCourse.Table on e.CourseId equals f.Id
                        join g in _repositoryUserGroup.Table.Where(x => x.UserId == userId) on b.Id equals g.CategoryGroupId
                        select new PeerFeedBackEvaluationDto
                        {
                            Name = b.Name,
                            From = d.EntryStartTime,
                            To = d.EntryCloseTime,
                            PeerFeedBackId = d.PeerFeedbackId,
                            CourseName = f.Name,
                            PeerFeedBackPairingId = a.PeerFeedbackPairingId,
                            PeerFeedBackSessionId = d.Id,
                            PeerFeedBackGroupId = a.OrgUnitId,
                            SessionName = d.Label,
                            CourseId = f.Id
                        };
            return query.Distinct().ToList();
        }
        public List<User> PeerFeedBackEvaluationDetail(int peerFeedBackSessionId, int peerFeedBackGroupId,
            int peerFeedBackPairingId, int userId)
        {
            var query = from a in _repositoryPeerFeedbackTargets.Table
                        join b in _repositoryPeerFeedBackPairingSessions.Table on a.PeerFeedbackPairingId equals b
                            .PeerFeedBackPairingId
                        join c in _userRepository.Table on a.UserId equals c.Id
                        join d in _userEnrollmentRepository.Table on c.Id equals d.UserId
                        join e in _categoryGroupRepository.Table on a.OrgUnitId equals e.Id
                        join f in _repositoryUserGroup.Table.Where(x => x.UserId == userId) on e.Id equals f.CategoryGroupId
                        select c;
            return query.Distinct().ToList();
        }

        public List<User> GetTargetByPairingId(int peerfeedbackPairingId)
        {
            var query = from pairing in _repositoryPeerFeedbackPairings.TableNoTracking
                        join target in _repositoryPeerFeedbackTargets.TableNoTracking on pairing.Id equals target.PeerFeedbackPairingId
                        join user in _userRepository.TableNoTracking on target.UserId equals user.Id
                        where pairing.Id == peerfeedbackPairingId
                        select user;

            return query.Distinct().ToList();
        }
        /// <summary>
        /// Get data dashboard filter
        /// </summary>
        /// <param name="strm"></param>
        /// <param name="whitelistedCourses"></param>
        /// <returns></returns>
        public List<DashboardFilterDto> PeerFeedBackGetDashboardFilter(string strm)
        {

            var coursesQuery = _repositoryCourse.TableNoTracking;
            var courses = coursesQuery.ToList();

            var isisCoursesInSelectedTerm = _tLCourseOfferingDao.GetPeerFeedBackDashboardData(strm);
            var data = from isisCourse in isisCoursesInSelectedTerm
                       join course in courses on isisCourse.CourseOfferingCode equals course.Code
                       select new DashboardFilterDto
                       {
                           CourseId = course.Id,
                           CLASS_NBR = isisCourse.CLASS_NBR,
                           ACADEMIC_YEAR = isisCourse.ACADEMIC_YEAR,
                           ACAD_GROUP = isisCourse.ACAD_GROUP,
                           CourseOfferingCode = isisCourse.CourseOfferingCode,
                           STRM = isisCourse.STRM,
                           ACADEMIC_TERM = isisCourse.ACADEMIC_TERM,
                           CLASS_SECTION = isisCourse.CLASS_SECTION
                       };
            return data.ToList();
        }
        /// <summary>
        /// Get data for export csv
        /// </summary>
        /// <param name="strm"></param>
        /// <param name="whitelistedCourses"></param>
        /// <returns></returns>
        public List<DashboardFilterDto> PeerFeedBackDownloadCsv(string strm)
        {
            var coursesQuery = _repositoryCourse.TableNoTracking;
            var courses = coursesQuery.ToList();

            var isisCoursesInSelectedTerm = _tLCourseOfferingDao.GetPeerFeedBackCourseOfferingCsv(strm);
            var data = from isisCourse in isisCoursesInSelectedTerm
                       join course in courses on isisCourse.CourseOfferingCode equals course.Code
                       select new DashboardFilterDto
                       {
                           CourseId = course.Id,
                           CLASS_NBR = isisCourse.CLASS_NBR,
                           ACADEMIC_YEAR = isisCourse.ACADEMIC_YEAR,
                           ACAD_GROUP = isisCourse.ACAD_GROUP,
                           CourseOfferingCode = isisCourse.CourseOfferingCode,
                           STRM = isisCourse.STRM,
                           ACADEMIC_TERM = isisCourse.ACADEMIC_TERM,
                           CLASS_SECTION = isisCourse.CLASS_SECTION
                       };
            return data.ToList();
        }
        #endregion

        #region PEER_FEEDBACK_EVALUATORS
        public void PeerFeedbackEvaluatorsInsert(PeerFeedbackEvaluators peerFeedbackEvaluators)
        {
            _repositoryPeerFeedbackEvaluators.Insert(peerFeedbackEvaluators);
        }

        public void PeerFeedbackEvaluatorsUpdate(PeerFeedbackEvaluators peerFeedbackEvaluators)
        {
            _repositoryPeerFeedbackEvaluators.Update(peerFeedbackEvaluators);
        }

        public void PeerFeedbackEvaluatorsDelete(PeerFeedbackEvaluators peerFeedbackEvaluators)
        {
            peerFeedbackEvaluators.IsDeleted = true;
            _repositoryPeerFeedbackEvaluators.Update(peerFeedbackEvaluators);
        }

        public PeerFeedbackEvaluators PeerFeedbackEvaluatorsGetById(int id)
        {
            return _repositoryPeerFeedbackEvaluators.GetById(id); ;
        }
        public List<ItemDto> GetEvaluators(int evaluationId, int courseId)
        {
            var evaluation = _repositoryPeerFeedback.Table.FirstOrDefault(x => x.Id == evaluationId && x.IsDeleted == false);

            var queryUserEnrollments = _userEnrollmentRepository.Table;
            var queryUsers = _userRepository.Table;
            var queryUserGroups = _userGroupRepository.Table;

            var queryGroups = from categoryGroup in _categoryGroupRepository.Table
                              join courseCategory in _courseCategoryRepository.Table on categoryGroup.CourseCategoryId equals courseCategory.Id
                              where courseCategory.CourseId == courseId
                              select categoryGroup;

            IQueryable<ItemDto> query;
            List<ItemDto> resultList = new List<ItemDto>();
            //ADD CODE HERE
            resultList = resultList.OrderBy(item => item.Name).ToList();
            return resultList;
        }

        #endregion

        #region PEER_FEEDBACK_PAIRINGS

        public void PeerFeedbackPairingsInsert(PeerFeedbackPairings peerFeedbackPairings)
        {
            _repositoryPeerFeedbackPairings.Insert(peerFeedbackPairings);
        }

        public void PeerFeedbackPairingsUpdate(PeerFeedbackPairings peerFeedbackPairings)
        {
            _repositoryPeerFeedbackPairings.Update(peerFeedbackPairings);
        }

        public void PeerFeedbackPairingsDelete(PeerFeedbackPairings peerFeedbackPairings)
        {
            peerFeedbackPairings.IsDeleted = true;
            _repositoryPeerFeedbackPairings.Update(peerFeedbackPairings);
        }

        public PeerFeedbackPairings PeerFeedbackPairingsGetById(int id)
        {
            return _repositoryPeerFeedbackPairings.GetById(id);
        }

        public List<PeerFeedbackPairings> PeerFeedbackPairingsGetByPeerFeedBackId(int peerFeedBackId)
        {
            var queryPairings = _repositoryPeerFeedbackPairings.Table;
            var query = from pairings in queryPairings
                        where pairings.PeerFeedbackId == peerFeedBackId
                        select pairings;
            return query.ToList();
        }
        /// <summary>
        /// Generate pairings along with evaluators and targets in specified session and courses
        /// </summary>
        /// <param name="peerFeedbackId"></param>
        /// <param name="peerFeedBackSessionId"></param>
        /// <param name="strm"></param>
        /// <param name="whiteListedCourses"></param>
        /// <param name="updatingUserId"></param>
        public void GeneratePairings(int peerFeedbackId, int peerFeedBackSessionId, string strm, int updatingUserId)
        {
            var courseOfferingCodesInSelectedTerm = _repositoryPeerFeedbackSessions.TableNoTracking.Where(r => r.Id == peerFeedBackSessionId).Select(r => r.CourseOfferingCode).ToList();
            var courseOfferingCode = new List<string>();
            foreach (var item in courseOfferingCodesInSelectedTerm)
            {
                courseOfferingCode.AddRange(item.Split(','));
            }
            _peerFeedBackDao.GeneratePairings(peerFeedbackId, peerFeedBackSessionId, courseOfferingCode, updatingUserId);
        }
        /// <summary>
        /// Generate pairings along with evaluators and targets in specified session and courses
        /// </summary>
        /// <param name="session"></param>
        /// <param name="whiteListedCourses"></param>
        /// <param name="updatingUserId"></param>
        public async Task GeneratePairings(PeerFeedbackSessions session, int updatingUserId)
        {
            var list = new List<(PeerFeedbackPairings Pairing, List<PeerFeedBackPairingSessions> PairingSession, List<PeerFeedbackTargets> Targets, List<PeerFeedbackEvaluators> Evaluators)>();
            var targetUserGroups = await GetTargetGroupsAsync(session);

            var groupIds = targetUserGroups.Select(x => x.CategoryGroupId).Distinct().ToList();
            foreach (var groupId in groupIds)
            {
                // create a pairing for each group
                var sessionPairings = new List<PeerFeedBackPairingSessions>();
                var evaluators = new List<PeerFeedbackEvaluators>();
                var targets = new List<PeerFeedbackTargets>();
                // create a new pairing for each group
                var pairing = new PeerFeedbackPairings
                {
                    PeerFeedbackId = session.PeerFeedbackId,
                    LastUpdatedBy = updatingUserId,
                };
                var dataItem = targetUserGroups.Where(x => x.CategoryGroupId == groupId).ToList();
                foreach (var item in dataItem)
                {
                    sessionPairings.Add(new PeerFeedBackPairingSessions
                    {
                        PeerFeedBackSessionId = session.Id
                    });
                    evaluators.Add(new PeerFeedbackEvaluators
                    {
                        UserId = item.UserId,
                        OrgUnitId = item.CategoryGroupId,
                        IsOrgUnit = false,
                        LastUpdatedBy = updatingUserId
                    });
                    targets.Add(new PeerFeedbackTargets
                    {
                        UserId = item.UserId,
                        OrgUnitId = item.CategoryGroupId,
                        IsOrgUnit = false,
                        LastUpdatedBy = updatingUserId
                    });
                }
                list.Add((pairing, sessionPairings, targets, evaluators));
            }
            if (list.Any())
                await _peerFeedBackDao.GeneratePairings(list);
        }
        public void GenStdEvalOwnGroupMemberPairingsForSession(int peerFeedBackSessionId, int updatingUserId)
        {
            using (var transaction = _context.BeginTransaction())
            {
                try
                {
                    var session = _repositoryPeerFeedbackSessions.GetById(peerFeedBackSessionId);
                    // session must exist
                    if (session == null)
                    {
                        throw new ApplicationException("Invalid peerfeedback session id parameter: {peerFeedBackSessionId}.");
                    }

                    var targetGroupIds = GetTargetGroups(session);

                    // evaluators and targets are group member of the same group
                    var userGroupQuery = from ug in _userGroupRepository.Table.Where(ug => targetGroupIds.Contains(ug.CategoryGroupId.Value))
                                         join user in _userRepository.TableNoTracking on ug.UserId equals user.Id
                                         select ug;

                    var targetUserGroups = userGroupQuery.Select(ug => new ItemDto
                    {
                        UserId = ug.UserId.Value,
                        GroupId = ug.CategoryGroupId.Value
                    }).GroupBy(item => item.GroupId);

                    // create a pairing for each group
                    var evaluationPairingEntities = new List<PeerFeedbackPairings>();
                    foreach (var targetUserGroup in targetUserGroups)
                    {
                        // create a new pairing for each group
                        evaluationPairingEntities.Add(new PeerFeedbackPairings
                        {
                            PeerFeedbackId = session.PeerFeedbackId,
                            IsDeleted = false,
                            LastUpdatedBy = updatingUserId,
                            LastUpdatedTime = DateTime.UtcNow,
                        });
                    }
                    _repositoryPeerFeedbackPairings.Insert(evaluationPairingEntities);

                    // claim 1 pair for each group and assigned it to new evaluators and targets 
                    int idx = 0;
                    var sessionPairings = new List<PeerFeedBackPairingSessions>();
                    var evaluators = new List<PeerFeedbackEvaluators>();
                    var targets = new List<PeerFeedbackTargets>();
                    foreach (var targetGroup in targetUserGroups)
                    {
                        var pairingId = evaluationPairingEntities[idx].Id;
                        sessionPairings.Add(new PeerFeedBackPairingSessions()
                        {
                            PeerFeedBackPairingId = pairingId,
                            PeerFeedBackSessionId = session.Id
                        });
                        evaluators.AddRange(targetGroup.Select(item => new PeerFeedbackEvaluators()
                        {
                            PeerFeedbackPairingId = pairingId,
                            UserId = item.UserId,
                            OrgUnitId = item.GroupId,
                            IsOrgUnit = false,
                            IsDeleted = false,
                            LastUpdatedBy = updatingUserId,
                            LastUpdatedTime = DateTime.UtcNow
                        }));
                        targets.AddRange(targetGroup.Select(item => new PeerFeedbackTargets()
                        {
                            PeerFeedbackPairingId = pairingId,
                            UserId = item.UserId,
                            OrgUnitId = item.GroupId,
                            IsOrgUnit = false,
                            IsDeleted = false,
                            LastUpdatedBy = updatingUserId,
                            LastUpdatedTime = DateTime.UtcNow
                        }));
                        idx++;
                    }
                    _repositoryPeerFeedBackPairingSessions.Insert(sessionPairings);
                    _repositoryPeerFeedbackEvaluators.Insert(evaluators);
                    _repositoryPeerFeedbackTargets.Insert(targets);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new ApplicationException("Exception occurs while creating pairings for peerfeedback. ex: {ex}", ex);
                }
            }
        }

        private List<int> GetTargetGroups(PeerFeedbackSessions session)
        {
            var listTargets = new List<int>();
            var courseOfferingCodesInSelectedTerm = session.CourseOfferingCode.Split(',').ToList();
            var query = from coursecategory in _courseCategoryRepository.TableNoTracking
                        join categorygroup in _categoryGroupRepository.TableNoTracking on coursecategory.Id equals categorygroup.CourseCategoryId
                        join course in _repositoryCourse.TableNoTracking.Where(x => courseOfferingCodesInSelectedTerm.Contains(x.Code)) on coursecategory.CourseId equals course.Id
                        select categorygroup.Id;
            return query.ToList();
        }
        private async Task<List<UserCategoryGroupDto>> GetTargetGroupsAsync(PeerFeedbackSessions session)
        {
            try
            {
                var result = await _categoryGroupDao.GetUserCategoryGroupAsync(session.CourseOfferingCode.Split(',').ToList());
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public PeerFeedbackPairings PeerFeedbackPairingsSave(int evaluationId, int peerFeedBackSessionId, int evaluationPairingId, int userId, List<int> evaluatorIds, List<int> targetIds)
        {
            using (var transaction = _context.BeginTransaction())
            {
                try
                {
                    var evaluationEntity = _repositoryPeerFeedback.GetById(evaluationId);

                    // create pairing container if needed, else update pairing info
                    PeerFeedbackPairings evaluationPairingEntity;
                    if (evaluationPairingId > 0)
                    {
                        evaluationPairingEntity = _repositoryPeerFeedbackPairings.GetById(evaluationPairingId);
                        if (evaluationPairingEntity != null)
                        {
                            evaluationPairingEntity.PeerFeedbackId = evaluationId;
                            evaluationPairingEntity.IsDeleted = false;
                            evaluationPairingEntity.LastUpdatedBy = userId;
                            evaluationPairingEntity.LastUpdatedTime = DateTime.UtcNow;
                            _repositoryPeerFeedbackPairings.Update(evaluationPairingEntity);
                        }
                    }
                    else
                    {
                        evaluationPairingEntity = new PeerFeedbackPairings
                        {
                            PeerFeedbackId = evaluationId,
                            IsDeleted = false,
                            LastUpdatedBy = userId,
                            LastUpdatedTime = DateTime.UtcNow,
                        };
                        _repositoryPeerFeedbackPairings.Insert(evaluationPairingEntity);
                    }

                    var session = _repositoryPeerFeedbackSessions.GetById(peerFeedBackSessionId);
                    if (session != null)
                    {
                        var sessionPairings = _repositoryPeerFeedBackPairingSessions.Table.FirstOrDefault(x =>
                            x.PeerFeedBackPairingId == evaluationPairingId &&
                            x.PeerFeedBackSessionId == peerFeedBackSessionId);
                        if (sessionPairings != null)
                        {
                            _repositoryPeerFeedBackPairingSessions.Delete(sessionPairings);
                        }
                        _repositoryPeerFeedBackPairingSessions.Insert(new PeerFeedBackPairingSessions
                        {
                            PeerFeedBackPairingId = evaluationPairingEntity.Id,
                            PeerFeedBackSessionId = peerFeedBackSessionId
                        });
                    }
                    SaveEvaluatorAndTarget(evaluationEntity, evaluationPairingEntity, evaluatorIds, targetIds, userId);

                    transaction.Commit();

                    return evaluationPairingEntity;
                }
                catch
                {
                    transaction.Rollback();
                }
            }
            return new PeerFeedbackPairings();
        }
        private void SaveEvaluatorAndTarget(PeerFeedback evaluationEntity, PeerFeedbackPairings evaluationPairingEntity, List<int> evaluatorIds, IEnumerable<int> targetIds, int userId)
        {
            // most case evaluator is a singular person where GroupId is undetermined.
            // but for student evaluate own group member, we need to show that all student belongs to same group
            var evaluators = new List<ItemDto>();

            if (evaluationEntity.TypeId == (int)Core.PeerFeedbackType.StudentsEvaluateOwnGroupMembers)
            {
                // evaluators and targets are group member of the same group
                evaluators = _userGroupRepository.Table.Where(ug => targetIds.Contains(ug.CategoryGroupId.Value)).Select(ug => new ItemDto
                {
                    UserId = ug.UserId.Value,
                    GroupId = ug.CategoryGroupId.Value
                }).ToList();
            }

            // Delete existing evauators for this pairing and insert new one
            _context.ExecuteSqlCommand($"DELETE FROM PeerFeedbackEvaluators WHERE PeerFeedbackPairingId = {evaluationPairingEntity.Id}");
            switch (evaluationEntity.TypeId)
            {
                case (int)Core.EvaluationType.StudentsEvaluateOwnGroupMembers:
                    _repositoryPeerFeedbackEvaluators.Insert(evaluators.Select(evaluator => new PeerFeedbackEvaluators
                    {
                        PeerFeedbackPairingId = evaluationPairingEntity.Id,
                        UserId = evaluator.UserId,
                        OrgUnitId = evaluator.GroupId,
                        IsOrgUnit = false,
                        IsDeleted = false,
                        LastUpdatedBy = userId,
                        LastUpdatedTime = DateTime.UtcNow
                    }));
                    break;
            }

            // Delete existing targets for this pairing and insert new one
            _context.ExecuteSqlCommand("DELETE FROM PeerFeedbackTargets WHERE PeerFeedbackPairingId = " + evaluationPairingEntity.Id);
            // predetermine evaluation type whose targets are groups (not individual)
            switch (evaluationEntity.TypeId)
            {
                case (int)Core.EvaluationType.StudentsEvaluateOwnGroupMembers:
                    // here there should only be same course group
                    _repositoryPeerFeedbackTargets.Insert(evaluators.Select(target => new PeerFeedbackTargets
                    {
                        PeerFeedbackPairingId = evaluationPairingEntity.Id,
                        UserId = target.UserId,
                        OrgUnitId = target.GroupId,
                        IsOrgUnit = false,
                        IsDeleted = false,
                        LastUpdatedBy = userId,
                        LastUpdatedTime = DateTime.UtcNow
                    }));
                    break;
            }
        }
        #endregion

        #region PEER_FEEDBACK_SESSIONS
        public void PeerFeedbackSessionsInsert(PeerFeedbackSessions peerFeedbackSessions)
        {
            _repositoryPeerFeedbackSessions.Insert(peerFeedbackSessions);
        }

        public void PeerFeedbackSessionsUpdate(PeerFeedbackSessions peerFeedbackSessions)
        {
            _repositoryPeerFeedbackSessions.Update(peerFeedbackSessions);
        }

        public void PeerFeedbackSessionsDelete(PeerFeedbackSessions peerFeedbackSessions)
        {
            peerFeedbackSessions.IsDeleted = true;
            _repositoryPeerFeedbackSessions.Update(peerFeedbackSessions);
        }
        public List<PeerFeedbackSessions> PeerFeedBackSessionGetListSessionIdByStrm(string strm)
        {
            if (string.IsNullOrEmpty(strm))
                throw new ArgumentNullException(nameof(strm));

            return _repositoryPeerFeedbackSessions.Table.Where(x => x.EntryCloseTime < DateTime.UtcNow && x.Strm == strm).ToList();
        }
        public List<PeerFeedbackSessions> PeerFeedBackSessionGetListSessionIdByListStrm(List<string> listStrm)
        {
            var query = _repositoryPeerFeedbackSessions.Table.Where(x => x.IsDeleted == false && x.EntryCloseTime < DateTime.UtcNow);
            if (listStrm != null && listStrm.Any())
            {
                query = query.Where(x => listStrm.Contains(x.Strm));
            }
            return query.ToList();
        }

        public List<PeerFeedbackSessions> PeerFeedBackSessionGetListSessionIdFiltered(List<string> listStrm, string school, List<int> courses)
        {
            var query = (from courseCate in _courseCategoryRepository.TableNoTracking
                         join course in courses on courseCate.CourseId equals course
                         join cateGroup in _categoryGroupRepository.TableNoTracking on courseCate.Id equals cateGroup.CourseCategoryId
                         join target in _repositoryPeerFeedbackTargets.TableNoTracking on cateGroup.Id equals target.OrgUnitId
                         join pairingsession in _repositoryPeerFeedBackPairingSessions.TableNoTracking on target.PeerFeedbackPairingId equals pairingsession.PeerFeedBackPairingId
                         join session in _repositoryPeerFeedbackSessions.TableNoTracking on pairingsession.PeerFeedBackSessionId equals session.Id
                         join strms in listStrm on session.Strm equals strms
                         select session)
                        .Where(x => x.EntryCloseTime < DateTime.UtcNow)
                        .DistinctBy(session => session.Id);
            return query.ToList();
        }

        public PeerFeedbackSessions PeerFeedbackSessionsGetById(int id)
        {
            return _repositoryPeerFeedbackSessions.GetById(id);
        }
        public bool PeerFeedbackSessionsCheckByCondition(PeerFeedbackSessions session)
        {
            return _repositoryPeerFeedbackSessions.Table.Any(x =>
                        x.IsDeleted == false &&
                        x.Strm == session.Strm &&
                        x.PeerFeedbackId == session.PeerFeedbackId &&
                        (session.Id > 0 ? x.Id != session.Id : true) &&
                        ((session.EntryStartTime >= x.EntryStartTime && session.EntryCloseTime <= x.EntryCloseTime)
                            || (session.EntryCloseTime >= x.EntryStartTime && session.EntryCloseTime <= x.EntryCloseTime)
                            || (x.EntryStartTime >= session.EntryStartTime && x.EntryStartTime <= session.EntryCloseTime)
                            || (session.EntryStartTime >= x.EntryStartTime && session.EntryStartTime <= x.EntryCloseTime)
                            || (session.EntryCloseTime >= x.EntryStartTime && session.EntryCloseTime <= x.EntryCloseTime))
                        );
        }
        public List<PeerFeedbackSessions> PeerFeedbackSessionsGetByPeerFeedbackId(int peerFeedbackId)
        {
            return _repositoryPeerFeedbackSessions.Table.Where(x => x.PeerFeedbackId == peerFeedbackId && x.IsDeleted == false).ToList();
        }

        // this is used in Export to file, so may need the full range of terms
        public List<TextValue> PeerFeedbackGetWhitelistedTerm()
        {
            var whitelistedCourseCodes = _repositoryCourse.TableNoTracking.Select(x => x.Code.ToLower()).Distinct().ToList();

            var data = _repositoryTlCourseOfferings.TableNoTracking.Select(courseOffering => new CourseOfferingDto
            {
                STRM = courseOffering.STRM,
                ACADEMIC_YEAR = courseOffering.ACADEMIC_YEAR,
                ACADEMIC_TERM = courseOffering.ACADEMIC_TERM,
                CourseOfferingCode = courseOffering.CourseOfferingCode
            }).ToList();

            var result = data.Where(co => whitelistedCourseCodes.Contains(co.CourseOfferingCode.ToLower())).OrderBy(x => x.ACADEMIC_YEAR).ThenBy(x => x.CourseOfferingCode).ToList();
            var response = result.GroupBy(x => x.STRM, (key, group) =>
            {
                var item = result.FirstOrDefault(t => t.STRM == key);
                return new TextValue
                {
                    Value = item.STRM,
                    Text = $"{item.ACADEMIC_YEAR} T{item.ACADEMIC_TERM}",
                    Items = group.Select(g => new TextValue
                    {
                        Value = g.CourseOfferingCode,
                        Text = g.CourseOfferingCode
                    }).Distinct().ToList()
                };
            }).ToList();
            return response;
        }

        // this is for UI hence limit
        //
        public List<TextValue> PeerFeedbackGetWhitelistedTerm(AcadCareer acadCareer = AcadCareer.Both, int takeRecentTerms = 25)
        {
            var whitelistedCourseCodes = _repositoryCourse.TableNoTracking.Select(x => x.Code.ToLower()).Distinct().ToList();

            var data = new List<CourseOfferingDto>();

            if (acadCareer == AcadCareer.Both)
            {
                data = _repositoryTlCourseOfferings.TableNoTracking.Select(courseOffering => new CourseOfferingDto
                {
                    STRM = courseOffering.STRM,
                    ACADEMIC_YEAR = courseOffering.ACADEMIC_YEAR,
                    ACADEMIC_TERM = courseOffering.ACADEMIC_TERM,
                    CourseOfferingCode = courseOffering.CourseOfferingCode
                }).ToList();
            }
            else
            {
                // this is specific to LMSISIS.TL_CourseOfferings.ACAD_CAREER.
                // UGRD = Undergrad
                // Other values = Postgrad
                string _acadCareer = "UGRD";

                if (acadCareer == AcadCareer.UG)
                {
                    data = _repositoryTlCourseOfferings.TableNoTracking
                    .Where(x => x.ACAD_CAREER == _acadCareer)
                    .Select(courseOffering => new CourseOfferingDto
                    {
                        STRM = courseOffering.STRM,
                        ACADEMIC_YEAR = courseOffering.ACADEMIC_YEAR,
                        ACADEMIC_TERM = courseOffering.ACADEMIC_TERM,
                        CourseOfferingCode = courseOffering.CourseOfferingCode
                    }).ToList();

                }
                else
                {
                    data = _repositoryTlCourseOfferings.TableNoTracking
                    .Where(x => x.ACAD_CAREER != _acadCareer)
                    .Select(courseOffering => new CourseOfferingDto
                    {
                        STRM = courseOffering.STRM,
                        ACADEMIC_YEAR = courseOffering.ACADEMIC_YEAR,
                        ACADEMIC_TERM = courseOffering.ACADEMIC_TERM,
                        CourseOfferingCode = courseOffering.CourseOfferingCode
                    }).ToList();
                }
            }


            var result = data.Where(co => whitelistedCourseCodes.Contains(co.CourseOfferingCode.ToLower())).OrderBy(x => x.ACADEMIC_YEAR).ThenBy(x => x.CourseOfferingCode).ToList();
            var response = result.GroupBy(x => x.STRM, (key, group) =>
            {
                var item = result.FirstOrDefault(t => t.STRM == key);
                return new TextValue
                {
                    Value = item.STRM,
                    Text = $"{item.ACADEMIC_YEAR} T{item.ACADEMIC_TERM}",
                    Items = group.Select(g => new TextValue
                    {
                        Value = g.CourseOfferingCode,
                        Text = g.CourseOfferingCode
                    }).Distinct().ToList()
                };
            })
                .OrderByDescending(x => x.Text)
                .Take(takeRecentTerms)
                .ToList();

            return response;
        }

        public List<PeerFeedBackDashboardSelectOptionDto> PeerFeedbackDashboardGetWhitelistedTerm()
        {
            var courses = _repositoryCourse.TableNoTracking;
            var whitelistedCourseCodes = courses.Select(c => c.Code.ToLower()).Distinct().ToList();
            var result = _repositoryTlCourseOfferings.TableNoTracking.Where(co => whitelistedCourseCodes.Contains(co.CourseOfferingCode.ToLower())).DistinctBy(x => x.STRM).OrderByDescending(co => co.STRM).ToList();
            return result.Select(x => new PeerFeedBackDashboardSelectOptionDto
            {
                Courses = courses.Where(c => c.Code.ToLower() == x.CourseOfferingCode.ToLower()).Select(r => r.Id.ToString()).ToList(),
                Text = $"{x.ACADEMIC_YEAR} T{x.ACADEMIC_TERM}",
                Value = Convert.ToInt32(x.STRM)
            }).ToList();
        }
        public List<DefaultEntity> PeerFeedbackSessionsGetTerm()
        {
            var result = _repositoryTlCourseOfferings.Table.DistinctBy(x => x.STRM).ToList();
            return result.Select(x => new DefaultEntity
            {
                Text = $"{x.ACADEMIC_YEAR} T{x.ACADEMIC_TERM}",
                Value = Convert.ToInt32(x.STRM)
            }).ToList();
        }
        public List<PeerFeedbackSessions> PeerFeedbackSessionsGetByPairingId(int peerFeedBackPairingId)
        {
            var queryPeerFeedBackSessions = _repositoryPeerFeedbackSessions.Table;
            var queryPeerFeedBackPairingSession = _repositoryPeerFeedBackPairingSessions.Table;
            var query = from sessions in queryPeerFeedBackSessions
                        join pairingSessions in queryPeerFeedBackPairingSession on sessions.Id equals pairingSessions.PeerFeedBackSessionId
                        where pairingSessions.PeerFeedBackPairingId == peerFeedBackPairingId
                        select sessions;
            return query.ToList();
        }
        public List<PeerFeedbackSessions> PeerFeedbackSessionsGetByPeerFeedBackId(int peerFeedBackId)
        {
            return _repositoryPeerFeedbackSessions.Table.Where(x => x.IsDeleted == false && x.PeerFeedbackId == peerFeedBackId).ToList();
        }
        public List<PeerFeedbackSessionsDto> PeerFeedbackSessionsGetByPeerFeedBackIdWithPairing(int peerFeedBackId)
        {
            var queryPeerFeedBackSessions = _repositoryPeerFeedbackSessions.Table;
            var queryPeerFeedBackPairingSession = _repositoryPeerFeedBackPairingSessions.Table;
            var query = from sessions in queryPeerFeedBackSessions
                        join pairingSessions in queryPeerFeedBackPairingSession on sessions.Id equals pairingSessions.PeerFeedBackSessionId into tmp
                        from pairings in tmp.DefaultIfEmpty()
                        where sessions.PeerFeedbackId == peerFeedBackId
                        select new PeerFeedbackSessionsDto
                        {
                            EntryCloseTime = sessions.EntryCloseTime,
                            EntryStartTime = sessions.EntryStartTime,
                            Id = sessions.Id,
                            PeerFeedbackId = sessions.PeerFeedbackId,
                            Strm = sessions.Strm,
                            Label = sessions.Label,
                            LastUpdatedBy = sessions.LastUpdatedBy,
                            LastUpdatedTime = sessions.LastUpdatedTime,
                            PeerFeedBackParingId = pairings.PeerFeedBackPairingId
                        };
            return query.ToList();
        }
        public List<string> PeerFeedbackSessionsGetCourseByIdAndTerm(string term)
        {
            var courses = _repositoryCourse.TableNoTracking;
            var query = from categoryGroup in _categoryGroupRepository.TableNoTracking
                        join courseCategories in _courseCategoryRepository.TableNoTracking on categoryGroup.CourseCategoryId equals courseCategories.Id
                        join userGroup in _userGroupRepository.TableNoTracking on categoryGroup.Id equals userGroup.CategoryGroupId
                        join user in _userRepository.TableNoTracking on userGroup.UserId equals user.Id
                        join course in courses on courseCategories.CourseId equals course.Id
                        select course.Code.ToLower();

            var whitelistedCourseCodes = query.Distinct().ToList();

            var data = (from courseOffering in _repositoryTlCourseOfferings.TableNoTracking
                        join classV in _repositoryPS_SIS_LMS_CLASS_V.TableNoTracking on courseOffering.STRM equals classV.STRM
                        where courseOffering.CLASS_NBR == classV.CLASS_NBR && courseOffering.ACAD_CAREER == classV.ACAD_CAREER
                        select new
                        {
                            courseOffering.STRM,
                            courseOffering.ACADEMIC_YEAR,
                            courseOffering.ACADEMIC_TERM,
                            courseOffering.CourseOfferingCode
                        }).ToList();

            var result = data.Where(co => whitelistedCourseCodes.Contains(co.CourseOfferingCode.ToLower()) && co.STRM == term).DistinctBy(x => x.STRM).OrderByDescending(co => co.STRM).ToList();
            return result.Select(x => x.CourseOfferingCode).ToList();
        }
        public List<PeerFeedBackGroupDto> PeerFeedbackSessionsCategoryGroup(List<string> codes, List<string> strms)
        {
            var result = new List<PeerFeedBackGroupDto>();
            var courses = _repositoryCourse.TableNoTracking;
            if (codes != null && codes.Any())
            {
                codes = codes.Select(x => x.ToLower()).ToList();
                courses = courses.Where(c => codes.Contains(c.Code.ToLower()));
            }
            var courseIds = courses.Select(x => x.Id).ToList();
            var userEnrollmentQuery = _userEnrollmentRepository.TableNoTracking.Where(x => courseIds.Contains(x.CourseId.Value)).ToList();
            var userIds = userEnrollmentQuery.Select(x => x.UserId).ToList();
            var userQuery = _userRepository.TableNoTracking.Where(x => userIds.Contains(x.Id)).ToList();

            var CourseCategoriesQuery = _courseCategoryRepository.TableNoTracking.Where(x => courseIds.Contains(x.CourseId.Value)).ToList();
            var courseCategoryIds = CourseCategoriesQuery.Select(c => c.Id).ToList();
            var categoryGroupsQuery = _categoryGroupRepository.TableNoTracking.Where(x => courseCategoryIds.Contains(x.CourseCategoryId.Value)).ToList();
            var categoryGroupIds = categoryGroupsQuery.Select(x => x.Id).ToList();
            var userGroupQuery = _userGroupRepository.TableNoTracking.Where(x => categoryGroupIds.Contains(x.CategoryGroupId.Value)).ToList();
            var roles = _repositoryRoles.TableNoTracking.ToList();
            var usersQuery = (from user in userQuery
                              join userEnrollment in userEnrollmentQuery on user.Id equals userEnrollment.UserId
                              select new UserDto
                              {
                                  Id = user.Id,
                                  DisplayName = user.DisplayName,
                                  EmailAddress = user.EmailAddress,
                                  RoleId = userEnrollment.RoleId.Value,
                                  CourseId = userEnrollment.CourseId.Value
                              }).ToList();

            var sessionQuery = strms == null || !strms.Any()
                                ? _repositoryPeerFeedbackSessions.TableNoTracking.Where(x => x.IsDeleted == false)
                                : _repositoryPeerFeedbackSessions.TableNoTracking.Where(x => x.IsDeleted == false && strms.Contains(x.Strm));
            var pfQuery = (from peerFeedBack in _repositoryPeerFeedback.TableNoTracking
                           join session in sessionQuery on peerFeedBack.Id equals session.PeerFeedbackId
                           select new PeerFeedBackSessionDto
                           {
                               CourseOfferingCode = session.CourseOfferingCode,
                               EntryCloseTime = session.EntryCloseTime,
                               EntryStartTime = session.EntryStartTime,
                               Id = session.Id,
                               Label = session.Label,
                               PeerFeedbackId = session.PeerFeedbackId,
                               PeerFeedBackName = peerFeedBack.Name,
                               Strm = session.Strm
                           }).ToList();
            foreach (var course in courses)
            {
                var users = usersQuery.Where(x => x.CourseId == course.Id);
                var instructorRole = roles.FirstOrDefault(x => string.Equals(x.Name, RoleName.INSTRUCTOR, StringComparison.OrdinalIgnoreCase));
                var studentRole = roles.FirstOrDefault(x => string.Equals(x.Name, RoleName.STUDENT, StringComparison.OrdinalIgnoreCase));
                var instructors = users.Where(x => x.RoleId == instructorRole.Id);
                var students = users.Where(x => x.RoleId == studentRole.Id);

                var courseCategories = CourseCategoriesQuery.Where(x => x.CourseId == course.Id).ToList();
                var groupName = courseCategories.Any() ? string.Join(",", courseCategories.Select(x => x.Name)) : string.Empty;
                var categoryGroups = (from courseCategory in courseCategories
                                      join categoryGroup in categoryGroupsQuery on courseCategory.Id equals categoryGroup.CourseCategoryId
                                      select categoryGroup).ToList();
                var noOfGroup = categoryGroups.Any() ? categoryGroups.Count() : 0;
                int unassignedStudent = 0;
                bool hasMultipleGroup = false, duplicateEnrollment = false;
                if (categoryGroups.Any())
                {
                    var groupIds = categoryGroups.Select(x => x.Id).ToList();
                    var groups = userGroupQuery.Where(x => groupIds.Contains(x.CategoryGroupId.Value));
                    var userGroup = groups.Select(x => x.UserId).ToList();
                    unassignedStudent = students.Count(x => !userGroup.Contains(x.Id));
                    hasMultipleGroup = groups.Count() > 1;
                    duplicateEnrollment = userGroup.Count > userGroup.Distinct().Count();
                }
                var sessionCourseOfferingCode = pfQuery.Where(x => !string.IsNullOrEmpty(x.CourseOfferingCode) && x.CourseOfferingCode.Split().Contains(course.Code)).FirstOrDefault();
                var createdInPSFS = sessionCourseOfferingCode == null
                                    ? "available"
                                    : $"({sessionCourseOfferingCode.PeerFeedBackName}) {sessionCourseOfferingCode.Label} ({sessionCourseOfferingCode.EntryStartTime.ToString("dd MMMM yyyy")} to {sessionCourseOfferingCode.EntryCloseTime.ToString("dd MMMM yyyy")})";
                var item = new PeerFeedBackGroupDto
                {
                    TotalParticipants = students.Count(),
                    InstructorName = string.Join(",", instructors.Select(x => x.DisplayName)),
                    InstructorEmail = string.Join(",", instructors.Select(x => x.EmailAddress)),
                    CourseCode = course.Code,
                    CourseId = course.Id,
                    GroupNames = groupName,
                    NoOfGroup = noOfGroup,
                    HasMultipleGroup = hasMultipleGroup ? "Y" : "N",
                    DuplicateEnrollment = duplicateEnrollment ? "Y" : "N",
                    CreatedInPSFS = createdInPSFS
                };
                result.Add(item);
            }
            var whitelistedCourseCodes = courses.Select(c => c.Code.ToLower()).Distinct().ToList();
            var tlcourseQuery = _repositoryTlCourseOfferings.TableNoTracking.Where(co => whitelistedCourseCodes.Contains(co.CourseOfferingCode.ToLower())).ToList();
            string param = strms == null || !strms.Any() ? string.Empty : string.Join(",", strms);
            string query = "SELECT [ACAD_CAREER],[STRM],[CLASS_NBR],[SIS_TERM_DESCR],[DESCR],[COURSE_TITLE_LONG],[SMU_CRSE_CD],[CLASS_SECTION],[START_DT],[END_DT],[ACAD_GROUP],[ACAD_ORG] FROM [PS_SIS_LMS_CLASS_V] {0} ";
            var classVQuery = _isisContext.SqlQuery<PS_SIS_LMS_CLASS_V>(string.Format(query, string.IsNullOrEmpty(param) ? string.Empty : $" WHERE STRM IN ({param})")).ToList();
            var isisQuery = (from tlCourse in tlcourseQuery
                             join classV in classVQuery on tlCourse.STRM equals classV.STRM
                             join data in result on tlCourse.CourseOfferingCode equals data.CourseCode
                             where tlCourse.CLASS_NBR == classV.CLASS_NBR && tlCourse.ACAD_CAREER == classV.ACAD_CAREER
                             select new PeerFeedBackGroupDto
                             {
                                 CourseCode = tlCourse.CourseOfferingCode,
                                 CourseTitle = classV.COURSE_TITLE_LONG,
                                 OfferingSchool = classV.ACAD_GROUP,
                                 TotalParticipants = data.TotalParticipants,
                                 InstructorName = data.InstructorName,
                                 InstructorEmail = data.InstructorEmail,
                                 CourseId = data.CourseId,
                                 GroupNames = data.GroupNames,
                                 NoOfGroup = data.NoOfGroup,
                                 CreatedInPSFS = data.CreatedInPSFS,
                             }).ToList();
            return isisQuery.ToList();
        }
        public List<PeerFeedbackSessionsDto> PeerFeedbackSessionsGetList()
        {
            var queryPeerFeedBackSessions = _repositoryPeerFeedbackSessions.TableNoTracking;
            var queryPeerFeedBack = _repositoryPeerFeedback.TableNoTracking;
            var query = from sessions in queryPeerFeedBackSessions
                        join peerFeedBack in queryPeerFeedBack on sessions.PeerFeedbackId equals peerFeedBack.Id
                        where peerFeedBack.IsDeleted == false && sessions.IsDeleted == false
                        select new PeerFeedbackSessionsDto
                        {
                            Id = sessions.Id,
                            Strm = sessions.Strm,
                            Label = sessions.Label,
                            PeerFeedBackName = peerFeedBack.Name,
                            EntryCloseTime = sessions.EntryCloseTime,
                            EntryStartTime = sessions.EntryStartTime,
                            PeerFeedbackId = sessions.PeerFeedbackId,
                            CourseOfferingCode = sessions.CourseOfferingCode
                        };
            return query.OrderByDescending(x => x.Id).ToList();
        }
        public List<PeerFeedBackGroupDto> PeerFeedbackSessionsGetCourseInfoPreview(List<string> codes, List<string> strms)
        {
            var result = new List<PeerFeedBackGroupDto>();
            codes = codes.Select(x => x.ToLower()).ToList();
            var courses = _repositoryCourse.TableNoTracking.Where(x => codes.Contains(x.Code.ToLower())).ToList();
            var courseIds = courses.Select(x => x.Id).ToList();

            var roles = _repositoryRoles.TableNoTracking.ToList();
            var instructorRole = roles.FirstOrDefault(x => string.Equals(x.Name, RoleName.INSTRUCTOR, StringComparison.OrdinalIgnoreCase));

            var usersQuery = (from user in _userRepository.TableNoTracking
                              join userEnrollment in _userEnrollmentRepository.TableNoTracking on user.Id equals userEnrollment.UserId
                              where courseIds.Contains(userEnrollment.CourseId.Value) && userEnrollment.RoleId == instructorRole.Id
                              select new UserDto
                              {
                                  Id = user.Id,
                                  DisplayName = user.DisplayName,
                                  EmailAddress = user.EmailAddress,
                                  RoleId = userEnrollment.RoleId.Value,
                                  CourseId = userEnrollment.CourseId.Value
                              }).ToList();

            var tlcourseQuery = _repositoryTlCourseOfferings.TableNoTracking.Where(co => codes.Contains(co.CourseOfferingCode.ToLower())).ToList();
            var listData = new List<PeerFeedBackGroupDto>();
            var classVResult = _repositoryPS_SIS_LMS_CLASS_V.TableNoTracking.Where(x => strms.Contains(x.STRM)).ToList();
            foreach (var item in tlcourseQuery)
            {
                var data = classVResult.FirstOrDefault(x => x.STRM == item.STRM && x.CLASS_NBR == item.CLASS_NBR && item.ACAD_CAREER == x.ACAD_CAREER);
                listData.Add(new PeerFeedBackGroupDto
                {
                    CourseCode = item.CourseOfferingCode,
                    OfferingSchool = data == null ? string.Empty : data.ACAD_GROUP
                });
            }
            foreach (var course in courses)
            {
                var users = usersQuery.Where(x => x.CourseId == course.Id);
                var instructors = users.Where(x => x.RoleId == instructorRole.Id);
                var isisData = listData.FirstOrDefault(i => string.Equals(i.CourseCode, course.Code, StringComparison.OrdinalIgnoreCase));
                var item = new PeerFeedBackGroupDto
                {
                    CourseId = course.Id,
                    CourseCode = course.Code,
                    CourseTitle = course.Name,
                    OfferingSchool = isisData == null ? string.Empty : isisData.OfferingSchool,
                    InstructorName = string.Join(",", instructors.Select(x => x.DisplayName)),
                    InstructorEmail = string.Join(",", instructors.Select(x => x.EmailAddress))
                };
                result.Add(item);
            }
            return result;
        }
        public List<string> PeerFeedBackSessionGetCourseOfferingCodeBySessionIds(List<int> sessionIds)
        {
            if (sessionIds == null || !sessionIds.Any())
                return null;
            var codes = _repositoryPeerFeedbackSessions.TableNoTracking.Where(x => sessionIds.Contains(x.Id) && x.IsDeleted == false).Select(x => x.CourseOfferingCode).ToList();
            return codes.SelectMany(c => c.Split(',')).Distinct().ToList();
        }
        public async Task<(int TotalCount, IList<CourseOfferingDto> Terms)> PeerFeedbackGetWhitelistedTermPagingAsync(int page = 1,
            int pageSize = 100, string filter = default, bool useFullDbName = false, AcadCareer acadCareer = AcadCareer.UG)
        {
            var totalCount = await _peerFeedBackDao.GetTotalCountTlCourseOfferingByCodes(acadCareer, filter, useFullDbName);
            if (totalCount > 0)
            {
                var terms = await _peerFeedBackDao.GetListCourseOfferingByCodes(acadCareer, page, pageSize, filter, useFullDbName);
                return (totalCount, terms.ToList());
            }
            return (default, default);
        }
        public async Task<IList<string>> PeerFeedbackGetCourseOfferingCodeByTerm(string strm)
        {
            if (string.IsNullOrEmpty(strm)) return new List<string>();

            var results = await _repositoryTlCourseOfferings.TableNoTracking
                .Where(x => x.STRM == strm)
                .Select(x => x.CourseOfferingCode)
                .Distinct().ToListAsync();
            return results;
        }
        public async Task<TextValue> PeerFeedbackGetDefaultSelectedStrm(string strm)
        {
            if (string.IsNullOrEmpty(strm)) return new TextValue();

            var result = await _repositoryTlCourseOfferings.TableNoTracking
                .Where(x => x.STRM == strm && x.MERGE_SECTION == false)
                .Select(x => new CourseOfferingDto
                {
                    STRM = x.STRM,
                    ACADEMIC_YEAR = x.ACADEMIC_YEAR,
                    ACADEMIC_TERM = x.ACADEMIC_TERM
                }).FirstOrDefaultAsync();
            return new TextValue
            {
                Value = result.STRM,
                Text = $"{result.ACADEMIC_YEAR} T{result.ACADEMIC_TERM}"
            };
        }
        public async Task<List<TextValue>> PeerFeedbackGetDefaultSelectedStrm(List<string> strms)
        {
            if (strms == null || !strms.Any()) return new List<TextValue>();

            var results = await _repositoryTlCourseOfferings.TableNoTracking
                .Where(x => strms.Contains(x.STRM) && x.MERGE_SECTION == false)
                .Select(x => new CourseOfferingDto
                {
                    STRM = x.STRM,
                    ACADEMIC_YEAR = x.ACADEMIC_YEAR,
                    ACADEMIC_TERM = x.ACADEMIC_TERM
                }).ToListAsync();
            return results.Select(result => new TextValue
            {
                Value = result.STRM,
                Text = $"{result.ACADEMIC_YEAR} T{result.ACADEMIC_TERM}"
            }).ToList();
        }
        #endregion

        #region PEER_FEEDBACK_TARGETS
        public void PeerFeedbackTargetsInsert(PeerFeedbackTargets peerFeedbackTargets)
        {
            _repositoryPeerFeedbackTargets.Insert(peerFeedbackTargets);
        }

        public void PeerFeedbackTargetsUpdate(PeerFeedbackTargets peerFeedbackTargets)
        {
            _repositoryPeerFeedbackTargets.Update(peerFeedbackTargets);
        }

        public void PeerFeedbackTargetsDelete(PeerFeedbackTargets peerFeedbackTargets)
        {
            peerFeedbackTargets.IsDeleted = true;
            _repositoryPeerFeedbackTargets.Update(peerFeedbackTargets);
        }

        public PeerFeedbackTargets PeerFeedbackTargetsGetById(int id)
        {
            return _repositoryPeerFeedbackTargets.GetById(id);
        }

        public List<ItemDto> PeerFeedbackTargetGetTargets(int peerFeedBackId, List<string> codes)
        {
            var queryUsers = _userRepository.Table;
            var queryGroups = _categoryGroupRepository.Table;
            var queryCourseCategory = _courseCategoryRepository.Table;
            var queryUserGroups = _userGroupRepository.Table;
            var evaluation = _repositoryPeerFeedback.GetById(peerFeedBackId);
            var resultList = new List<ItemDto>();

            var courses = _repositoryCourse.Table.Where(x => codes.Contains(x.Code))
                .Select(x => x.Id).ToList();
            switch (evaluation.TypeId)
            {
                case (int)PeerFeedbackType.StudentsEvaluateOwnGroupMembers:
                    var query = from courseCategories in queryCourseCategory
                                join categoryGroups in queryGroups on courseCategories.Id equals categoryGroups.CourseCategoryId
                                join userGroup in queryUserGroups on categoryGroups.Id equals userGroup.CategoryGroupId
                                join users in queryUsers on userGroup.UserId equals users.Id
                                where courses.Contains(courseCategories.CourseId.Value)
                                select new ItemDto
                                {
                                    Id = categoryGroups.Id,
                                    Name = categoryGroups.Name,
                                    Description = users.DisplayName,
                                    Type = 1,
                                    OrgDefinedId = "",
                                    GroupId = courseCategories.Id,
                                    DisplayName = courseCategories.Name
                                };
                    resultList.AddRange(query.ToList().GroupBy(x => x.Id).Select(item => new ItemDto
                    {
                        Id = item.First().Id,
                        Name = item.First().Name,
                        Description = string.Join(", ", item.Select(x => x.Description).ToList()),
                        Type = 1,
                        GroupId = item.First().GroupId,
                        DisplayName = item.First().DisplayName
                    }));
                    break;
            }

            resultList = resultList.OrderBy(item => item.Name).ToList();
            return resultList;
        }
        public List<ItemDto> PeerFeedbackTargetsGetByPairingId(int peerFeedBackPairingId, int? targetId = null)
        {
            var returnList = new List<ItemDto>();

            var queryTargets = _repositoryPeerFeedbackTargets.Table;
            var queryUsers = _userRepository.Table;
            var queryUserEnrollment = _userEnrollmentRepository.Table;

            var targets = from target in queryTargets
                          where target.PeerFeedbackPairingId == peerFeedBackPairingId
                          select target;

            var peerFeedbackPairings = _repositoryPeerFeedbackPairings.GetById(peerFeedBackPairingId);
            var peerFeedback = _repositoryPeerFeedback.TableNoTracking.FirstOrDefault(q => q.Id == peerFeedbackPairings.PeerFeedbackId);

            if (targets.Any())
            {
                if (peerFeedback != null)
                {
                    var singularTargetQuery =
                        from target in targets
                        join user in queryUsers on target.UserId equals user.Id
                        join userEnrollment in queryUserEnrollment on user.Id equals userEnrollment.UserId
                        where userEnrollment.CourseId == 6726
                        select new
                        {
                            OrgUnitId = target.OrgUnitId,
                            UserId = target.UserId,
                            OrgDefinedId = user.OrgDefinedId,
                            IsOrgUnit = target.IsOrgUnit,
                            section = string.Empty
                        };
                    if (peerFeedback.TypeId == (int)Core.PeerFeedbackType.StudentsEvaluateOwnGroupMembers)
                    {
                        var groups = _categoryGroupRepository.Table;
                        returnList = singularTargetQuery.Join(groups,
                                             user => user.OrgUnitId,
                                             group => group.Id,
                                             (user, group) => new ItemDto
                                             {
                                                 Id = user.UserId,
                                                 Name = queryUsers.FirstOrDefault(g => g.Id == user.UserId).DisplayName,
                                                 Type = 0, // singular
                                                 Description = user.section,
                                                 OrgDefinedId = user.OrgDefinedId,
                                                 GroupId = user.OrgUnitId,
                                                 Group = group.Name
                                             }).ToList();
                    }
                }
            }

            return returnList.OrderBy(x => x.Name).ToList();
        }
        public List<PeerFeedbackTargets> PeerFeedbackTargetsGetByPairingGroup(int peerFeedBackPairingId, int peerFeedBackGroupId)
        {
            var targets = from target in _repositoryPeerFeedbackTargets.Table.Where(x => x.IsDeleted == false)
                          where target.PeerFeedbackPairingId == peerFeedBackPairingId && target.OrgUnitId == peerFeedBackGroupId
                          select target;
            return targets.ToList();
        }
        public List<int> PeerFeedBackTargetsGetGroup(int peerFeedBackPairingId)
        {
            return _repositoryPeerFeedbackTargets.Table
                .Where(x => x.PeerFeedbackPairingId == peerFeedBackPairingId)
                .Select(x => x.OrgUnitId)
                .ToList();
        }
        public List<PeerFeedbackTargets> PeerFeedBackTargetsGetList(int targetUserId)
        {
            var groupIds = _repositoryPeerFeedbackTargets.Table.Where(x => x.IsDeleted == false && x.UserId == targetUserId).Select(x => x.OrgUnitId).ToList();
            var query = from target in _repositoryPeerFeedbackTargets.Table.Where(x => x.IsDeleted == false && groupIds.Contains(x.OrgUnitId))
                        join pairing in _repositoryPeerFeedbackPairings.Table on target.PeerFeedbackPairingId equals pairing.Id
                        join pairingSession in _repositoryPeerFeedBackPairingSessions.Table on pairing.Id equals pairingSession.PeerFeedBackPairingId
                        join session in _repositoryPeerFeedbackSessions.Table.Where(x => x.IsDeleted == false) on pairingSession.PeerFeedBackSessionId equals session.Id
                        join feedBack in _repositoryPeerFeedback.Table.Where(x => x.IsDeleted == false) on session.PeerFeedbackId equals feedBack.Id
                        select target;
            return query.ToList();
        }
        public List<PeerFeedbackTargets> PeerFeedBackTargetsGetUserIdBySessionList(List<int> sessionIds, List<int> courses)
        {
            var pairingSessionQuery = _repositoryPeerFeedBackPairingSessions.TableNoTracking.Where(x => sessionIds.Contains(x.PeerFeedBackSessionId));
            var targets = from courseCate in _courseCategoryRepository.TableNoTracking
                          join course in courses on courseCate.CourseId equals course
                          join cateGroup in _categoryGroupRepository.TableNoTracking on courseCate.Id equals cateGroup.CourseCategoryId
                          join target in _repositoryPeerFeedbackTargets.TableNoTracking on cateGroup.Id equals target.OrgUnitId
                          where target.IsDeleted == false
                          select target;

            var query = from target in targets
                        join pairingSession in pairingSessionQuery on target.PeerFeedbackPairingId equals pairingSession.PeerFeedBackPairingId
                        select target;
            return query.Distinct().ToList();
        }
        public List<PeerFeedbackTargets> PeerFeedbackTargetsGetByPairingGroupSession(int peerFeedBackSessionId, int peerFeedBackGroupId)
        {
            var targets = from target in _repositoryPeerFeedbackTargets.Table.Where(x => x.IsDeleted == false)
                          join pairingSession in _repositoryPeerFeedBackPairingSessions.TableNoTracking on target.PeerFeedbackPairingId equals pairingSession.PeerFeedBackPairingId
                          where pairingSession.PeerFeedBackSessionId == peerFeedBackSessionId && target.OrgUnitId == peerFeedBackGroupId
                          select target;
            return targets.ToList();
        }
        public List<PeerFeedbackTargetsDto> PeerFeedBackTargetsGetBySessionList(List<int> sessionIds, List<int> categoryIds)
        {
            var pairingSessionQuery = _repositoryPeerFeedBackPairingSessions.TableNoTracking.Where(x => sessionIds.Contains(x.PeerFeedBackSessionId));
            var targets = _repositoryPeerFeedbackTargets.TableNoTracking.Where(x => x.IsDeleted == false);
            var query = from target in targets
                        join pairingSession in pairingSessionQuery on target.PeerFeedbackPairingId equals pairingSession.PeerFeedBackPairingId
                        where categoryIds.Contains(target.OrgUnitId)
                        select new PeerFeedbackTargetsDto
                        {
                            UserId = target.UserId,
                            OrgUnitId = target.OrgUnitId,
                            PeerFeedBackSessionId = pairingSession.PeerFeedBackSessionId,
                        };
            return query.Distinct().ToList();
        }
        #endregion

        #region PEER_FEEDBACK_QUESTION_MAP
        public void PeerFeedbackQuestionMapInsert(PeerFeedbackQuestionMap feedbackQuestionMap)
        {
            _repositoryPeerFeedbackQuestionMap.Insert(feedbackQuestionMap);
        }

        public void PeerFeedbackQuestionMapDeleteById(PeerFeedbackQuestionMap feedbackQuestionMap)
        {
            _repositoryPeerFeedbackQuestionMap.Delete(feedbackQuestionMap);
        }
        public void PeerFeedbackQuestionMapDeleteByCondition(int peerFeedbackId, ICollection<int> questions)
        {
            if (questions != null)
            {
                var result = _repositoryPeerFeedbackQuestionMap.Table.Where(x =>
                    questions.Contains(x.PeerFeedbackQuestionId) && x.PeerFeedbackId == peerFeedbackId).ToList();
                _repositoryPeerFeedbackQuestionMap.Delete(result);
            }
        }
        public void PeerFeedbackQuestionMapDeleteByPeerFeedBackId(int peerFeedbackId)
        {
            if (peerFeedbackId > 0)
            {
                var result = _repositoryPeerFeedbackQuestionMap.Table.Where(x => x.PeerFeedbackId == peerFeedbackId).ToList();
                if (result != null && result.Any())
                {
                    _repositoryPeerFeedbackQuestionMap.Delete(result);
                }
            }
        }
        public List<PeerFeedbackQuestion> PeerFeedbackQuestionMapList(int peerFeedbackId)
        {
            var query = from a in _repositoryPeerFeedbackQuestion.TableNoTracking.Where(x => x.Deleted == false)
                        join b in _repositoryPeerFeedbackQuestionMap.TableNoTracking.Where(x =>
                                x.PeerFeedbackId == peerFeedbackId)
                            on a.Id equals b.PeerFeedbackQuestionId
                        select a;

            return query.ToList();
        }
        public PeerFeedbackQuestionMap PeerFeedbackQuestionMapGetById(int id)
        {
            return _repositoryPeerFeedbackQuestionMap.GetById(id);
        }
        #endregion

        #region PEER_FEEDBACK_PAIRINGS_SESSION

        public List<PeerFeedBackPairingSessions> PeerFeedBackPairingSessionsGetBySessionId(int peerFeedbackSessionId)
        {
            var result = _repositoryPeerFeedBackPairingSessions.Table
                .Where(x => x.PeerFeedBackSessionId == peerFeedbackSessionId).ToList();
            return result;
        }
        #endregion

        #region PEER_FEEDBACK_RESPONSES

        public void PeerFeedBackResponsesInsert(List<PeerFeedBackResponses> responsesList)
        {
            _peerFeedBackResponsesDao.BulkInsert(responsesList);
        }
        public void PeerFeedBackResponsesDeleteById(int id)
        {
            var response = _repositoryPeerFeedBackResponses.GetById(id);
            if (response != null)
            {
                _repositoryPeerFeedBackResponses.Delete(response);
            }
        }
        public void PeerFeedBackResponsesDeletePeerFeedBack(int peerFeedBackId, int peerFeedBackSessionId, int peerFeedBackGroupId, int evaluatorUserId)
        {
            _peerFeedBackResponsesDao.Delete(peerFeedBackId, peerFeedBackSessionId, peerFeedBackGroupId, evaluatorUserId);
        }
        public List<PeerFeedBackResponses> PeerFeedBackResponsesGetList(
            int peerFeedbackId,
            int peerFeedBackQuestionId,
            int peerFeedBackSessionId,
            int peerFeedBackGroupId)
        {
            var responses = _repositoryPeerFeedBackResponses.Table.Where(x =>
                x.PeerFeedbackId == peerFeedbackId &&
                x.PeerFeedbackQuestionId == peerFeedBackQuestionId &&
                x.PeerFeedBackGroupId == peerFeedBackGroupId &&
                x.PeerFeedbackSessionId == peerFeedBackSessionId &&
                x.IsDeleted == false).ToList();
            return responses;
        }
        public List<PeerFeedBackResponses> PeerFeedBackResponsesGetData(
            int peerFeedbackId,
            int peerFeedBackQuestionId,
            int peerFeedBackSessionId)
        {
            var responses = _repositoryPeerFeedBackResponses.Table.Where(x =>
                x.PeerFeedbackId == peerFeedbackId &&
                x.PeerFeedbackQuestionId == peerFeedBackQuestionId &&
                x.PeerFeedbackSessionId == peerFeedBackSessionId &&
                x.EvaluatorUserId != x.TargetUserId &&
                x.IsDeleted == false).ToList();
            return responses;
        }
        public List<PeerFeedBackResponses> PeerFeedBackResponsesGetList(
            int peerFeedbackId,
            int peerFeedBackSessionId,
            int peerFeedBackGroupId)
        {
            var responses = _repositoryPeerFeedBackResponses.Table.Where(x =>
                x.PeerFeedbackId == peerFeedbackId &&
                x.PeerFeedBackGroupId == peerFeedBackGroupId &&
                x.PeerFeedbackSessionId == peerFeedBackSessionId &&
                x.IsDeleted == false).ToList();
            return responses;
        }
        public List<PeerFeedBackResponses> PeerFeedBackResponsesGetListByUser(
            int peerFeedbackId,
            int peerFeedBackSessionId,
            int peerFeedBackGroupId, int evaluatorUserId)
        {
            var responses = _repositoryPeerFeedBackResponses.Table.Where(x =>
                x.PeerFeedbackId == peerFeedbackId &&
                x.PeerFeedBackGroupId == peerFeedBackGroupId &&
                x.PeerFeedbackSessionId == peerFeedBackSessionId &&
                x.EvaluatorUserId == evaluatorUserId &&
                x.IsDeleted == false).ToList();
            return responses;
        }
        public List<PeerFeedBackResponses> PeerFeedBackResponsesGetByTarget(
            int userId,
            int peerFeedbackId,
            int peerFeedBackQuestionId,
            int peerFeedBackSessionId,
            int peerFeedBackGroupId)
        {
            var responses = _repositoryPeerFeedBackResponses.Table.Where(x => x.TargetUserId == userId &&
                x.PeerFeedbackId == peerFeedbackId &&
                x.PeerFeedbackQuestionId == peerFeedBackQuestionId &&
                x.PeerFeedBackGroupId == peerFeedBackGroupId &&
                x.PeerFeedbackSessionId == peerFeedBackSessionId &&
                x.IsDeleted == false).ToList();
            return responses;
        }
        public List<PeerFeedBackResponses> PeerFeedBackResponsesGetListWithGroup(int peerFeedbackId,
            int peerFeedBackSessionId, int peerFeedBackGroupId)
        {
            var responses = _repositoryPeerFeedBackResponses.Table.Where(x =>
                x.PeerFeedbackId == peerFeedbackId &&
                x.PeerFeedbackSessionId == peerFeedBackSessionId &&
                x.PeerFeedBackGroupId == peerFeedBackGroupId &&
                x.IsDeleted == false).ToList();
            return responses;
        }
        public List<PeerFeedBackResponses> PeerFeedBackResponsesGetListWithGroupAndUser(int peerFeedbackId, int peerFeedBackSessionId, int peerFeedBackGroupId, int evaluatorUserId)
        {
            var responses = _repositoryPeerFeedBackResponses.Table.Where(x =>
                x.PeerFeedbackId == peerFeedbackId &&
                x.PeerFeedbackSessionId == peerFeedBackSessionId &&
                x.PeerFeedBackGroupId == peerFeedBackGroupId &&
                x.EvaluatorUserId == evaluatorUserId &&
                x.IsDeleted == false).ToList();
            return responses;
        }
        public List<PeerFeedBackResponses> PeerFeedBackResponsesGetPeerFeedBackId(List<int> groups, List<int> sessionIds, List<int> peerFeedBackIds, int? peerFeedBackQuestionId = null)
        {
            var result = _repositoryPeerFeedBackResponses.Table.Where(x =>
                            sessionIds.Contains(x.PeerFeedbackSessionId) &&
                            peerFeedBackIds.Contains(x.PeerFeedbackId)
                            && x.IsDeleted == false);
            result = from response in result
                     join session in _repositoryPeerFeedbackSessions.TableNoTracking.Where(x => DbFunctions.TruncateTime(x.EntryCloseTime) < DateTime.UtcNow && x.IsDeleted == false) on response.PeerFeedbackSessionId equals session.Id
                     select response;

            if (groups != null && groups.Any())
            {
                result = result.Where(x => groups.Contains(x.PeerFeedBackGroupId));
            }
            if (peerFeedBackQuestionId.HasValue)
            {
                result = result.Where(x => x.PeerFeedbackQuestionId == peerFeedBackQuestionId.Value);
            }
            return result.ToList();
        }
        public List<PeerFeedBackResponses> PeerFeedBackResponsesGetPeerFeedBack(List<int> groups, List<int> sessionIds, List<int> peerFeedBackIds, int? peerFeedBackQuestionId = null)
        {
            return _peerFeedBackResponsesDao.PeerFeedBackResponsesGetPeerFeedBackId(groups, sessionIds, peerFeedBackIds, peerFeedBackQuestionId);
        }
        public List<PeerFeedBackDashboardExportCsvDto> PeerFeedBackResponsesGetDataCsv(List<int> groups, List<int> sessionIds = null, List<int> peerFeedBackIds = null)
        {
            var queryUserTarget = _userRepository.TableNoTracking;
            var queryUserEvaluator = _userRepository.TableNoTracking;
            var queryCategoryGroup = _categoryGroupRepository.TableNoTracking;
            var queryQuestion = _repositoryPeerFeedbackQuestion.TableNoTracking;
            var queryRating = _repositoryPeerFeedbackRatingQuestion.TableNoTracking;
            var queryOption = _repositoryPeerFeedbackRatingOption.TableNoTracking;
            var queryCourseCategory = _courseCategoryRepository.TableNoTracking;
            var queryCourse = _repositoryCourse.TableNoTracking;

            var queryResponse = _repositoryPeerFeedBackResponses.Table.Where(x => groups.Contains(x.PeerFeedBackGroupId) && x.IsDeleted == false);

            if (sessionIds != null && sessionIds.Any())
            {
                queryResponse = queryResponse.Where(x => sessionIds.Contains(x.PeerFeedbackSessionId));
            }
            if (peerFeedBackIds != null && peerFeedBackIds.Any())
            {
                queryResponse = queryResponse.Where(x => peerFeedBackIds.Contains(x.PeerFeedbackId));
            }

            var result = from user in queryUserTarget
                         join response in queryResponse on user.Id equals response.TargetUserId
                         join userEvaluator in queryUserEvaluator on response.EvaluatorUserId equals userEvaluator.Id
                         join categoryGroup in queryCategoryGroup on response.PeerFeedBackGroupId equals categoryGroup.Id
                         join question in queryQuestion on response.PeerFeedbackQuestionId equals question.Id
                         join rating in queryRating on response.PeerFeedBackRatingId equals rating.Id
                         join option in queryOption on response.PeerFeedBackOptionId equals option.Id
                         join courseCategory in queryCourseCategory on categoryGroup.CourseCategoryId equals courseCategory.Id
                         join course in queryCourse on courseCategory.CourseId equals course.Id
                         join session in _repositoryPeerFeedbackSessions.TableNoTracking.Where(x => DbFunctions.TruncateTime(x.EntryCloseTime) < DateTime.UtcNow && x.IsDeleted == false) on response.PeerFeedbackSessionId equals session.Id
                         select new PeerFeedBackDashboardExportCsvDto
                         {
                             CourseId = course.Id,
                             CourseName = course.Name,
                             EvaluatorCampusId = userEvaluator.OrgDefinedId,
                             EvaluatorName = userEvaluator.DisplayName,
                             GroupName = categoryGroup.Name,
                             Question = question.Title,
                             Rating = rating.Name,
                             SelectedOption = option.Name,
                             TargetCampusId = user.OrgDefinedId,
                             TargetName = user.DisplayName,
                             UpdatedDateTime = response.LastUpdateTime,
                             TargetUserId = response.TargetUserId,
                             EvaluatorUserId = response.EvaluatorUserId,
                             PeerFeedBackRatingId = response.PeerFeedBackRatingId,
                             PeerFeedBackOptionId = response.PeerFeedBackOptionId,
                             PeerFeedbackQuestionId = response.PeerFeedbackQuestionId,
                             PeerSelfValue = response.TargetUserId == response.EvaluatorUserId ? "Self" : "Peer"
                         };
            return result.Distinct().ToList();
        }
        public List<PeerFeedBackResponses> PeerFeedBackResponsesByListGroup(int peerFeedBackQuestionId, List<int> groups)
        {
            var responses = _repositoryPeerFeedBackResponses.Table.Where(x => x.PeerFeedbackQuestionId == peerFeedBackQuestionId && groups.Contains(x.PeerFeedBackGroupId) && x.IsDeleted == false);
            var query = from response in responses
                        join session in _repositoryPeerFeedbackSessions.Table.Where(x => x.IsDeleted == false
                        && DbFunctions.TruncateTime(x.EntryCloseTime) < DateTime.UtcNow) on response.PeerFeedbackSessionId equals session.Id
                        select response;
            return query.ToList();
        }
        public List<PeerFeedBackResponses> PeerFeedBackResponsesGetByQuestion(int peerFeedBackQuestionId, int targetUserId)
        {
            var responses = _repositoryPeerFeedBackResponses.Table.Where(x => x.PeerFeedbackQuestionId == peerFeedBackQuestionId && x.IsDeleted == false);
            if (targetUserId > 0)
            {
                responses = responses.Where(x => x.TargetUserId == targetUserId);
            }
            var query = from response in responses
                        join session in _repositoryPeerFeedbackSessions.Table.Where(x => x.IsDeleted == false
                        && DbFunctions.TruncateTime(x.EntryCloseTime) < DateTime.UtcNow) on response.PeerFeedbackSessionId equals session.Id
                        select response;
            return query.ToList();
        }
        public List<PeerFeedBackResponses> PeerFeedBackResponsesGetByQuestion(int peerFeedBackQuestionId, List<int> peerFeedBackIds, List<int> groups)
        {
            var responses = _repositoryPeerFeedBackResponses.Table.Where(x => x.PeerFeedbackQuestionId == peerFeedBackQuestionId
            && x.IsDeleted == false
            && groups.Contains(x.PeerFeedBackGroupId)
            && peerFeedBackIds.Contains(x.PeerFeedbackId)).ToList();
            return responses;
        }
        #endregion

        #region PEER_FEEDBACK_RESPONSE_REMARKS

        public void PeerFeedBackResponseRemarksInsert(List<PeerFeedBackResponseRemarks> responsesList)
        {
            _peerFeedBackResponseRemarksDao.BulkInsert(responsesList);
        }
        public void PeerFeedBackResponseRemarksInsertOrUpdate(List<PeerFeedBackResponseRemarks> responsesList)
        {
            foreach (var item in responsesList)
            {
                if (item.Id != 0)
                {
                    _peerFeedBackResponseRemarksDao.Update(item);
                }
                else
                {
                    _peerFeedBackResponseRemarksDao.Insert(item);
                }
            }
        }
        public void PeerFeedBackResponseRemarksDeleteById(int id)
        {
            var response = _repositoryPeerFeedBackResponseRemarks.GetById(id);
            if (response != null)
            {
                _repositoryPeerFeedBackResponseRemarks.Delete(response);
            }
        }
        public void PeerFeedBackResponseRemarksDeletePeerFeedBack(int peerFeedBackId, int peerFeedBackSessionId, int peerFeedBackGroupId, int evaluatorUserId)
        {
            _peerFeedBackResponsesDao.Delete(peerFeedBackId, peerFeedBackSessionId, peerFeedBackGroupId, evaluatorUserId);
        }
        public List<PeerFeedBackResponseRemarks> PeerFeedBackResponseRemarksGetList(
            int peerFeedbackId,
            int peerFeedBackQuestionId,
            int peerFeedBackSessionId,
            int peerFeedBackGroupId)
        {
            var responses = _repositoryPeerFeedBackResponseRemarks.Table.Where(x =>
                x.PeerFeedbackId == peerFeedbackId &&
                x.PeerFeedBackGroupId == peerFeedBackGroupId &&
                x.PeerFeedbackSessionId == peerFeedBackSessionId &&
                x.IsDeleted == false).ToList();
            return responses;
        }
        public List<PeerFeedBackResponseRemarks> PeerFeedBackResponseRemarksGetData(
            int peerFeedbackId,
            int peerFeedBackQuestionId,
            int peerFeedBackSessionId)
        {
            var responses = _repositoryPeerFeedBackResponseRemarks.Table.Where(x =>
                x.PeerFeedbackId == peerFeedbackId &&
                x.PeerFeedbackSessionId == peerFeedBackSessionId &&
                x.EvaluatorUserId != x.TargetUserId &&
                x.IsDeleted == false).ToList();
            return responses;
        }
        public List<PeerFeedBackResponseRemarks> PeerFeedBackResponseRemarksGetList(
            int peerFeedbackId,
            int peerFeedBackSessionId,
            int peerFeedBackGroupId)
        {
            var responses = _repositoryPeerFeedBackResponseRemarks.Table.Where(x =>
                x.PeerFeedbackId == peerFeedbackId &&
                x.PeerFeedBackGroupId == peerFeedBackGroupId &&
                x.PeerFeedbackSessionId == peerFeedBackSessionId &&
                x.IsDeleted == false).ToList();
            return responses;
        }
        public List<PeerFeedBackResponseRemarks> PeerFeedBackResponseRemarksGetListByUser(
            int peerFeedbackId,
            int peerFeedBackSessionId,
            int peerFeedBackGroupId, int evaluatorUserId)
        {
            var responses = _repositoryPeerFeedBackResponseRemarks.Table.Where(x =>
                x.PeerFeedbackId == peerFeedbackId &&
                x.PeerFeedBackGroupId == peerFeedBackGroupId &&
                x.PeerFeedbackSessionId == peerFeedBackSessionId &&
                x.EvaluatorUserId == evaluatorUserId &&
                x.IsDeleted == false).ToList();
            return responses;
        }
        public List<PeerFeedBackResponseRemarks> PeerFeedBackResponseRemarksGetByTarget(
            int userId,
            int peerFeedbackId,
            int peerFeedBackSessionId,
            int peerFeedBackGroupId)
        {
            var responses = _repositoryPeerFeedBackResponseRemarks.Table.Where(x => x.TargetUserId == userId &&
                x.PeerFeedbackId == peerFeedbackId &&
                x.PeerFeedBackGroupId == peerFeedBackGroupId &&
                x.PeerFeedbackSessionId == peerFeedBackSessionId &&
                x.IsDeleted == false).ToList();
            return responses;
        }
        public List<PeerFeedBackResponseRemarks> PeerFeedBackResponseRemarksGetListWithGroup(int peerFeedbackId,
            int peerFeedBackSessionId, int peerFeedBackGroupId)
        {
            var responses = _repositoryPeerFeedBackResponseRemarks.Table.Where(x =>
                x.PeerFeedbackId == peerFeedbackId &&
                x.PeerFeedbackSessionId == peerFeedBackSessionId &&
                x.PeerFeedBackGroupId == peerFeedBackGroupId &&
                x.IsDeleted == false).ToList();
            return responses;
        }
        public List<PeerFeedBackResponseRemarks> PeerFeedBackResponseRemarksGetListWithGroupAndUser(int peerFeedbackId, int peerFeedBackSessionId, int peerFeedBackGroupId, int evaluatorUserId)
        {
            var responses = _repositoryPeerFeedBackResponseRemarks.Table.Where(x =>
                x.PeerFeedbackId == peerFeedbackId &&
                x.PeerFeedbackSessionId == peerFeedBackSessionId &&
                x.PeerFeedBackGroupId == peerFeedBackGroupId &&
                x.EvaluatorUserId == evaluatorUserId &&
                x.IsDeleted == false).ToList();
            return responses;
        }
        public List<PeerFeedBackResponseRemarks> PeerFeedBackResponseRemarksGetPeerFeedBackId(List<int> groups, List<int> sessionIds, List<int> peerFeedBackIds)
        {
            var result = _repositoryPeerFeedBackResponseRemarks.Table.Where(x =>
                            sessionIds.Contains(x.PeerFeedbackSessionId) &&
                            peerFeedBackIds.Contains(x.PeerFeedbackId)
                            && x.IsDeleted == false);
            result = from response in result
                     join session in _repositoryPeerFeedbackSessions.TableNoTracking.Where(x => DbFunctions.TruncateTime(x.EntryCloseTime) < DateTime.UtcNow && x.IsDeleted == false) on response.PeerFeedbackSessionId equals session.Id
                     select response;

            if (groups != null && groups.Any())
            {
                result = result.Where(x => groups.Contains(x.PeerFeedBackGroupId));
            }
            return result.ToList();
        }
        public List<PeerFeedBackResponseRemarks> PeerFeedBackResponseRemarksGetPeerFeedBack(List<int> groups, List<int> sessionIds, List<int> peerFeedBackIds)
        {
            return _peerFeedBackResponseRemarksDao.PeerFeedBackResponseRemarksGetPeerFeedBackId(groups, sessionIds, peerFeedBackIds);
        }
        public List<PeerFeedBackResponseRemarks> PeerFeedBackResponseRemarksByListGroup(List<int> groups)
        {
            var responses = _repositoryPeerFeedBackResponseRemarks.Table.Where(x => groups.Contains(x.PeerFeedBackGroupId) && x.IsDeleted == false);
            var query = from response in responses
                        join session in _repositoryPeerFeedbackSessions.Table.Where(x => x.IsDeleted == false
                        && DbFunctions.TruncateTime(x.EntryCloseTime) < DateTime.UtcNow) on response.PeerFeedbackSessionId equals session.Id
                        select response;
            return query.ToList();
        }
        public List<PeerFeedBackResponseRemarks> PeerFeedBackResponseRemarksGetByQuestion(int targetUserId)
        {
            var responses = _repositoryPeerFeedBackResponseRemarks.Table.Where(x => x.IsDeleted == false);
            if (targetUserId > 0)
            {
                responses = responses.Where(x => x.TargetUserId == targetUserId);
            }
            var query = from response in responses
                        join session in _repositoryPeerFeedbackSessions.Table.Where(x => x.IsDeleted == false
                        && DbFunctions.TruncateTime(x.EntryCloseTime) < DateTime.UtcNow) on response.PeerFeedbackSessionId equals session.Id
                        select response;
            return query.ToList();
        }
        public List<PeerFeedBackResponseRemarks> PeerFeedBackResponseRemarksGetByQuestion(List<int> peerFeedBackIds, List<int> groups)
        {
            var responses = _repositoryPeerFeedBackResponseRemarks.Table.Where(x => x.IsDeleted == false
            && groups.Contains(x.PeerFeedBackGroupId)
            && peerFeedBackIds.Contains(x.PeerFeedbackId)).ToList();
            return responses;
        }
        #endregion

        #region OTHER
        public List<int?> GetUserIdEnrollByCategories(int courseId, List<int> categoryIds)
        {
            var userGroupQuery = _userGroupRepository.Table.Where(x => categoryIds.Contains(x.CategoryGroupId.Value));
            var query = from userGroup in userGroupQuery
                        join enroll in _userEnrollmentRepository.Table.Where(x => x.CourseId == courseId) on userGroup.UserId equals enroll.UserId
                        select userGroup.UserId;
            return query.ToList();
        }
        #endregion

        #region SEED DATA
        public SeedData GetSeedData()
        {
            SeedData seedData = new SeedData
            {
                PeerFeedbackRatingQuestions = _repositoryPeerFeedbackRatingQuestion.Table.ToList(),
                PeerFeedbackRatingOptions = _repositoryPeerFeedbackRatingOption.Table.ToList(),
                PeerFeedbackQuestions = _repositoryPeerFeedbackQuestion.Table.ToList(),
                PeerFeedbackQuestionRatingMaps = _repositoryPeerFeedbackQuestionRatingMap.Table.ToList()
            };
            return seedData;
        }
        #endregion

        #region REPORT
        public IEnumerable<PeerFeedBackGroupReadinessReportDto> GetGroupReadinessData(List<string> selectedCourseCodes)
        {
            try
            {
                return _peerFeedBackDao.GetGroupReadinessData(selectedCourseCodes);
            }
            catch
            {
                return new List<PeerFeedBackGroupReadinessReportDto>();
            }
        }
        #endregion
    }
}