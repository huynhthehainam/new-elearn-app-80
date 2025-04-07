using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using eLearnApps.Core;
using eLearnApps.Entity.LmsTools;
using eLearnApps.Entity.LmsTools.Dto;
using eLearnApps.ViewModel.ICS;

namespace eLearnApps.Business.Interface
{
    public interface IIcsService
    {
        List<SessionViewModel> GetSessions(int courseId);

        /// <summary>
        ///     Get detail of an ICS Session that is not user specific. this is use to get detail of class performance
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        SessionDetailViewModel GetSessionDetail(int sessionId);

        /// <summary>
        ///     Get detail of an ICS session for one particular session. this is use to reload GiveFeedback page when user
        ///     navigate away from GiveFeedback Page
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        SessionDetailViewModel GetUserSessionDetail(int sessionId, int userId);

        int CreateSession(DateTime sessionDate, TimeSpan starTime, TimeSpan endTime, int courseId, string title);

        int CreateSessionFromClassSchedule(int courseId);

        void UpdateSession(int sessionId, DateTime sessionDate, TimeSpan startTime, TimeSpan endTime, string title);

        void DeleteSession(int sessionId);

        List<LearningPointViewModel> GetLearningPointBySessionId(int sessionId);

        LearningPointViewModel GetLearningPoint(int learningPointId);

        List<QuestionViewModel> GetQuestions(int sessionId);

        LearningPointViewModel AddLearningPoint(int sessionId, string description);

        void DeleteLearningPoint(int learningPointId);

        void UpdateLearningPoint(int learningPointId, string description);

        QuestionViewModel AddQuestion(int sessionId, int userId, string question);

        void DeleteQuestion(int questionId);

        void EditQuestion(int questionId, string question);

        void AddSense(ICSSessionUserSense userSense);

        void SetLearningPointCheck(int learningPointId, int userId, bool checkLearningPoint);

        SessionChartViewModel GetClassSensingSummary(int sessionId);

        SessionLineChart GetAllClassSensing(int sessionId);
        /// <summary>
        /// Get ics session by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ICSSession GetIcsSessionById(int id);
        /// <summary>
        /// Get list session by course id
        /// </summary>
        /// <param name="courseId"></param>
        /// <returns></returns>
        List<SessionViewModel> GetListSession(int courseId);
        /// <summary>
        /// Check user can delete session
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        int CanDeleteSession(int sessionId);
        /// <summary>
        ///     Check exists ics session between start time - end time
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="sessionDate"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        List<ICSSession> GetSessionsWithinTimeRange(int courseId, DateTime sessionDate, TimeSpan startTime, TimeSpan endTime);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        SessionDetailViewModel GetSessionLearningPointAndQuestion(int sessionId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        SessionDetailViewModel GetSessionQuestion(int sessionId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<LearningPoint> GetLearningPointBySessionAndId(int sessionId, int id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="sessionDate"></param>
        /// <param name="startTime"></param>
        /// <returns></returns>
        Task<ICSSession> GetSessionBySessionDateTime(int courseId, DateTime sessionDate, TimeSpan startTime);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="question"></param>
        /// <returns></returns>
        Task UpdateQuestionAddressed(Question question);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> IsExistsQuestionById(int id);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        Task UpdateDoingWellBySessionId(int sessionId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userSense"></param>
        /// <returns></returns>
        Task DeleteByCondition(ICSSessionUserSense userSense);
    }
}