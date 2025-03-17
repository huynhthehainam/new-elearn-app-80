using eLearnApps.Entity.Logging;

namespace eLearnApps.Data.Interface.Logging
{
    public interface IToolAccessLogDao
    {
        /// <summary>
        ///     Insert ToolAccessLog
        /// </summary>
        /// <param name="toolAccessLog"></param>
        /// <returns></returns>
        int Insert(ToolAccessLog toolAccessLog);
    }
}