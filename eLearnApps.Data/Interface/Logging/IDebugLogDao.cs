using eLearnApps.Entity.Logging;
using System.Threading.Tasks;

namespace eLearnApps.Data.Interface.Logging
{
    public interface IDebugLogDao
    {
        Task Insert(DebugLog debugLog);
    }
}
