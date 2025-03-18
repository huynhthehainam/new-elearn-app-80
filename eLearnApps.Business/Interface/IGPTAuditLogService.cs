using eLearnApps.Entity.Logging;

namespace eLearnApps.Business.Interface
{
    public interface IGPTAuditLogService
    {
        /// <summary>
        ///     Insert GPTAuditLog
        /// </summary>
        /// <param name="gptAuditLog"></param>
        /// <returns></returns>
        int Insert(GPTAuditLog gptAuditLog);
    }
}