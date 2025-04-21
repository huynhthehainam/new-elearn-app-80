using Dapper;
using eLearnApps.Core;
using eLearnApps.Data.Interface.LmsTool;
using eLearnApps.Entity.LmsTools;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace eLearnApps.Data.LmsTool
{
    public class PeerFeedBackResponsesDao : BaseDao, IPeerFeedBackResponsesDao
    {
        public PeerFeedBackResponsesDao(IConfiguration configuration) : base(configuration)
        {
            ConnectionType = ConnectionStringType.LmsTool;
        }

        public void BulkInsert(List<PeerFeedBackResponses> responses)
        {
            PeerFeedBackResponses schema;
            var dtSource = GetPeerFeedBackResponsesSchema();
            foreach (var response in responses)
            {
                var row = dtSource.NewRow();
                row[nameof(schema.PeerFeedbackId)] = response.PeerFeedbackId;
                row[nameof(schema.PeerFeedbackSessionId)] = response.PeerFeedbackSessionId;
                row[nameof(schema.PeerFeedbackQuestionId)] = response.PeerFeedbackQuestionId;
                row[nameof(schema.TargetUserId)] = response.TargetUserId;
                row[nameof(schema.EvaluatorUserId)] = response.EvaluatorUserId;
                row[nameof(schema.LastUpdateTime)] = response.LastUpdateTime;
                row[nameof(schema.IsDeleted)] = false;
                if (response.PeerFeedBackOptionId.HasValue)
                    row[nameof(schema.PeerFeedBackOptionId)] = response.PeerFeedBackOptionId;

                if (response.PeerFeedBackRatingId.HasValue)
                    row[nameof(schema.PeerFeedBackRatingId)] = response.PeerFeedBackRatingId;

                row[nameof(schema.PeerFeedBackGroupId)] = response.PeerFeedBackGroupId;

                dtSource.Rows.Add(row);
            }

            InsertResponses(dtSource);
        }

        public void Insert(List<PeerFeedBackResponses> responses)
        {
            try
            {
                var sql =
                    "INSERT INTO PeerFeedBackResponses (PeerFeedBackId, PeerFeedBackSessionId, PeerFeedBackQuestionId, TargetUserId, EvaluatorUserId, LastUpdateTime, IsDeleted, PeerFeedBackOptionId, PeerFeedBackRatingId, PeerFeedBackGroupId ) VALUES (@PeerFeedBackId, @PeerFeedBackSessionId, @PeerFeedBackQuestionId, @TargetUserId, @EvaluatorUserId, @LastUpdateTime, @IsDeleted, @PeerFeedBackOptionId, @PeerFeedBackRatingId, @PeerFeedBackGroupId)";
                using (var conn = CreateConnection(ConnectionString))
                {
                    conn.Execute(sql, responses, commandType: CommandType.Text);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Delete(int peerFeedBackId, int peerFeedBackSessionId, int peerFeedBackGroupId, int evaluatorUserId)
        {
            try
            {
                var sql =
                    "DELETE FROM PeerFeedBackResponses WHERE PeerFeedbackId = @peerFeedBackId AND PeerFeedbackSessionId = @peerFeedBackSessionId AND PeerFeedBackGroupId = @PeerFeedBackGroupId AND EvaluatorUserId = @EvaluatorUserId;";
                using (var conn = CreateConnection(ConnectionString))
                {
                    conn.Execute(sql, new { peerFeedBackId, peerFeedBackSessionId, peerFeedBackGroupId, evaluatorUserId },
                        commandType: CommandType.Text);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetPeerFeedBackResponsesSchema()
        {
            var dtSource = new DataTable();
            using (var connection = CreateConnection(ConnectionString))
            {
                using (var com = new SqlCommand("SELECT * FROM PeerFeedBackResponses WHERE id=-1;", connection))
                {
                    using (var reader = com.ExecuteReader())
                    {
                        dtSource.Load(reader);
                    }
                }
            }

            return dtSource;
        }

        public void InsertResponses(DataTable dtSource)
        {
            using (var connection = CreateConnection(ConnectionString))
            {
                using (var bulkCopy = new SqlBulkCopy(connection))
                {
                    bulkCopy.BatchSize = 100;
                    bulkCopy.DestinationTableName = "PeerFeedBackResponses";
                    for (var i = 0; i < dtSource.Columns.Count; i++)
                    {
                        var destinationColumnName = dtSource.Columns[i].ToString();
                        bulkCopy.ColumnMappings.Add(destinationColumnName, destinationColumnName);
                    }

                    try
                    {
                        bulkCopy.WriteToServer(dtSource);
                    }
                    catch (Exception ex)
                    {
                        connection.Close();
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="groups"></param>
        /// <param name="sessionIds"></param>
        /// <param name="peerFeedBackIds"></param>
        /// <param name="peerFeedBackQuestionId"></param>
        /// <returns></returns>
        public List<PeerFeedBackResponses> PeerFeedBackResponsesGetPeerFeedBackId(List<int> groups, List<int> sessionIds, List<int> peerFeedBackIds, int? peerFeedBackQuestionId = null)
        {
            try
            {
                var sb = new System.Text.StringBuilder();
                sb.AppendLine(@"SELECT ");
                sb.AppendLine(@"    response.[Id] AS [Id], ");
                sb.AppendLine(@"    response.[PeerFeedbackId] AS [PeerFeedbackId], ");
                sb.AppendLine(@"    response.[PeerFeedbackQuestionId] AS [PeerFeedbackQuestionId], ");
                sb.AppendLine(@"    response.[PeerFeedbackSessionId] AS [PeerFeedbackSessionId], ");
                sb.AppendLine(@"    response.[TargetUserId] AS [TargetUserId], ");
                sb.AppendLine(@"    response.[EvaluatorUserId] AS [EvaluatorUserId], ");
                sb.AppendLine(@"    response.[LastUpdateTime] AS [LastUpdateTime], ");
                sb.AppendLine(@"    response.[IsDeleted] AS [IsDeleted], ");
                sb.AppendLine(@"    response.[PeerFeedBackRatingId] AS [PeerFeedBackRatingId], ");
                sb.AppendLine(@"    response.[PeerFeedBackOptionId] AS [PeerFeedBackOptionId], ");
                sb.AppendLine(@"    response.[PeerFeedBackGroupId] AS [PeerFeedBackGroupId]");
                sb.AppendLine(@"FROM  ");
                sb.AppendLine(@"	[PeerFeedBackResponses] AS response");
                sb.AppendLine(@"WHERE ");
                sb.AppendLine(@"	(response.[PeerFeedbackSessionId] IN @sessionIds) AND ");
                sb.AppendLine(@"	(response.[PeerFeedbackId] IN @peerFeedBackIds) AND ");
                sb.AppendLine(@"	(response.[PeerFeedBackGroupId] IN @groups)");
                if (peerFeedBackQuestionId.HasValue)
                {
                    sb.AppendLine($"AND (response.[PeerFeedbackQuestionId] = {peerFeedBackQuestionId})");
                }
                using (var conn = CreateConnection(ConnectionString))
                {
                    return conn.Query<PeerFeedBackResponses>(sb.ToString(),
                    new
                    {
                        groups = groups.ToArray(),
                        sessionIds = sessionIds.ToArray(),
                        peerFeedBackIds = peerFeedBackIds.ToArray()
                    }, commandType: CommandType.Text).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}