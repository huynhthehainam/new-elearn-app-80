using eLearnApps.Entity.Logging;
using System.Threading.Tasks;

namespace eLearnApps.Data.Interface.Logging
{
    public interface IAuditLogDao
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="auditLog"></param>
        /// <returns></returns>
        Task Insert(AuditLog auditLog);
    }
}