using eLearnApps.Entity.LmsTools;
using eLearnApps.Entity.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eLearnApps.Business.Interface
{
    public interface IAuditService
    {
        /// <summary>
        /// Insert audit log
        /// </summary>
        /// <param name="audit"></param>
        /// <returns></returns>
        Task InsertAsync(Audit audit);
        List<Audit> GetListAudit();
        List<Audit> GetListAuditByUserId(int UserId);
        List<UserClickedResources> CheckUserClickedResources(List<int> UserIds);
    }
}
