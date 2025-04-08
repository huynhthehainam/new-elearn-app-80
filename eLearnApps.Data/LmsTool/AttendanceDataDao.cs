using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using eLearnApps.Core;
using eLearnApps.Data.Interface.LmsTool;
using eLearnApps.Entity.LmsTools;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace eLearnApps.Data.LmsTool
{
    public class AttendanceDataDao : BaseDao, IAttendanceDataDao
    {
        public AttendanceDataDao(IConfiguration configuration) : base(configuration)
        {
            ConnectionType = ConnectionStringType.LmsTool;
        }

        /// <summary>
        ///     insert list attendance data
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public int Insert(List<AttendanceData> list)
        {
            try
            {
                using (var conn = CreateConnection(ConnectionString))
                {
                    return conn.Execute(Resources.LmsTool.AttendanceData_Insert, list, commandType: CommandType.Text);
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
        /// <param name="data"></param>
        /// <returns></returns>
        public void Insert(AttendanceData data)
        {
            try
            {
                using (var conn = CreateConnection(ConnectionString))
                {
                    conn.Execute(Resources.LmsTool.AttendanceData_Insert, data, commandType: CommandType.Text);
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
        /// <param name="data"></param>
        /// <returns></returns>
        public void Update(AttendanceData data)
        {
            try
            {
                using (var conn = CreateConnection(ConnectionString))
                {
                    conn.Execute(Resources.LmsTool.AttendanceData_Update, data, commandType: CommandType.Text);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        ///     update percent list attendance data
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public int Update(List<AttendanceData> list)
        {
            try
            {
                using (var conn = CreateConnection(ConnectionString))
                {
                    return conn.Execute(Resources.LmsTool.AttendanceData_UpdatePercent, list,
                        commandType: CommandType.Text);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int UpdateForSetAll(AttendanceData attendanceData)
        {
            try
            {
                using (var conn = CreateConnection(ConnectionString))
                {
                    return conn.Execute(Resources.LmsTool.AttendanceData_UpdateForSetAll, attendanceData,
                        commandType: CommandType.Text);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int SoftDelete(List<int> AttendanceDataIds)
        {
            try
            {
                using (var conn = CreateConnection(ConnectionString))
                {
                    var sqlcmd = @"UPDATE AttendanceDatas SET IsDeleted = 1 WHERE AttendanceDataId IN @AttendanceDataIds";
                    return conn.Execute(sqlcmd, new { AttendanceDataIds },
                        commandType: CommandType.Text);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Insert or update
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public int Save(List<AttendanceData> list)
        {
            try
            {
                using (var conn = CreateConnection(ConnectionString))
                {
                    return conn.Execute(Resources.LmsTool.AttendanceData_Save, list, commandType: CommandType.Text);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Bulk insert attendance data
        /// </summary>
        /// <param name="dtSource"></param>
        public void InsertAttendanceData(DataTable dtSource)
        {
            using (var connection = CreateConnection(ConnectionString))
            {
                AttendanceData schema = new AttendanceData();
                using (var bulkCopy = new SqlBulkCopy(connection))
                {
                    bulkCopy.BatchSize = 100;
                    bulkCopy.DestinationTableName = "AttendanceDatas";
                    bulkCopy.ColumnMappings.Add(nameof(schema.AttendanceSessionId), nameof(schema.AttendanceSessionId));
                    bulkCopy.ColumnMappings.Add(nameof(schema.UserId), nameof(schema.UserId));
                    bulkCopy.ColumnMappings.Add(nameof(schema.Percentage), nameof(schema.Percentage));
                    bulkCopy.ColumnMappings.Add(nameof(schema.Remarks), nameof(schema.Remarks));
                    bulkCopy.ColumnMappings.Add(nameof(schema.LastUpdatedBy), nameof(schema.LastUpdatedBy));
                    bulkCopy.ColumnMappings.Add(nameof(schema.LastUpdatedTime), nameof(schema.LastUpdatedTime));
                    bulkCopy.ColumnMappings.Add(nameof(schema.IsDeleted), nameof(schema.IsDeleted));
                    try
                    {
                        bulkCopy.WriteToServer(dtSource);
                    }
                    catch 
                    {
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// get schema attendance data
        /// </summary>
        /// <returns></returns>
        public DataTable GetAttendanceDataSchema()
        {
            AttendanceData schema = new AttendanceData();
            DataTable dtSource = new DataTable();
            dtSource.Columns.Add(nameof(schema.AttendanceSessionId), typeof(int));
            dtSource.Columns.Add(nameof(schema.UserId), typeof(int));
            dtSource.Columns.Add(nameof(schema.Percentage), typeof(decimal));
            dtSource.Columns.Add(nameof(schema.Remarks), typeof(string));
            dtSource.Columns.Add(nameof(schema.LastUpdatedBy), typeof(int));
            dtSource.Columns.Add(nameof(schema.LastUpdatedTime), typeof(DateTime));
            dtSource.Columns.Add(nameof(schema.IsDeleted), typeof(bool));
            return dtSource;
        }

        /// <summary>
        /// Update attendance data
        /// </summary>
        /// <param name="listAttendanceData"></param>
        public void UpdateAttendanceData(List<AttendanceData> listAttendanceData)
        {
            try
            {
                var item = listAttendanceData[0];
                string query = string.Format( Resources.LmsTool.AttendanceData_UpdateAttendanceData, item.AttendanceSessionId, item.LastUpdatedBy);
                using (var conn = CreateConnection(ConnectionString))
                {
                    int count = listAttendanceData.Count;
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = conn;
                        command.CommandText = query;
                        command.CommandType = CommandType.Text;
                        for (int i = 0; i < count; i++)
                        {
                            var attendanceData = listAttendanceData[i];
                            command.Parameters.AddWithValue("@UserId", attendanceData.UserId);
                            command.Parameters.AddWithValue("@Percentage", attendanceData.Percentage ?? (object) DBNull.Value);
                            command.Parameters.AddWithValue("@AttendanceDataId", attendanceData.AttendanceDataId);
                            command.ExecuteNonQuery();
                            command.Parameters.Clear();
                        }
                    }
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
        /// <param name="attendanceSessionId"></param>
        /// <returns></returns>
        public List<AttendanceData> GetBySession(int attendanceSessionId)
        {
            try
            {
                using (var conn = CreateConnection(ConnectionString))
                {
                    var result = conn.Query<AttendanceData>(Resources.LmsTool.AttendanceData_GetBySessionId, new { attendanceSessionId }, commandType: CommandType.Text);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}