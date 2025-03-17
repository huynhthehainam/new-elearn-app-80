#region USING

using System;
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
    public class GPTAuditLogDao : BaseDao, IGPTAuditLogDao
    {
        public GPTAuditLogDao(IConfiguration configuration) : base(configuration)
        {
            ConnectionType = ConnectionStringType.Logging;
        }
        /// <summary>
        ///     Insert log
        /// </summary>
        /// <param name="auditLog"></param>
        /// <returns></returns>
        public int Insert(GPTAuditLog auditLog)
        {
            try
            {
                using (var conn = CreateConnection(ConnectionString))
                {
                    return conn.Execute(Resources.Logging.GPTAuditLog_Insert,
                        new
                        {
                            auditLog.Action,
                            auditLog.ActionByUserId,
                            auditLog.ActionByName,
                            auditLog.CourseId,
                            auditLog.CourseName,
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