using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using eLearnApps.Core;
using eLearnApps.Data.Interface.LmsTool;
using eLearnApps.Entity.LmsTools.Dto;
using Microsoft.Extensions.Configuration;

namespace eLearnApps.Data.LmsTool
{
    public class CategoryGroupDao : BaseDao, ICategoryGroupDao
    {
        public CategoryGroupDao(IConfiguration configuration) : base(configuration)
        {
            ConnectionType = ConnectionStringType.LmsTool;
        }

        /// <summary>
        ///     Get category group by list merge course
        /// </summary>
        /// <param name="listCourse"></param>
        /// <returns></returns>
        public List<CategorySectionGroup> GetListByMergeCourse(List<int> listCourse)
        {
            try
            {
                if (listCourse == null || listCourse.Count == 0)
                    throw new ArgumentNullException("param is not valid.");
                using (var conn = CreateConnection(ConnectionString))
                {
                    var query = string.Format(Resources.LmsTool.CategoryGroup_GetListByMergeCourse,
                        string.Join(",", listCourse));
                    return conn.Query<CategorySectionGroup>(query, commandType: CommandType.Text).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<CategorySectionGroup>> GetListGroupByCourseCategoryId(int courseId)
        {
            try
            {
                using (var conn = CreateConnection(ConnectionString))
                {
                    var result = await conn.QueryAsync<CategorySectionGroup>(Resources.LmsTool.CategoryGroup_GetListGroupByCourseCategoryId, new { courseId }, commandType: CommandType.Text);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// get user category group by condition
        /// </summary>
        /// <param name="courseOfferingCodes"></param>
        /// <returns></returns>
        public async Task<List<UserCategoryGroupDto>> GetUserCategoryGroupAsync(List<string> courseOfferingCodes)
        {
            try
            {
                using (var conn = CreateConnection(ConnectionString))
                {
                    var result = await conn.QueryAsync<UserCategoryGroupDto>(Resources.LmsTool.UserGroup_GetPeerFeedBackTargetUser,
                        new
                        {
                            courseOfferingCodes = courseOfferingCodes.ToArray()
                        }, commandType: CommandType.Text);
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