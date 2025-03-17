#region USING

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using eLearnApps.Core;
using eLearnApps.Data.Interface.Logging;
using eLearnApps.Entity.Logging;
using Microsoft.Extensions.Configuration;

#endregion

namespace eLearnApps.Data.Logging
{
    public class GPTDebugLogDao : BaseDao, IGPTDebugLogDao
    {
        public GPTDebugLogDao(IConfiguration configuration) : base(configuration)
        {
            ConnectionType = ConnectionStringType.Logging;
        }
        /// <summary>
        ///     Insert log
        /// </summary>
        /// <param name="auditLog"></param>
        /// <returns></returns>
        public int Insert(GPTDebugLog gptDebugLog)
        {
            try
            {
                using (var conn = CreateConnection(ConnectionString))
                {
                    return conn.Execute(Resources.Logging.GPTDebugLog_Insert,
                        new
                        {

                            gptDebugLog.Description,
                            gptDebugLog.CreatedOn,
                            gptDebugLog.SessionId
                        },
                        commandType: CommandType.Text);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<GPTDebugLog> GetAll()
        {
            try
            {
                using (var conn = CreateConnection(ConnectionString))
                {
                    return conn.Query<GPTDebugLog>("SELECT * FROM GPTDebugLogs WHERE DATEDIFF(m, CreatedOn, GETDATE()) = 0 ORDER BY Id DESC;",
                        commandType: CommandType.Text).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}