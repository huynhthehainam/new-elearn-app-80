using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using eLearnApps.Core;
using eLearnApps.Entity.LmsIsis.Dto;
using eLearnApps.Entity.LmsTools;
using eLearnApps.Entity.LmsTools.Dto;

namespace eLearnApps.Business.Interface
{
    public interface IPeerFeedbackService
    {
        #region RATING
        void InsertPeerFeedbackRatingQuestion(PeerFeedbackRatingQuestion feedbackRatingQuestion);
        void UpdatePeerFeedbackRatingQuestion(PeerFeedbackRatingQuestion feedbackRatingQuestion);
        void DeletePeerFeedbackRatingQuestion(PeerFeedbackRatingQuestion feedbackRatingQuestion);
        PeerFeedbackRatingQuestion GetPeerFeedbackRatingQuestionById(int id);
        List<PeerFeedbackRatingQuestion> GetListPeerFeedbackRatingQuestions();
        List<PeerFeedbackRatingQuestion> GetListPeerFeedbackRatingQuestionsByQuestionId(int questionId);

        void InsertPeerFeedbackRatingOption(PeerFeedbackRatingOption feedbackRatingOption);
        void UpdatePeerFeedbackRatingOption(PeerFeedbackRatingOption feedbackRatingOption);
        void DeletePeerFeedbackRatingOption(PeerFeedbackRatingOption feedbackRatingOption);
        PeerFeedbackRatingOption GetPeerFeedbackRatingOptionById(int id);
        List<PeerFeedbackRatingOption> GetListPeerFeedbackRatingOptions();
        List<PeerFeedbackRatingOptionDto> GetListPeerFeedbackRatingOptionsByQuestionId(int questionId, int ratingQuestionId);
        #endregion

        #region QUESTION

        void InsertPeerFeedbackQuestion(PeerFeedbackQuestion feedbackQuestion);
        void UpdatePeerFeedbackQuestion(PeerFeedbackQuestion feedbackQuestion);
        void DeletePeerFeedbackQuestion(PeerFeedbackQuestion feedbackQuestion);
        PeerFeedbackQuestion GetPeerFeedbackQuestionById(int id);
        List<PeerFeedbackQuestion> GetListPeerFeedbackQuestions();

        #endregion

        #region QUESTION - RATING MAP
        void InsertPeerFeedbackQuestionRatingMap(PeerFeedbackQuestionRatingMap feedbackQuestionRatingMap);
        void UpdatePeerFeedbackQuestionRatingMap(PeerFeedbackQuestionRatingMap feedbackQuestionRatingMap);
        void DeletePeerFeedbackQuestionRatingMapByQuestionId(int questionId);
        void DeletePeerFeedbackQuestionRatingMapByRatingQuestionId(int questionId, int ratingQuestionId);
        PeerFeedbackQuestionRatingMap GetPeerFeedbackQuestionRatingMapById(int id);
        List<PeerFeedbackQuestionRatingMap> GetFeedbackQuestionRatingMapByQuestionId(int questionId);
        List<PeerFeedbackQuestionRatingMap> GetFeedbackQuestionRatingMapList();
        List<PeerFeedbackRatingQuestionMapOptionDto> GetListPeerFeedbackRatingQuestionsWithOptions(int questionId);

        #endregion

        #region PEER_FEEDBACK

        void PeerFeedbackInsert(PeerFeedback peerFeedback);
        void PeerFeedbackUpdate(PeerFeedback peerFeedback);
        void PeerFeedbackDelete(PeerFeedback peerFeedback);
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
        void PeerFeedbackDelete(int peerFeedBackId, int lastUpdatedBy);
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
        void PeerFeedbackDelete(int peerfeedbackId, int updatedBy, DateTime updatedTime);
        PeerFeedback PeerFeedbackGetById(int id);
        List<PeerFeedback> PeerFeedbackGetList();
        List<PeerFeedBackEvaluationDto> PeerFeedBackEvaluation();
        List<PeerFeedBackEvaluationDto> PeerFeedBackEvaluationList(int userId);
        List<User> PeerFeedBackEvaluationDetail(int peerFeedBackSessionId, int peerFeedBackGroupId,
            int peerFeedBackPairingId, int userId);
        List<DashboardFilterDto> PeerFeedBackGetDashboardFilter(string strm);

