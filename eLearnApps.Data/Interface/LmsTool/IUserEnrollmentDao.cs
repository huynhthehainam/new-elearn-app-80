using System.Collections.Generic;
using System.Threading.Tasks;
using eLearnApps.Entity.LmsTools;
using eLearnApps.Entity.LmsTools.Dto;

namespace eLearnApps.Data.Interface.LmsTool
{
    public interface IUserEnrollmentDao
    {
        /// <summary>
        ///     Get list user id enroll
        /// </summary>
        /// <param name="courseId"></param>
        /// <returns></returns>
        List<int> GetUserIdEnrollByCourse(int courseId);
        /// <summary>
        ///  Get list user id enroll
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        List<int> GetAllStudentByCourse(int courseId, int roleId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<UserDto> GetUserEnrollByCourseIdAndUserIdAsync(int courseId, int userId);
    }
}