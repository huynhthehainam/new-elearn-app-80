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
    public class ToolAccessLogDao : BaseDao, IToolAccessLogDao
    {
        public ToolAccessLogDao(IConfiguration configuration) : base(configuration)
        {
            ConnectionType = ConnectionStringType.Logging;
        }
        /// <summary>
        /// Insert  ToolAccessLog
        /// </summary>
        /// <param name="toolAccessLog"></param>
        /// <returns></returns>
        public int Insert(ToolAccessLog toolAccessLog)
        {
            try
            {
                using (var conn = CreateConnection(ConnectionString))
                {
                    return conn.Query<int>(Resources.Logging.ToolAccessLogs_Insert,
                        new
                        {
                            toolAccessLog.ToolId,
                            toolAccessLog.UserId,
                            toolAccessLog.Username,
                            toolAccessLog.AccessTime,
                            toolAccessLog.IpAddress,
                            toolAccessLog.OrgUnitId,
                            toolAccessLog.OrgUnitRoleId,
                            toolAccessLog.SessionId
                        },
                        commandType: CommandType.Text).FirstOrDefault();
                }
            }
            catch
            {
                return 0;
            }
        }
    }
}