#region USING

using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using eLearnApps.Core;
using eLearnApps.Data.Interface.Logging;
using eLearnApps.Entity.Logging;
using Microsoft.Extensions.Configuration;

#endregion

namespace eLearnApps.Data.Logging
{
    public class AuditLogDao : BaseDao, IAuditLogDao
    {
        public AuditLogDao(IConfiguration configuration) : base(configuration)
        {
            ConnectionType = ConnectionStringType.Logging;
        }
        /// <summary>
        ///     Insert log
        /// </summary>
        /// <param name="auditLog"></param>
        /// <returns></returns>
        public async Task Insert(AuditLog auditLog)
        {
            try
            {
                using (var conn = CreateConnection(ConnectionString))
                {
                    await conn.ExecuteAsync(Resources.Logging.AuditLog_Insert,
                        new
                        {
                            auditLog.UserId,
                            auditLog.ToolId,
                            auditLog.OrgUnitId,
                            auditLog.ToolAccessRoleId,
                            auditLog.ActionCategory,
                            ActionDescription = auditLog.ActionDescription??string.Empty,
                            auditLog.ActionTime,
                            auditLog.IpAddress,
                            auditLog.SessionId
                        },
                        commandType: CommandType.Text);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}