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
    public class QuestionDao : BaseDao, IQuestionDao
    {
        public QuestionDao(IConfiguration configuration) : base(configuration)
        {
            ConnectionType = ConnectionStringType.LmsTool;
        }

        /// <summary>
        ///     Get question by list session id
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public List<Question> GetByListSession(List<string> ids)
        {
            try
            {
                using (var conn = CreateConnection(ConnectionString))
                {
                    return conn.Query<Question>(Ics.Questions_GetByListSession,
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="question"></param>
        /// <returns></returns>
        public async Task UpdateQuestionAddressed(Question question)
        {
            try
            {
                using (var conn = CreateConnection(ConnectionString))
                {
                    await conn.ExecuteAsync(Ics.Questions_UpdateAddressed,
                        new
                        {
                            question.Id,
                            question.LastUpdatedOn,
                            question.Addressed
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