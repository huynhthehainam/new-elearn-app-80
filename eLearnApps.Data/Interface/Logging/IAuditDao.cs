using eLearnApps.Entity.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eLearnApps.Data.Interface.Logging
{
    public interface IAuditDao
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="auditLog"></param>
        /// <returns></returns>
        Task Insert(Audit audit);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        Task Insert(List<Audit> list);
    }
}
