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
    public class BatchJobLogDetailDao : BaseDao, IBatchJobLogDetailDao
    {
        public BatchJobLogDetailDao(IConfiguration configuration) : base(configuration)
        {
            ConnectionType = ConnectionStringType.Logging;
        }
        /// <summary>
        ///     Insert BatchJobLogDetails
        /// </summary>
        /// <param name="batchJobLogDetail"></param>
        /// <returns></returns>
        public int Insert(BatchJobLogDetail batchJobLogDetail)
        {
            try
            {
                using (var conn = CreateConnection(ConnectionString))
                {
                    return conn.Query<int>(Resources.Logging.BatchJobLogDetails_Insert,
                        new
                        {
                            batchJobLogDetail.BatchJobLogId,
                            batchJobLogDetail.LogLevel,
                            batchJobLogDetail.LogContent,
                            batchJobLogDetail.CreatedOn
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