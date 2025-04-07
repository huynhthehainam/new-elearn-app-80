using eLearnApps.Entity.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.Business.Interface
{
    public interface ICurrentUserService
    {
        void SetCurrentUserId(int userId);
        void SetCurrentCourseId(int courseId);
        void AuditUserActrion(AuditEntry audit);
    }
}
