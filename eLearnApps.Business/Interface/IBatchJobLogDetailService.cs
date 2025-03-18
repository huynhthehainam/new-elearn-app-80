using eLearnApps.Entity.Logging;

namespace eLearnApps.Business.Interface
{
    public interface IBatchJobLogDetailService
    {
        /// <summary>
        ///     Insert BatchJobLogDetails
        /// </summary>
        /// <param name="batchJobLogDetail"></param>
        /// <returns></returns>
        int Insert(BatchJobLogDetail batchJobLogDetail);
    }
}