using eLearnApps.Business.Interface;
using eLearnApps.Data;
using eLearnApps.Data.Interface;
using eLearnApps.Data.Interface.Logging;
using eLearnApps.Entity.Logging;

namespace eLearnApps.Business
{
    public class BatchJobLogDetailService : IBatchJobLogDetailService
    {
        private readonly IBatchJobLogDetailDao _batchJobLogDetailDao;

        public BatchJobLogDetailService(IDaoFactory factory)
        {
            _batchJobLogDetailDao = factory.BatchJobLogDetailDao;
        }

        /// <summary>
        ///     Insert batchJobLogDetail
        /// </summary>
        /// <param name="batchJobLogDetail"></param>
        /// <returns></returns>
        public int Insert(BatchJobLogDetail batchJobLogDetail)
        {
            return _batchJobLogDetailDao.Insert(batchJobLogDetail);
        }
    }
}