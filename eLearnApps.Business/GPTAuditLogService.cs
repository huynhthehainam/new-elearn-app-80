using eLearnApps.Business.Interface;
using eLearnApps.Data;
using eLearnApps.Data.Interface;
using eLearnApps.Data.Interface.Logging;
using eLearnApps.Entity.Logging;

namespace eLearnApps.Business
{
    public class GPTAuditLogService : IGPTAuditLogService
    {
        private readonly IGPTAuditLogDao _gptAuditLogDao;

        public GPTAuditLogService(IDaoFactory factory)
        {
         
            _gptAuditLogDao = factory.GPTAuditLogDao;
        }

        /// <summary>
        ///     Insert Audit log
        /// </summary>
        /// <param name="auditLog"></param>
        /// <returns></returns>
        public int Insert(GPTAuditLog gptAuditLog)
        {
            return _gptAuditLogDao.Insert(gptAuditLog);
        }
    }
}