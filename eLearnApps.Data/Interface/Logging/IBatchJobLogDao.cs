using eLearnApps.Entity.Logging;

namespace eLearnApps.Data.Interface.Logging
{
    public interface IBatchJobLogDao
    {
        /// <summary>
        /// Insert BatchJobLog
        /// </summary>
        /// <param name="batchJobLog"></param>
        /// <returns></returns>
        int Insert(BatchJobLog batchJobLog);
    }
}