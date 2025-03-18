using eLearnApps.Data.Interface.Lmsisis;
using eLearnApps.Data.Interface.LmsTool;
using eLearnApps.Data.Interface.Logging;

namespace eLearnApps.Data.Interface
{
    public interface IDaoFactory : ILmsToolDaoFactory, ILoggingDaoFactory, ILmsisDaoFactory
    {
    }
}