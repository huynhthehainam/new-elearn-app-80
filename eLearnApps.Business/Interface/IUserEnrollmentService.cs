using System.Collections.Generic;
using System.Threading.Tasks;
using eLearnApps.Entity.LmsTools;
using eLearnApps.Entity.LmsTools.Dto;

namespace eLearnApps.Business.Interface
{
    public interface IUserEnrollmentService
    {
        UserEnrollment GetById(int id);
        void Insert(UserEnrollment enrollment);
        void Update(UserEnrollment enrollment);
        void Delete(UserEnrollment enrollment);
        void Insert(List<UserEnrollment> enrollments);
        List<UserEnrollment> GetAll();
        void Delete(List<UserEnrollment> enrollments);
        void Save(List<UserEnrollment> enrollments);
        List<UserEnrollmentDto> GetByUserId(int userId);
        Entity.LmsTools.Dto.UserEnrollmentDto FindByUserIdAndCourseId(int userId, int courseId);
        List<int?> GetListRoleId(int userId);
        List<int?> FilterEnrollmentList(int courseId, List<int> attendanceCourseIds);
        List<Entity.LmsTools.Dto.StudentCountDto> GetUserEnrollmentCountByCourseId(int courseId);
        List<Entity.LmsTools.Dto.SectionData> GetUserEnrollmentByCourseId(int courseId);
        List<Entity.LmsTools.Dto.CourseDto> GetCourseByUserIdAndRole(int userId, bool checkSubmitted);
        /// <summary>
        ///     Get list user id enroll
        /// </summary>
        /// <param name="courseId"></param>
        /// <returns></returns>
        List<int> GetUserIdEnrollByCourse(int courseId);
        List<int> GetUserIdEnrollByCourses(List<int?> coursesId);
        List<UserEnrollment> GetByCourseId(int courseId);
        UserEnrollment GetInstructor(int courseId, int userId);
        List<Permission> GetPermissionsForRoleId(int roleId);
        /// <summary>
        /// Get all student in this course
        /// </summary>
        /// <param name="courseId"></param>
        /// <returns></returns>
        List<int> GetAllStudentByCourse(int courseId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<UserDto> GetUserEnrollByCourseIdAndUserIdAsync(int courseId, int userId);
        List<UserEnrollmentDto> GetUserEnrolledWithCourseByUserId(int userId);
    }
}