using eLearnApps.Entity.Logging;

namespace eLearnApps.Business.Interface
{
    public interface IBatchJobLogService
    {
        /// <summary>
        ///     Insert BatchJobLog
        /// </summary>
        /// <param name="batchJobLog"></param>
        /// <returns></returns>
        int Insert(BatchJobLog batchJobLog);
    }
}