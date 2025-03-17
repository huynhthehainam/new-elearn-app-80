#region USING

using System;
using System.Collections.Generic;
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
    public class AuditDao : BaseDao, IAuditDao
    {
        public AuditDao(IConfiguration configuration) : base(configuration)
        {
            ConnectionType = ConnectionStringType.LmsTool;
        }
        public async Task Insert(Audit audit)
        {
            try
            {
                using (var conn = CreateConnection(ConnectionString))
                {
                    await conn.ExecuteAsync(Resources.LmsTool.Audit_Insert,
                        new
                        {
                            audit.ToolId,
                            audit.UserId,
                            audit.OrgUnitId,
                            audit.Type,
                            audit.ResourceId,
                            audit.TableName,
                            audit.DateTime,
                            audit.OldValues,
                            audit.NewValues,
                            audit.AffectedColumns,
                            audit.PrimaryKeys
                        },
                        commandType: CommandType.Text);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public async Task Insert(List<Audit> list)
        {
            try
            {
                using (var conn = CreateConnection(ConnectionString))
                {
                    await conn.ExecuteAsync(Resources.LmsTool.Audit_Insert, list, commandType: CommandType.Text);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
