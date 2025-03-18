using Dapper;
using eLearnApps.Core;
using eLearnApps.Data.Interface.LmsTool;
using eLearnApps.Data.Resources;
using eLearnApps.Entity.LmsTools;
using eLearnApps.Entity.LmsTools.Dto;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace eLearnApps.Data.LmsTool
{
    public class UserDao : BaseDao, IUserDao
    {
        public UserDao(IConfiguration configuration) : base(configuration)
        {
            ConnectionType = ConnectionStringType.LmsTool;
        }
        /// <summary>
        /// Get all user in this course
        /// </summary>
        /// <param name="courseId"></param>
        /// <returns></returns>
        public List<User> GetUserByCourse(int courseId)
        {
            try
            {
                using (var conn = CreateConnection(ConnectionString))
                {
                    return conn.Query<User>(Users.Users_GetUserByCourse,
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
        /// get user by list course
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public List<UserDto> GetByListCourse(List<int> courseId, List<int> roles)
        {
            try
            {
                string query = string.Format(Users.Users_GetByListCourse, string.Join(",", roles), string.Join(",", courseId));
                using (var conn = CreateConnection(ConnectionString))
                {
                    return conn.Query<UserDto>(query, commandType: CommandType.Text).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get user enroll by id , course id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="courseId"></param>
        /// <returns></returns>
        public User GetUserEnrollByUserIdWithCourseId(int userId, int courseId)
        {
            try
            {
                using (var conn = CreateConnection(ConnectionString))
                {
                    return conn.Query<User>(Users.Users_GetUserEnrollByUserIdWithCourseId, new { courseId, userId }, commandType: CommandType.Text).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get list user by section id and course id
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="sectionId"></param>
        /// <param name="courseOfferingCode"></param>
        /// <returns></returns>
        public List<UserDto> GetUserBySection(int courseId, int sectionId, string courseOfferingCode)
        {
            try
            {
                using (var conn = CreateConnection(ConnectionString))
                {
                    return conn.Query<UserDto>(Users.Users_GetUserBySection,
                        new
                        {
                            courseId,
                            sectionId,
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
    }
}