using System.Collections.Generic;
using System.Threading.Tasks;
using eLearnApps.Entity.LmsIsis;
using eLearnApps.Entity.LmsIsis.Dto;
using eLearnApps.Entity.LmsTools.Dto;

namespace eLearnApps.Data.Interface.Lmsisis
{
    public interface ITLCourseOfferingDao
    {
        /// <summary>
        /// Get merge section by course code
        /// </summary>
        /// <param name="courseOfferingCode"></param>
        /// <returns></returns>
        List<TL_CourseOfferings> SearchByCourseOfferingCode(string courseOfferingCode);
        /// <summary>
        /// Get merge section by course offering code
        /// </summary>
        /// <param name="courseOfferingCode"></param>
        /// <returns></returns>
        bool GetMergeSectionByCourseOfferingCode(string courseOfferingCode);
        /// <summary>
        /// get tl course offering by course offering code
        /// </summary>
        /// <param name="courseOfferingCode"></param>
        /// <returns></returns>
        TL_CourseOfferings GetByCourseOfferingCode(string courseOfferingCode);
        /// <summary>
        /// get tl course offering by list course offering code
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        List<TL_CourseOfferings> GetByListCourseOfferingCode(List<string> list);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strm"></param>
        /// <returns></returns>
        List<DashboardFilterDto> GetPeerFeedBackDashboardData(string strm);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strm"></param>
        /// <returns></returns>
        Task<List<string>> GetListCourseOfferingCode(string strm);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strm"></param>
        /// <returns></returns>
        List<DashboardFilterDto> GetPeerFeedBackCourseOfferingCsv(string strm);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="acad_career"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="filter"></param>
        /// <param name="useFullDbName"></param>
        /// <returns></returns>
        Task<IEnumerable<CourseOfferingDto>> GetListCourseOfferingByCodes(string acad_career, int pageNumber, int pageSize, string filter = "", bool useFullDbName = false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="acad_career"></param>
        /// <param name="filter"></param>
        /// <param name="useFullDbName"></param>
        /// <returns></returns>
        Task<int> GetTotalCountTlCourseOfferingByCodes(string acad_career, string filter = "", bool useFullDbName = false);
    }
}