        /// <summary>
        /// Evaluation type is student evaluate own group member
        /// This function get users recorded in PeerFeedbackTargets
        /// </summary>
        /// <param name="peerfeedbackPairingId">Id of pairing</param>
        /// <returns>List of users</returns>
        List<User> GetTargetByPairingId(int peerfeedbackPairingId);

        List<Instructor> GetInstructorName(List<int> courseIds);

        int GetCourseId(int peerFeedbackGroupId);
        /// <summary>
        /// get data for csv
        /// </summary>
        /// <param name="strm"></param>
        /// <param name="whitelistedCourses"></param>
        /// <returns></returns>
        List<DashboardFilterDto> PeerFeedBackDownloadCsv(string strm);
        #endregion

        #region PEER_FEEDBACK_EVALUATORS
        void PeerFeedbackEvaluatorsInsert(PeerFeedbackEvaluators peerFeedbackEvaluators);
        void PeerFeedbackEvaluatorsUpdate(PeerFeedbackEvaluators peerFeedbackEvaluators);
        void PeerFeedbackEvaluatorsDelete(PeerFeedbackEvaluators peerFeedbackEvaluators);
        PeerFeedbackEvaluators PeerFeedbackEvaluatorsGetById(int id);


        #endregion

        #region PEER_FEEDBACK_PAIRINGS
        void PeerFeedbackPairingsInsert(PeerFeedbackPairings peerFeedbackPairings);
        void PeerFeedbackPairingsUpdate(PeerFeedbackPairings peerFeedbackPairings);
        void PeerFeedbackPairingsDelete(PeerFeedbackPairings peerFeedbackPairings);
        PeerFeedbackPairings PeerFeedbackPairingsGetById(int id);
        List<PeerFeedbackPairings> PeerFeedbackPairingsGetByPeerFeedBackId(int peerFeedBackId);

        /// <summary>
        /// Generate pairings along with evaluators and targets in specified session and courses
        /// </summary>
        /// <param name="peerFeedBackSessionId">Id of session to generate</param>
        /// <param name="updatingUserId">Id of logged user</param>
        void GenStdEvalOwnGroupMemberPairingsForSession(int peerFeedBackSessionId,int updatingUserId);
        PeerFeedbackPairings PeerFeedbackPairingsSave(int evaluationId,int peerFeedBackSessionId, int evaluationPairingId, int userId,
            List<int> evaluatorIds, List<int> targetIds);
        /// <summary>
        /// Generate pairings along with evaluators and targets in specified session and courses
        /// </summary>
        /// <param name="peerFeedbackId"></param>
        /// <param name="peerFeedBackSessionId"></param>
        /// <param name="strm"></param>
        /// <param name="updatingUserId"></param>
        void GeneratePairings(int peerFeedbackId, int peerFeedBackSessionId, string strm, int updatingUserId);
        /// <summary>
        /// Generate pairings along with evaluators and targets in specified session and courses
        /// </summary>
        /// <param name="session"></param>
        /// <param name="updatingUserId"></param>
        /// <returns></returns>
        Task GeneratePairings(PeerFeedbackSessions session,int updatingUserId);
        #endregion

        #region PEER_FEEDBACK_SESSIONS
        void PeerFeedbackSessionsInsert(PeerFeedbackSessions peerFeedbackSessions);
        void PeerFeedbackSessionsUpdate(PeerFeedbackSessions peerFeedbackSessions);
        void PeerFeedbackSessionsDelete(PeerFeedbackSessions peerFeedbackSessions);
        PeerFeedbackSessions PeerFeedbackSessionsGetById(int id);
        List<PeerFeedbackSessions> PeerFeedbackSessionsGetByPeerFeedbackId(int peerFeedbackId);

