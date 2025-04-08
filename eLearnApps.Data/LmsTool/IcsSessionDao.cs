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
    public class IcsSessionDao : BaseDao, IIcsSessionDao
    {
        public IcsSessionDao(IConfiguration configuration) : base(configuration)
        {
            ConnectionType = ConnectionStringType.LmsTool;
        }

        /// <summary>
        ///     get Ics session by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ICSSession GetById(int id)
        {
            try
            {
                using (var conn = CreateConnection(ConnectionString))
                {
                    return conn.Query<ICSSession>(Ics.ICSSessions_GetById,
                        new
                        {
                            id
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
        ///     Get list session by course
        /// </summary>
        /// <param name="courseId"></param>
        /// <returns></returns>
        public List<ICSSession> GetByCourse(int courseId)
        {
            try
            {
                using (var conn = CreateConnection(ConnectionString))
                {
                    return conn.Query<ICSSession>(Ics.ICSSessions_GetByCourse,
                        new
                        {
                            courseId
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
        ///     Check user can delete session
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int CanDeleteSession(int id)
        {
            try
            {
                using (var conn = CreateConnection(ConnectionString))
                {
                    return conn.Query<int>(Ics.ICSSessions_CanDelete,
                        new
                        {
                            id
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
        ///     Check exists ics session between start time - end time
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="sessionDate"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public List<ICSSession> CheckRangeStartTimeEndTime(int courseId, DateTime sessionDate, TimeSpan startTime, TimeSpan endTime)
        {
            try
            {
                using (var conn = CreateConnection(ConnectionString))
                {
                    return conn.Query<ICSSession>(Ics.ICCSSessions_CheckRange,
                        new
                        {
                            courseId,
                            sessionDate,
                            startTime,
                            endTime
                        },
                        commandType: CommandType.Text)
                    .ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}