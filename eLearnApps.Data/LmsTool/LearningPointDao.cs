using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using eLearnApps.Core;
using eLearnApps.Data.Interface.LmsTool;
using eLearnApps.Data.Resources;
using eLearnApps.Entity.LmsTools.Dto;
using Microsoft.Extensions.Configuration;

namespace eLearnApps.Data.LmsTool
{
    public class LearningPointDao : BaseDao, ILearningPointDao
    {
        public LearningPointDao(IConfiguration configuration) : base(configuration)
        {
            ConnectionType = ConnectionStringType.LmsTool;
        }

        /// <summary>
        ///     Get learning point by list session id
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public List<LearningPointDto> GetByListSession(List<string> ids)
        {
            try
            {
                using (var conn = CreateConnection(ConnectionString))
                {
                    return conn.Query<LearningPointDto>(Ics.LearningPoints_GetByListSession,
                        new
                        {
                            ids = ids.ToArray()
                        },
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