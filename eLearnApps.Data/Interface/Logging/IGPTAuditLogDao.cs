using eLearnApps.Entity.Logging;

namespace eLearnApps.Data.Interface.Logging
{
    public interface IGPTAuditLogDao
    {
        /// <summary>
        /// Insert AuditLog
        /// </summary>
        /// <param name="auditLog"></param>
        /// <returns></returns>
        int Insert(GPTAuditLog auditLog);
    }
}