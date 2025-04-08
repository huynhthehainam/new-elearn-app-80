using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using eLearnApps.Core;
using eLearnApps.Data.Interface.LmsTool;
using eLearnApps.Data.Resources;
using eLearnApps.Entity.LmsTools;
using Microsoft.Extensions.Configuration;

namespace eLearnApps.Data.LmsTool
{
    public class LearningPointCheckDao : BaseDao, ILearningPointCheckDao
    {
        public LearningPointCheckDao(IConfiguration configuration) : base(configuration)
        {
            ConnectionType = ConnectionStringType.LmsTool;
        }

        /// <summary>
        ///     Get list learning point check by list learning point id
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public List<LearningPointCheck> GetByListLearningPoint(List<string> ids)
        {
            try
            {
                using (var conn = CreateConnection(ConnectionString))
                {
                    return conn.Query<LearningPointCheck>(Ics.LearningPointChecks_GetByListLearningPoint,
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