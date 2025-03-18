using eLearnApps.Business.Interface;
using eLearnApps.Data;
using eLearnApps.Data.Interface;
using eLearnApps.Data.Interface.Logging;
using eLearnApps.Entity.Logging;

namespace eLearnApps.Business
{
    public class BatchJobLogService : IBatchJobLogService
    {
        private readonly IBatchJobLogDao _batchJobLogDao;

        public BatchJobLogService(IDaoFactory factory)
        {
            _batchJobLogDao = factory.BatchJobLogDao;
        }

        /// <summary>
        ///     Insert batchJobLog
        /// </summary>
        /// <param name="batchJobLog"></param>
        /// <returns></returns>
        public int Insert(BatchJobLog batchJobLog)
        {
            return _batchJobLogDao.Insert(batchJobLog);
        }
    }
}