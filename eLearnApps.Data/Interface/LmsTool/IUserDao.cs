using System.Collections.Generic;
using eLearnApps.Entity.LmsTools;
using eLearnApps.Entity.LmsTools.Dto;

namespace eLearnApps.Data.Interface.LmsTool
{
    public interface IUserDao
    {
        /// <summary>
        /// Get list user by section id and course id
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="sectionId"></param>
        /// <param name="courseOfferingCode"></param>
        /// <returns></returns>
        List<UserDto> GetUserBySection(int courseId, int sectionId, string courseOfferingCode);
        /// <summary>
        /// Get all user in this course
        /// </summary>
        /// <param name="courseId"></param>
        /// <returns></returns>
        List<User> GetUserByCourse(int courseId);
        /// <summary>
        /// get user by list course
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        List<UserDto> GetByListCourse(List<int> courseId, List<int> roles);
        /// <summary>
        /// Get user enroll by id , course id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="courseId"></param>
        /// <returns></returns>
        User GetUserEnrollByUserIdWithCourseId(int userId, int courseId);
    }
}