using eLearnApps.Business.Interface;
using eLearnApps.Core;
using eLearnApps.Data;
using eLearnApps.Data.Interface;
using eLearnApps.Data.Interface.Logging;
using eLearnApps.Entity.LmsTools;
using eLearnApps.Entity.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eLearnApps.Business
{
    public class AuditService : IAuditService
    {
        private readonly IRepository<Audit> _repositoryAudit;
        private readonly IRepository<PeerFeedbackQuestion> _repositoryQuestion;
        private readonly IAuditDao _auditDao;
        public AuditService(IServiceProvider serviceProvider, IDaoFactory factory)
        {
            _repositoryAudit = serviceProvider.GetRequiredKeyedService<IRepository<Audit>>("default"); ;
            _repositoryQuestion = serviceProvider.GetRequiredKeyedService<IRepository<PeerFeedbackQuestion>>("default"); ;
            _auditDao = factory.AuditDao;
        }
        public async Task InsertAsync(Audit audit)
        {
            await _auditDao.Insert(audit);
        }
        public List<Audit> GetListAudit()
        {
            return _repositoryAudit.Table.ToList();
        }
        public List<Audit> GetListAuditByUserId(int userId)
        {
            return _repositoryAudit.Table.Where(x => x.UserId == userId.ToString()).ToList();
        }

        public List<UserClickedResources> CheckUserClickedResources(List<int> userIds)
        {
            return (from a in _repositoryAudit.TableNoTracking
                    join b in userIds on a.UserId equals b.ToString()
                    where a.Type == AuditType.Click.ToString()
                    select new { a.UserId, a.OrgUnitId, a.ResourceId }).Distinct().ToList().Select(x => new UserClickedResources
                    {
                        UserId = Convert.ToInt32(x.UserId),
                        Resource = x.ResourceId,
                        OrgUnitId = x.OrgUnitId
                    }).ToList();
        }

    }
}
