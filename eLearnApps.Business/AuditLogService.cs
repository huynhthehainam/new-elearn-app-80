using eLearnApps.Business.Interface;
using eLearnApps.Data;
using eLearnApps.Data.Interface;
using eLearnApps.Data.Interface.Logging;
using eLearnApps.Entity.Logging;
using System.Threading.Tasks;

namespace eLearnApps.Business
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IAuditLogDao _auditLogDao;

        public AuditLogService(IDaoFactory factory)
        {
            _auditLogDao = factory.AuditLogDao;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="auditLog"></param>
        /// <returns></returns>
        public async Task Insert(AuditLog auditLog)
        {
            await _auditLogDao.Insert(auditLog).ConfigureAwait(false);
        }
    }
}