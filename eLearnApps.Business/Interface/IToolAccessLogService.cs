using eLearnApps.Entity.Logging;

namespace eLearnApps.Business.Interface
{
    public interface IToolAccessLogService
    {
        /// <summary>
        ///     Insert ToolAccessLog
        /// </summary>
        /// <param name="toolAccessLog"></param>
        /// <returns></returns>
        int Insert(ToolAccessLog toolAccessLog);
    }
}