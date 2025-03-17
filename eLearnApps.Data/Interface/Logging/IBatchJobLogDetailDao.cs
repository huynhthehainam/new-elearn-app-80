using eLearnApps.Entity.Logging;

namespace eLearnApps.Data.Interface.Logging
{
    public interface IBatchJobLogDetailDao
    {
        /// <summary>
        ///     Insert BatchJobLogDetails
        /// </summary>
        /// <param name="batchJobLogDetail"></param>
        /// <returns></returns>
        int Insert(BatchJobLogDetail batchJobLogDetail);
    }
}