        /// <summary>
        /// Get ACAD_YEAR, ACAD_TERM of whitelisted courses
        /// This data to be used as term selection 
        /// </summary>
        /// <returns>list of whitelisted terms in listitem format</returns>
        List<TextValue> PeerFeedbackGetWhitelistedTerm();

        /// <summary>
        /// Get ACAD_YEAR, ACAD_TERM of whitelisted courses by acadCareer and limit result to takeRecentTerms
        /// This data to be used as term selection in UI
        /// </summary>
        /// <returns>list of whitelisted terms in listitem format</returns>
        List<TextValue> PeerFeedbackGetWhitelistedTerm(AcadCareer acadCareer, int takeRecentTerms);

        /// <summary>
        /// Get ACAD_YEAR, ACAD_TERM that is available in LMSISIS TL_CourseOffering
        /// This function to be used when display list of session available to students
        /// </summary>
        /// <returns>list of whitelisted terms in listitem format</returns>
        List<DefaultEntity> PeerFeedbackSessionsGetTerm();
        Task<(int TotalCount, IList<CourseOfferingDto> Terms)> PeerFeedbackGetWhitelistedTermPagingAsync(int page = 1, int pageSize = 10, string filter = default, bool useFullDbName = false, AcadCareer acadCareer = AcadCareer.UG);
        List<PeerFeedbackSessions> PeerFeedbackSessionsGetByPairingId(int peerFeedBackPairingId);
        List<PeerFeedbackSessions> PeerFeedbackSessionsGetByPeerFeedBackId(int peerFeedBackId);
        List<PeerFeedbackSessionsDto> PeerFeedbackSessionsGetByPeerFeedBackIdWithPairing(int peerFeedBackId);
        List<PeerFeedbackSessions> PeerFeedBackSessionGetListSessionIdByStrm(string strm);
        List<PeerFeedbackSessions> PeerFeedBackSessionGetListSessionIdByListStrm(List<string> listStrm);
        List<PeerFeedbackSessions> PeerFeedBackSessionGetListSessionIdFiltered(List<string> listStrm, string school, List<int> courses);
        bool PeerFeedbackSessionsCheckByCondition(PeerFeedbackSessions peerFeedbackSessions);
        List<PeerFeedBackDashboardSelectOptionDto> PeerFeedbackDashboardGetWhitelistedTerm();
        List<string> PeerFeedbackSessionsGetCourseByIdAndTerm(string term);
        List<PeerFeedBackGroupDto> PeerFeedbackSessionsCategoryGroup(List<string> codes, List<string> strms);
        List<PeerFeedbackSessionsDto> PeerFeedbackSessionsGetList();
        List<PeerFeedBackGroupDto> PeerFeedbackSessionsGetCourseInfoPreview(List<string> codes, List<string> strms);
        List<string> PeerFeedBackSessionGetCourseOfferingCodeBySessionIds(List<int> sessionIds);
        Task<IList<string>> PeerFeedbackGetCourseOfferingCodeByTerm(string strm);
        Task<TextValue> PeerFeedbackGetDefaultSelectedStrm(string strm);
        Task<List<TextValue>> PeerFeedbackGetDefaultSelectedStrm(List<string> strms);
        #endregion

