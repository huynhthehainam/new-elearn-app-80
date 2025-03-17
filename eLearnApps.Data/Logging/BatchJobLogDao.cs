using System;
using System.Data;
using System.Linq;
using Dapper;
using eLearnApps.Core;
using eLearnApps.Data.Interface.Logging;
using eLearnApps.Entity.Logging;
using Microsoft.Extensions.Configuration;

namespace eLearnApps.Data.Logging
{
    public class BatchJobLogDao : BaseDao, IBatchJobLogDao
    {
        public BatchJobLogDao(IConfiguration configuration) : base(configuration)
        {
            ConnectionType = ConnectionStringType.Logging;
        }
        /// <summary>
        ///     Insert BatchJobLogs
        /// </summary>
        /// <param name="batchJobLog"></param>
        /// <returns></returns>
        public int Insert(BatchJobLog batchJobLog)
        {
            try
            {
                using (var conn = CreateConnection(ConnectionString))
                {
                    return conn.Query<int>(Resources.Logging.BatchJobLogs_Insert,
                        new
                        {
                            batchJobLog.ExecutableName,
                            batchJobLog.Parameters,
                            batchJobLog.JobStart,
                            batchJobLog.JobEnd,
                            batchJobLog.CreatedOn,
                            batchJobLog.LastUpdatedOn
                        },
                        commandType: CommandType.Text).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}