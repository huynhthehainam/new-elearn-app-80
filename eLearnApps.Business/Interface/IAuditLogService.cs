using eLearnApps.Entity.Logging;
using System.Threading.Tasks;

namespace eLearnApps.Business.Interface
{
    public interface IAuditLogService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="auditLog"></param>
        /// <returns></returns>
        Task Insert(AuditLog auditLog);
    }
}