        #region PEER_FEEDBACK_TARGETS
        void PeerFeedbackTargetsInsert(PeerFeedbackTargets peerFeedbackTargets);
        void PeerFeedbackTargetsUpdate(PeerFeedbackTargets peerFeedbackTargets);
        void PeerFeedbackTargetsDelete(PeerFeedbackTargets peerFeedbackTargets);
        PeerFeedbackTargets PeerFeedbackTargetsGetById(int id);
        List<ItemDto> PeerFeedbackTargetGetTargets(int evaluationId, List<string> codes);
        List<ItemDto> PeerFeedbackTargetsGetByPairingId(int peerFeedBackPairingId, int? targetId = null);
        List<int> PeerFeedBackTargetsGetGroup(int peerFeedBackPairingId);
        List<PeerFeedbackTargets> PeerFeedbackTargetsGetByPairingGroup(int peerFeedBackPairingId, int peerFeedBackGroupId);
        List<PeerFeedbackTargets> PeerFeedBackTargetsGetList(int targetUserId);
        List<PeerFeedbackTargets> PeerFeedBackTargetsGetUserIdBySessionList(List<int> sessionIds, List<int> courses);
        List<PeerFeedbackTargets> PeerFeedbackTargetsGetByPairingGroupSession(int peerFeedBackSessionId, int peerFeedBackGroupId);
        List<PeerFeedbackTargetsDto> PeerFeedBackTargetsGetBySessionList(List<int> sessionIds, List<int> categoryIds);
        #endregion

        #region PEER_FEEDBACK_QUESTION_MAP

        void PeerFeedbackQuestionMapInsert(PeerFeedbackQuestionMap feedbackQuestionMap);
        void PeerFeedbackQuestionMapDeleteById(PeerFeedbackQuestionMap feedbackQuestionMap);
        void PeerFeedbackQuestionMapDeleteByCondition( int peerFeedbackId, ICollection<int> questions);
        List<PeerFeedbackQuestion> PeerFeedbackQuestionMapList(int peerFeedbackId);
        PeerFeedbackQuestionMap PeerFeedbackQuestionMapGetById(int id);
        void PeerFeedbackQuestionMapDeleteByPeerFeedBackId(int peerFeedbackId);
        #endregion

        #region PEER_FEEDBACK_PAIRINGS_SESSION
        List<PeerFeedBackPairingSessions> PeerFeedBackPairingSessionsGetBySessionId(int peerFeedbackSessionId);
        #endregion

        #region PEER_FEEDBACK_RESPONSES

        void PeerFeedBackResponsesInsert(List<PeerFeedBackResponses> responsesList);
        void PeerFeedBackResponsesDeleteById(int id);
        void PeerFeedBackResponsesDeletePeerFeedBack(int peerFeedBackId, int peerFeedBackSessionId, int peerFeedBackGroupId, int evaluatorUserId);
        List<PeerFeedBackResponses> PeerFeedBackResponsesGetList(int peerFeedbackId, int peerFeedBackQuestionId, int peerFeedBackSessionId, int peerFeedBackGroupId);

        List<PeerFeedBackResponses> PeerFeedBackResponsesGetList(
            int peerFeedbackId,
            int peerFeedBackSessionId,
            int peerFeedBackGroupId);
        List<PeerFeedBackResponses> PeerFeedBackResponsesGetListByUser(
            int peerFeedbackId,
            int peerFeedBackSessionId,
            int peerFeedBackGroupId, int evaluatorUserId);
        List<PeerFeedBackResponses> PeerFeedBackResponsesGetByTarget(
            int userId,
            int peerFeedbackId,
            int peerFeedBackQuestionId,
            int peerFeedBackSessionId,
            int peerFeedBackGroupId);
        List<PeerFeedBackResponses> PeerFeedBackResponsesGetData(
            int peerFeedbackId,
            int peerFeedBackQuestionId,
            int peerFeedBackSessionId);
        List<PeerFeedBackResponses> PeerFeedBackResponsesGetListWithGroup(int peerFeedbackId, int peerFeedBackSessionId, int peerFeedBackGroupId);
        List<PeerFeedBackResponses> PeerFeedBackResponsesGetListWithGroupAndUser(int peerFeedbackId, int peerFeedBackSessionId, int peerFeedBackGroupId, int evaluatorUserId);
        List<PeerFeedBackResponses> PeerFeedBackResponsesGetPeerFeedBackId(List<int> groups, List<int> sessionIds, List<int> peerFeedBackIds, int? peerFeedBackQuestionId = null);
        List<PeerFeedBackDashboardExportCsvDto> PeerFeedBackResponsesGetDataCsv(List<int> groups, List<int> sessionIds = null, List<int> peerFeedBackIds = null);
        List<PeerFeedBackResponses> PeerFeedBackResponsesByListGroup(int peerFeedBackQuestionId, List<int> groups);
        List<PeerFeedBackResponses> PeerFeedBackResponsesGetByQuestion(int peerFeedBackQuestionId, int targetUserId);
        List<PeerFeedBackResponses> PeerFeedBackResponsesGetByQuestion(int peerFeedBackQuestionId, List<int> peerFeedBackIds, List<int> groups);
        List<PeerFeedBackResponses> PeerFeedBackResponsesGetPeerFeedBack(List<int> groups, List<int> sessionIds, List<int> peerFeedBackIds, int? peerFeedBackQuestionId = null);
        #endregion

