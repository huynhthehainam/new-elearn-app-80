using eLearnApps.Data;

namespace eLearnApps.Business
{
    public class BaseService
    {
        protected IDbContext _dbContext { get; set; }
        protected IDbContext CmtDbContext { get; set; }
    }
}