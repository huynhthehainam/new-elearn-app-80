using eLearnApps.Business.Interface;
using eLearnApps.Data;
using eLearnApps.Data.Interface.Logging;
using eLearnApps.Data.Logging;
using eLearnApps.Entity.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.Business
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IAuditDao _auditDao;
        public CurrentUserService(IConfiguration configuration)
        {
            _auditDao = new AuditDao(configuration);
        }
        public void SetCurrentUserId(int userId)
        {
            //DataContext.UserID = userId;
        }
        public void SetCurrentCourseId(int courseId)
        {
            //DataContext.CourseId = courseId;
        }

        public void AuditUserActrion(AuditEntry audit)
        {
            _auditDao.Insert(audit.ToAudit());
        }
    }
}
