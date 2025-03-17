using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using eLearnApps.Core;
using eLearnApps.Data.Interface.Logging;
using eLearnApps.Entity.Logging;
using Microsoft.Extensions.Configuration;

namespace eLearnApps.Data.Logging
{
    public class ErrorLogDao : BaseDao, IErrorLogDao
    {
        public ErrorLogDao(IConfiguration configuration) : base(configuration)
        {
            ConnectionType = ConnectionStringType.Logging;
        }
        /// <summary>
        ///     Insert ErrorLog
        /// </summary>
        /// <param name="errorLog"></param>
        /// <returns></returns>
        public int Insert(ErrorLog errorLog)
        {
            try
            {
                using (var conn = CreateConnection(ConnectionString))
                {
                    return conn.Query<int>(Resources.Logging.ErrorLogs_Insert,
                        new
                        {
                            errorLog.UserId,
                            errorLog.ToolId,
                            errorLog.OrgUnitId,
                            errorLog.ToolAccessRoleId,
                            errorLog.ErrorPage,
                            errorLog.ErrorMessage,
                            errorLog.ErrorDetails,
                            errorLog.ErrorTime,
                            errorLog.IpAddress,
                            errorLog.SessionId
                        },
                        commandType: CommandType.Text).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get all ErrorLog records
        /// </summary>
        /// <returns></returns>
        public List<ErrorLog> GetAll()
        {
            try
            {
                using (var conn = CreateConnection(ConnectionString))
                {
                    return conn.Query<ErrorLog>("SELECT * FROM ErrorLogs WHERE DATEDIFF(m, ErrorTime, GETDATE()) = 0 ORDER BY ErrorLogId DESC;",
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