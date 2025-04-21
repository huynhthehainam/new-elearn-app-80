using Dapper;
using eLearnApps.Core;
using eLearnApps.Data.Interface.Lmsisis;
using eLearnApps.Entity.LmsIsis;
using eLearnApps.Entity.LmsIsis.Dto;
using eLearnApps.Entity.LmsTools.Dto;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace eLearnApps.Data.Lmsisis
{
    public class TLCourseOfferingDao : BaseDao, ITLCourseOfferingDao
    {
        public TLCourseOfferingDao(IConfiguration configuration) : base(configuration)
        {
            ConnectionType = ConnectionStringType.Lmsisis;
        }

        /// <summary>
        ///     Get merge section by course code
        /// </summary>
        /// <param name="courseOfferingCode"></param>
        /// <returns></returns>
        public List<TL_CourseOfferings> SearchByCourseOfferingCode(string courseOfferingCode)
        {
            try
            {
                using (var conn = CreateConnection(ConnectionString))
                {
                    return conn.Query<TL_CourseOfferings>(Resources.Lmsisis.TL_CourseOfferingCode_GetByCourseCode,
                        new
                        {
                            courseOfferingCode
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
        ///     Get merge section by course offering code
        /// </summary>
        /// <param name="courseOfferingCode"></param>
        /// <returns></returns>
        public bool GetMergeSectionByCourseOfferingCode(string courseOfferingCode)
        {
            try
            {
                using (var conn = CreateConnection(ConnectionString))
                {
                    return conn.Query<bool>(Resources.Lmsisis.TL_CourseOfferingCode_GetByCourseCode,
                        new
                        {
                            courseOfferingCode
                        },
                        commandType: CommandType.Text).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        ///     get tl course offering by course offering code
        /// </summary>
        /// <param name="courseOfferingCode"></param>
        /// <returns></returns>
        public TL_CourseOfferings GetByCourseOfferingCode(string courseOfferingCode)
        {
            try
            {
                using (var conn = CreateConnection(ConnectionString))
                {
                    return conn.Query<TL_CourseOfferings>(Resources.Lmsisis.TL_CourseOfferings_GetByCourseOfferingCode,
                        new
                        {
                            courseOfferingCode
                        },
                        commandType: CommandType.Text).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// get tl course offering by list course offering code
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<TL_CourseOfferings> GetByListCourseOfferingCode(List<string> list)
        {
            try
            {
                using (var conn = CreateConnection(ConnectionString))
                {
                    return conn.Query<TL_CourseOfferings>(Resources.Lmsisis.TL_CourseOfferings_GetByListCourseOfferingCode,
                        new
                        {
                            list = list.Distinct().ToArray()
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
        /// <param name="strm"></param>
        /// <returns></returns>
        public List<DashboardFilterDto> GetPeerFeedBackDashboardData(string strm)
        {
            try
            {
                var sql = Resources.Lmsisis.TL_CourseOfferings_GetPeerFeedBackDashboard;
                if (!string.IsNullOrEmpty(strm))
                {
                    sql += " AND (tl.[STRM] = @strm or ps.[STRM] = @strm )";
                }
                using (var conn = CreateConnection(ConnectionString))
                {
                    return conn.Query<DashboardFilterDto>(sql, new { strm },
                        commandType: CommandType.Text).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Get list course offering code by strm
        /// </summary>
        /// <param name="strm"></param>
        /// <returns></returns>
        public async Task<List<string>> GetListCourseOfferingCode(string strm)
        {
            try
            {
                var sql = Resources.Lmsisis.TL_CourseOfferingCode_GetListCourseOfferingCodeByStrm;
                if (!string.IsNullOrEmpty(strm))
                {
                    sql += " AND [STRM] = @strm";
                }
                using (var conn = CreateConnection(ConnectionString))
                {
                    var result = await conn.QueryAsync<string>(sql, new
                    {
                        strm
                    }, commandType: CommandType.Text);
                    return result.ToList();
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
        /// <param name="strm"></param>
        /// <returns></returns>
        public List<DashboardFilterDto> GetPeerFeedBackCourseOfferingCsv(string strm)
        {
            try
            {
                var sql = Resources.Lmsisis.TL_CourseOfferings_GetPeerFeedBackDownloadCSV;
                if (!string.IsNullOrEmpty(strm))
                {
                    sql += " AND (tl.[STRM] = @strm or ps.[STRM] = @strm )";
                }
                using (var conn = CreateConnection(ConnectionString))
                {
                    return conn.Query<DashboardFilterDto>(sql, new { strm },
                        commandType: CommandType.Text).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<CourseOfferingDto>> GetListCourseOfferingByCodes(string acad_career, int pageNumber, int pageSize, string filter = "", bool useFullDbName = false)
        {
            try
            {
                var alias = $"{DatabaseName.LMSTools.ToString()}.{(useFullDbName ? "dbo." : "")}";
                if (!string.IsNullOrEmpty(filter))
                    filter = $"%{filter}%";
                string query = string.Format(Resources.Lmsisis.TL_CourseOfferings_GetListByCodes, alias);
                using (var conn = CreateConnection(ConnectionString))
                {
                    var result = await conn.QueryAsync<CourseOfferingDto>(query,
                        new
                        {
                            pageSize,
                            pageNumber,
                            acad_career,
                            filter
                        }, commandType: CommandType.Text);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<int> GetTotalCountTlCourseOfferingByCodes(string acad_career, string filter = "", bool useFullDbName = false)
        {
            try
            {
                var alias = $"{DatabaseName.LMSTools.ToString()}.{(useFullDbName ? "dbo." : "")}";
                if (!string.IsNullOrEmpty(filter))
                    filter = $"%{filter}%";
                string query = string.Format(Resources.Lmsisis.TL_CourseOfferings_GetTotalCountByCodes, alias);
                using (var conn = CreateConnection(ConnectionString))
                {
                    var result = await conn.ExecuteScalarAsync<int>(query,
                        new
                        {
                            filter,
                            acad_career
                        }, commandType: CommandType.Text);
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}