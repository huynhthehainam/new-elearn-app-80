using System.Collections.Generic;
using System.Threading.Tasks;
using eLearnApps.Entity.LmsTools;

namespace eLearnApps.Data.Interface.LmsTool
{
    public interface IIcsSessionUserSenseDao
    {
        /// <summary>
        /// Get list by session id
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        List<ICSSessionUserSense> GetBySessionId(int sessionId);
        /// <summary>
        /// Get list by list session id
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        List<ICSSessionUserSense> GetByListSessionId(List<string> ids);
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