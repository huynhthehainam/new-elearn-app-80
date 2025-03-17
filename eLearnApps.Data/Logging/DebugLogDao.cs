using System.Data;
using System.Threading.Tasks;
using Dapper;
using eLearnApps.Core;
using eLearnApps.Data.Interface.Logging;
using eLearnApps.Entity.Logging;
using Microsoft.Extensions.Configuration;

namespace eLearnApps.Data.Logging
{
    public class DebugLogDao : BaseDao, IDebugLogDao
    {
        public DebugLogDao(IConfiguration configuration) : base(configuration)
        {
            ConnectionType = ConnectionStringType.Logging;
        }

        /// <summary>
        ///     Insert log
        /// </summary>
        /// <param name="debugLog"></param>
        /// <returns></returns>
        public async Task Insert(DebugLog debugLog)
        {
            try
            {
                using (var conn = CreateConnection(ConnectionString))
                {
                    if (conn != null)
                        await conn.ExecuteAsync(Resources.Logging.DebugLog_Insert, debugLog,
                            commandType: CommandType.Text);
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}