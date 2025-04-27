using Dapper;
using eLearnApps.Core;
using eLearnApps.Entity.LmsTools;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace eLearnApps.Data.LmsTool
{
    public class PeerTutoringRecordDao : BaseDao, Interface.LmsTool.IPeerTutoringRecordDao
    {
        public PeerTutoringRecordDao(IConfiguration configuration) : base(configuration)
        {
            ConnectionType = ConnectionStringType.LmsTool;
        }

        public void Insert(PeerTutoringRecord newRecord)
        {
            try
            {
                var sql =
                    "INSERT INTO PeerTutoringRecords (" +
                    "OrgUnitId, TargetRole, StudentUserId, SubmittedOn, RecommendedBy, RecommendedOn " +
                    ") VALUES (" +
                    "@OrgUnitId, @TargetRole, @StudentUserId, @SubmittedOn, @RecommendedBy, @RecommendedOn)";

                using (var conn = CreateConnection(ConnectionString))
                {
                    conn.Execute(sql, newRecord, commandType: CommandType.Text);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Update(PeerTutoringRecord newRecord)
        {
            try
            {
                var sql =
                    "UPDATE PeerTutoringRecords " +
                    "SET SubmittedOn=@SubmittedOn, RecommendedBy=@RecommendedBy, RecommendedOn=@RecommendedOn " +
                    "WHERE OrgUnitId=@OrgUnitId AND StudentUserId=@StudentUserId;";

                using (var conn = CreateConnection(ConnectionString))
                {
                    conn.Execute(sql, newRecord, commandType: CommandType.Text);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PeerTutoringRecord> GetRecordByKey(int courseId, int studentUserId)
        {
            var query =
                "SELECT OrgUnitId, TargetRole, StudentUserId, SubmittedOn, RecommendedBy, RecommendedOn " +
                "FROM PeerTutoringRecords " +
                "WHERE OrgUnitId = @CourseId AND StudentUserId = @StudentUserId;";

            using (var conn = CreateConnection(ConnectionString))
            {
                return conn.Query<PeerTutoringRecord>(query,
                    new
                    {
                        CourseId = courseId,
                        StudentUserId = studentUserId
                    },
                    commandType: CommandType.Text).ToList();
            }
        }

        public List<PeerTutoringRecord> GetRecordByKey(int courseId)
        {
            var query =
                "SELECT OrgUnitId, TargetRole, StudentUserId, SubmittedOn, RecommendedBy, RecommendedOn " +
                "FROM PeerTutoringRecords " +
                "WHERE OrgUnitId = @CourseId;";

            using (var conn = CreateConnection(ConnectionString))
            {
                return conn.Query<PeerTutoringRecord>(query,
                    new
                    {
                        CourseId = courseId
                    },
                    commandType: CommandType.Text).ToList();
            }
        }
        public bool CheckExistsRecord(int courseId, int studentUserId)
        {
            var query = "SELECT 1 FROM PeerTutoringRecords WHERE OrgUnitId = @CourseId;";
            using (var conn = CreateConnection(ConnectionString))
            {
                var result = conn.ExecuteScalar<int>(query,
                    new
                    {
                        CourseId = courseId,
                        StudentUserId = studentUserId
                    },
                    commandType: CommandType.Text);
                return result == 1;
            }
        }
        public PeerTutoringRecord GetPeerTutoringRecordByKey(int courseId, int studentUserId)
        {
            var query =
                "SELECT OrgUnitId, TargetRole, StudentUserId, SubmittedOn, RecommendedBy, RecommendedOn " +
                "FROM PeerTutoringRecords " +
                "WHERE OrgUnitId = @CourseId AND StudentUserId = @StudentUserId;";

            using (var conn = CreateConnection(ConnectionString))
            {
                return conn.Query<PeerTutoringRecord>(query,
                    new
                    {
                        CourseId = courseId,
                        StudentUserId = studentUserId
                    },
                    commandType: CommandType.Text).FirstOrDefault();
            }
        }
    }
}
