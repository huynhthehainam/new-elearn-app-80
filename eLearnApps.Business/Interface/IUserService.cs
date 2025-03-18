using eLearnApps.Entity.LmsTools;
using eLearnApps.Entity.LmsTools.Dto;
using eLearnApps.ViewModel.Security;

namespace eLearnApps.Business.Interface
{
    public interface IUserService
    {
        User GetById(int id);
        IEnumerable<User> GetByIds(IEnumerable<int> Ids);
        void Insert(User user);
        void Update(User user);
        void Delete(User user);
        void Insert(List<User> users);
        User GetByOrgDefinedId(string orgDefinedId);
        List<TeamMate> GetByCourseId(int courseId);
        List<TeamMate> GetByCourseCategoryId(int courseCategoryId);
        List<TeamMate> GetByCategoryGroupId(int categoryGroupId);
        /// <summary>
        /// Get list of users who is enrolled in Group that is specifed in GroupIds
        /// </summary>
        /// <param name="groupIds">GroupIds whose student we want to find</param>
        /// <returns>List of user</returns>
        List<User> GetByCategoryGroupIds(List<int> groupIds);
        List<User> GetListAttendeesById(List<int> ids);
        void Save(List<User> user);
        D2LLoginLog GetLastLogin(int userId, string IPAddress, string D2LTokenDigest);
        /// <summary>
        /// Get list user by section id and course id
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="sectionId"></param>
        /// <param name="courseOfferingCode"></param>
        /// <returns></returns>
        List<UserDto> GetUserBySection(int courseId, int sectionId, string courseOfferingCode);
        List<UserDto> GetUserByCourseId(int courseId);
        /// <summary>
        /// Get all user in this course
        /// </summary>
        /// <param name="courseId"></param>
        /// <returns></returns>
        List<User> GetAllUserInThisCourse(int courseId);
        void GeneratePhotoAtPath(string path, List<UserDto> users);

        List<UserDto> GetUserDtoForPhotoGen();

        // TODO move to a better place (aim to have LMS Service
        List<Role> GetAllRoles();
        List<UserEnrollmentViewModel> GetUserEnrollmentForAllCourse(int userId);
        List<RolePermissionViewModel> GetRolePermission(List<int> roleIds);
        List<UserEnrollment> GetEnrollmentFor(int userId, int courseId);

        /// <summary>
        /// Get user by list course id
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orgDefinedId"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        Task<byte[]> GetStudentPhotoByOrgDefineIdAsync(string orgDefinedId, int width, int height);

        /// <summary>
        /// TODO this function need to be made more mature
        /// </summary>
        /// <param name="UserGroupIds"></param>
        /// <returns></returns>
        List<ItemDto> GetUserByGroupId(List<int> UserGroupIds);
    }
}