        #region PEER_FEEDBACK_RESPONSE_REMARKS

        void PeerFeedBackResponseRemarksInsert(List<PeerFeedBackResponseRemarks> responsesList);
        void PeerFeedBackResponseRemarksInsertOrUpdate(List<PeerFeedBackResponseRemarks> responsesList);
        void PeerFeedBackResponseRemarksDeleteById(int id);
        void PeerFeedBackResponseRemarksDeletePeerFeedBack(int peerFeedBackId, int peerFeedBackSessionId, int peerFeedBackGroupId, int evaluatorUserId);
        List<PeerFeedBackResponseRemarks> PeerFeedBackResponseRemarksGetList(int peerFeedbackId, int peerFeedBackQuestionId, int peerFeedBackSessionId, int peerFeedBackGroupId);

        List<PeerFeedBackResponseRemarks> PeerFeedBackResponseRemarksGetList(
            int peerFeedbackId,
            int peerFeedBackSessionId,
            int peerFeedBackGroupId);
        List<PeerFeedBackResponseRemarks> PeerFeedBackResponseRemarksGetListByUser(
            int peerFeedbackId,
            int peerFeedBackSessionId,
            int peerFeedBackGroupId, int evaluatorUserId);
        List<PeerFeedBackResponseRemarks> PeerFeedBackResponseRemarksGetByTarget(
            int userId,
            int peerFeedbackId,
            int peerFeedBackSessionId,
            int peerFeedBackGroupId);
        List<PeerFeedBackResponseRemarks> PeerFeedBackResponseRemarksGetData(
            int peerFeedbackId,
            int peerFeedBackQuestionId,
            int peerFeedBackSessionId);
        List<PeerFeedBackResponseRemarks> PeerFeedBackResponseRemarksGetListWithGroup(int peerFeedbackId, int peerFeedBackSessionId, int peerFeedBackGroupId);
        List<PeerFeedBackResponseRemarks> PeerFeedBackResponseRemarksGetListWithGroupAndUser(int peerFeedbackId, int peerFeedBackSessionId, int peerFeedBackGroupId, int evaluatorUserId);
        List<PeerFeedBackResponseRemarks> PeerFeedBackResponseRemarksGetPeerFeedBackId(List<int> groups, List<int> sessionIds, List<int> peerFeedBackIds);
        List<PeerFeedBackResponseRemarks> PeerFeedBackResponseRemarksByListGroup(List<int> groups);
        List<PeerFeedBackResponseRemarks> PeerFeedBackResponseRemarksGetByQuestion(int targetUserId);
        List<PeerFeedBackResponseRemarks> PeerFeedBackResponseRemarksGetByQuestion(List<int> peerFeedBackIds, List<int> groups);
        List<PeerFeedBackResponseRemarks> PeerFeedBackResponseRemarksGetPeerFeedBack(List<int> groups, List<int> sessionIds, List<int> peerFeedBackIds);
        #endregion

        #region OTHER
        List<int?> GetUserIdEnrollByCategories(int courseId, List<int> categoryIds);
        #endregion

        #region SEED DATA
        SeedData GetSeedData();
        #endregion

        #region REPORT
        IEnumerable<PeerFeedBackGroupReadinessReportDto> GetGroupReadinessData(List<string> selectedCourseCodes);
        #endregion
    }
}
