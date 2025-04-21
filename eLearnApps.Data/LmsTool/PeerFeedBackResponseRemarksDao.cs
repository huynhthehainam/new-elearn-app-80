using Dapper;
using eLearnApps.Core;
using eLearnApps.Data.Interface.LmsTool;
using eLearnApps.Entity.LmsTools;
using Microsoft.Extensions.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;

namespace eLearnApps.Data.LmsTool
{
    public class PeerFeedBackResponseRemarksDao : BaseDao, IPeerFeedBackResponseRemarksDao
    {
        public PeerFeedBackResponseRemarksDao(IConfiguration configuration) : base(configuration)
        {
            ConnectionType = ConnectionStringType.LmsTool;
        }
        public void BulkInsert(List<PeerFeedBackResponseRemarks> remarks)
        {
            PeerFeedBackResponseRemarks schema;
            var dtSource = GetPeerFeedBackResponseRemarksSchema();
            foreach (var response in remarks)
            {
                var row = dtSource.NewRow();
                row[nameof(schema.PeerFeedbackId)] = response.PeerFeedbackId;
                row[nameof(schema.PeerFeedbackSessionId)] = response.PeerFeedbackSessionId;
                row[nameof(schema.TargetUserId)] = response.TargetUserId;
                row[nameof(schema.EvaluatorUserId)] = response.EvaluatorUserId;
                row[nameof(schema.LastUpdateTime)] = response.LastUpdateTime;
                row[nameof(schema.IsDeleted)] = false;
                row[nameof(schema.Remarks)] = response.Remarks;
                row[nameof(schema.PeerFeedBackGroupId)] = response.PeerFeedBackGroupId;

                dtSource.Rows.Add(row);
            }

            InsertResponseRemarks(dtSource);
        }

        public void Delete(int peerFeedBackId, int peerFeedBackSessionId, int peerFeedBackGroupId, int evaluatorUserId)
        {
            try
            {
                var sql =
                    @"DELETE FROM PeerFeedBackResponseRemarks 
                    WHERE PeerFeedbackId = @peerFeedBackId 
                        AND PeerFeedbackSessionId = @peerFeedBackSessionId 
                        AND PeerFeedBackGroupId = @PeerFeedBackGroupId 
                        AND EvaluatorUserId = @EvaluatorUserId;";
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

        public void Insert(PeerFeedBackResponseRemarks remarks)
        {
            try
            {
                var sql =
                    @"INSERT INTO PeerFeedBackResponseRemarks (
                        PeerFeedBackId, 
                        PeerFeedBackSessionId, 
                        TargetUserId, 
                        EvaluatorUserId, 
                        LastUpdateTime, 
                        IsDeleted, 
                        PeerFeedBackGroupId,
						Remarks
                    ) 
                     VALUES (
                        @PeerFeedBackId, 
                        @PeerFeedBackSessionId, 
                        @TargetUserId, 
                        @EvaluatorUserId, 
                        @LastUpdateTime, 
                        @IsDeleted, 
                        @PeerFeedBackGroupId,
						@Remarks
                    )";
                using (var conn = CreateConnection(ConnectionString))
                {
                    conn.Execute(sql, remarks, commandType: CommandType.Text);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void Update(PeerFeedBackResponseRemarks remarks)
        {
            try
            {
                var sql =
                @"UPDATE PeerFeedBackResponseRemarks 
                    SET
                        PeerFeedBackId = @PeerFeedBackId, 
                        PeerFeedBackSessionId = @PeerFeedBackSessionId, 
                        TargetUserId = @TargetUserId, 
                        EvaluatorUserId = @EvaluatorUserId, 
                        LastUpdateTime = @LastUpdateTime, 
                        IsDeleted = @IsDeleted, 
                        PeerFeedBackGroupId = @PeerFeedBackGroupId,
						Remarks = @Remarks
                    
                    WHERE Id = @Id";
                using (var conn = CreateConnection(ConnectionString))
                {
                    conn.Execute(sql, remarks, commandType: CommandType.Text);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PeerFeedBackResponseRemarks> PeerFeedBackResponseRemarksGetPeerFeedBackId(List<int> groups, List<int> sessionIds, List<int> peerFeedBackIds)
        {
            try
            {
                var sql = @"SELECT 
                    remark.[Id] AS [Id], 
                    remark.[PeerFeedbackId] AS [PeerFeedbackId], 
                    remark.[PeerFeedbackSessionId] AS [PeerFeedbackSessionId],
                    remark.[TargetUserId] AS [TargetUserId], 
                    remark.[EvaluatorUserId] AS [EvaluatorUserId], 
                    remark.[LastUpdateTime] AS [LastUpdateTime], 
                    remark.[IsDeleted] AS [IsDeleted], 
                    remark.[PeerFeedBackGroupId] AS [PeerFeedBackGroupId],
					remark.[Remarks] AS [Remarks]
                FROM  
                	[PeerFeedBackResponseRemarks] AS remark
                WHERE 
                	(response.[PeerFeedbackSessionId] IN @sessionIds) AND 
                	(response.[PeerFeedbackId] IN @peerFeedBackIds) AND 
                	(response.[PeerFeedBackGroupId] IN @groups)";
                using (var conn = CreateConnection(ConnectionString))
                {
                    return conn.Query<PeerFeedBackResponseRemarks>(sql,
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

        public DataTable GetPeerFeedBackResponseRemarksSchema()
        {
            var dtSource = new DataTable();
            using (var connection = CreateConnection(ConnectionString))
            {
                using (var com = connection.CreateCommand())
                {
                    com.CommandText = "SELECT * FROM PeerFeedBackResponseRemarks WHERE id=-1;";
                    using (var reader = com.ExecuteReader())
                    {
                        dtSource.Load(reader);
                    }
                }
            }

            return dtSource;
        }

        public void InsertResponseRemarks(DataTable dtSource)
        {
            using (var connection = CreateConnection(ConnectionString))
            {
                using (var bulkCopy = new SqlBulkCopy(connection))
                {
                    bulkCopy.BatchSize = 100;
                    bulkCopy.DestinationTableName = "PeerFeedBackResponseRemarks";
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
    }
}
