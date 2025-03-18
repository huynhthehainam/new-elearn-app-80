using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using eLearnApps.Core;
using eLearnApps.Data.Interface.LmsTool;
using eLearnApps.Data.Resources;
using eLearnApps.Entity.LmsTools.Dto;
using Microsoft.Extensions.Configuration;

namespace eLearnApps.Data.LmsTool
{
    public class UserEnrollmentDao : BaseDao, IUserEnrollmentDao
    {
        public UserEnrollmentDao(IConfiguration configuration) : base(configuration)
        {
            ConnectionType = ConnectionStringType.LmsTool;
        }

        /// <summary>
        ///     Get list user id enroll
        /// </summary>
        /// <param name="courseId"></param>
        /// <returns></returns>
        public List<int> GetUserIdEnrollByCourse(int courseId)
        {
            try
            {
                using (var conn = CreateConnection(ConnectionString))
                {
                    return conn.Query<int>(Users.UserEnrollments_GetListUserIdEnroll,
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
        ///     Get list user id enroll
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public List<int> GetAllStudentByCourse(int courseId, int roleId)
        {
            try
            {
                using (var conn = CreateConnection(ConnectionString))
                {
                    return conn.Query<int>(Users.UserEnrollments_GetAllStudentByCourse,
                        new
                        {
                            courseId,
                            roleId
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
        /// <param name="courseId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<UserDto> GetUserEnrollByCourseIdAndUserIdAsync(int courseId, int userId)
        {
            try
            {
                using (var conn = CreateConnection(ConnectionString))
                {
                    return await conn.QueryFirstOrDefaultAsync<UserDto>(Users.UserEnrollments_GetUserEnrollByCourseIdAndUserIdAsync,
                        new
                        {
                            courseId,
                            userId
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