using eLearnApps.Core;
using eLearnApps.Entity.Logging;
using eLearnApps.Entity.Logging.Dto;
using System;
using System.Threading.Tasks;

namespace eLearnApps.Business.Interface
{
    public interface ILoggingService
    {
        int Save(LogType logType, Logging logging);

        int Error(int userId, string toolId, int orgUnitId, int roleId, string page, string errorMessage, string errorDetails,
            string IpAddress, string sessionid);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="toolId"></param>
        /// <param name="orgUnitId"></param>
        /// <param name="roleId"></param>
        /// <param name="actionCategory"></param>
        /// <param name="actionDescription"></param>
        /// <param name="actionTime"></param>
        /// <param name="ipAddress"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        Task Audit(int userId, string toolId, int orgUnitId, int roleId, string actionCategory, string actionDescription, DateTime actionTime, string ipAddress, string sessionId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="debugLog"></param>
        /// <returns></returns>
        Task Debug(DebugLog debugLog);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="audit"></param>
        void AuditUserAction(AuditEntry audit);
    }
}