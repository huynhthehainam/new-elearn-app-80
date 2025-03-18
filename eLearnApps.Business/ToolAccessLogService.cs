using eLearnApps.Business.Interface;
using eLearnApps.Data;
using eLearnApps.Data.Interface;
using eLearnApps.Data.Interface.Logging;
using eLearnApps.Entity.Logging;

namespace eLearnApps.Business
{
    public class ToolAccessLogService : IToolAccessLogService
    {
        private readonly IToolAccessLogDao _toolAccessLogDao;

        public ToolAccessLogService(IDaoFactory factory)
        {
            _toolAccessLogDao = factory.ToolAccessLogDao;
        }

        /// <summary>
        ///     Insert tool access log
        /// </summary>
        /// <param name="toolAccessLog"></param>
        /// <returns></returns>
        public int Insert(ToolAccessLog toolAccessLog)
        {
            return _toolAccessLogDao.Insert(toolAccessLog);
        }
    }
}