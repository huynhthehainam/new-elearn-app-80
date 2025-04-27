using eLearnApps.Core;
using eLearnApps.Entity.LmsTools;
using eLearnApps.Entity.LmsTools.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eLearnApps.Data.Interface.LmsTool
{
    public interface IPeerFeedBackDao
    {
        /// <summary>
        /// Delete PeerFeedBack
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
        void Delete(int peerFeedBackId, int lastUpdatedBy);
        /// <summary>
        /// Generate pairings along with evaluators and targets in specified session and courses
        /// </summary>
        /// <param name="peerFeedbackId"></param>
        /// <param name="peerFeedBackSessionId"></param>
        /// <param name="courseOfferingCode"></param>
        /// <param name="updatingUserId"></param>
        void GeneratePairings(int peerFeedbackId, int peerFeedBackSessionId, List<string> courseOfferingCode, int updatingUserId);
        /// <summary>
        /// Generate pairings along with evaluators and targets in specified session and courses
        /// </summary>
        /// <param name="dataItems"></param>
        /// <returns></returns>
        Task GeneratePairings(List<(PeerFeedbackPairings Pairing, List<PeerFeedBackPairingSessions> PairingSession, List<PeerFeedbackTargets> Targets, List<PeerFeedbackEvaluators> Evaluators)> dataItems);
        IEnumerable<PeerFeedBackGroupReadinessReportDto> GetGroupReadinessData(List<string> selectedCourseCodes);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="acad_career"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="filter"></param>
        /// <param name="useFullDbName"></param>
        /// <returns></returns>
        Task<IEnumerable<CourseOfferingDto>> GetListCourseOfferingByCodes(AcadCareer acad_career, int pageNumber, int pageSize, string filter = "", bool useFullDbName = false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="acad_career"></param>
        /// <param name="filter"></param>
        /// <param name="useFullDbName"></param>
        /// <returns></returns>
        Task<int> GetTotalCountTlCourseOfferingByCodes(AcadCareer acad_career, string filter = "", bool useFullDbName = false);
    }
}
