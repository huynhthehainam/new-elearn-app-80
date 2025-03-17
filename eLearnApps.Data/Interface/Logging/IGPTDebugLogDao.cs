using eLearnApps.Entity.Logging;
using System.Collections.Generic;

namespace eLearnApps.Data.Interface.Logging
{
    public interface IGPTDebugLogDao
    {
        /// <summary>
        /// Insert AuditLog
        /// </summary>
        /// <param name="auditLog"></param>
        /// <returns></returns>
        int Insert(GPTDebugLog debugLog);
        List<GPTDebugLog> GetAll();
    }
}