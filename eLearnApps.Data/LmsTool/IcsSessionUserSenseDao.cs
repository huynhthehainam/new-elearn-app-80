using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using eLearnApps.Core;
using eLearnApps.Data.Interface.LmsTool;
using eLearnApps.Data.Resources;
using eLearnApps.Entity.LmsTools;
using Microsoft.Extensions.Configuration;

namespace eLearnApps.Data.LmsTool
{
    public class IcsSessionUserSenseDao : BaseDao, IIcsSessionUserSenseDao
    {
        public IcsSessionUserSenseDao(IConfiguration configuration) : base(configuration)
        {
            ConnectionType = ConnectionStringType.LmsTool;
        }

        /// <summary>
        ///     Get list by session id
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public List<ICSSessionUserSense> GetBySessionId(int ids)
        {
            try
            {
                using (var conn = CreateConnection(ConnectionString))
                {
                    return conn.Query<ICSSessionUserSense>(Ics.ICSSessionUserSenses_GetListBySessionId, ids, commandType: CommandType.Text).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        ///     Get list by list session id
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public List<ICSSessionUserSense> GetByListSessionId(List<string> ids)
        {
            try
            {
                using (var conn = CreateConnection(ConnectionString))
                {
                    return conn.Query<ICSSessionUserSense>(Ics.ICSSessionUserSenses_GetListBySessionId, new
                    {
                        ids = ids.ToArray()
                    }, commandType: CommandType.Text).ToList();
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
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public async Task UpdateDoingWellBySessionId(int sessionId)
        {
            try
            {
                using (var conn = CreateConnection(ConnectionString))
                {
                    await conn.ExecuteAsync(Ics.ICSSessionUserSenses_UpdateDoingWellBySessionId, new
                    {
                        icsSessionId = sessionId,
                        sense = (int)Senses.DoingWell
                    }, commandType: CommandType.Text);
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
        /// <param name="userSense"></param>
        /// <returns></returns>
        public async Task DeleteByCondition(ICSSessionUserSense userSense)
        {
            try
            {
                using (var conn = CreateConnection(ConnectionString))
                {
                    await conn.ExecuteAsync(Ics.ICSSessionUserSenses_DeleteByIdSessionIdUserId, new
                    {
                        userSense.ICSSessionId,
                        userSense.UserId,
                        userSense.Id
                    }, commandType: CommandType.Text);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}