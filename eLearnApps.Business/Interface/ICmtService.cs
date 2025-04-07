using System.Collections.Generic;
using eLearnApps.Entity.LmsTools;
using eLearnApps.Entity.LmsTools.Dto;

namespace eLearnApps.Business.Interface
{
    public interface ICmtService
    {
        List<UserDto> GetUsersByCourseId(int courseId);
        ClassPhotoSetting GetClassPhotoSettingByCourseId(int courseId, int userId);
        List<GroupData> GetGroupDataByCourseCategoryId(int courseCategoryId);
        List<Entity.LmsTools.Dto.CourseCategory> GetCategoriesByCourseId(int courseId);
        List<MySessionDto> GetAttendanceSessionByAttendanceId(int attendanceId, int studentId);
        List<MyAttendanceDto> GetAllAttendanceByStudentId(int courseId, int studentId);
        List<MySessionDto> GetCurrentSessionByStudentId(int courseId, int studentId);
        MyAttendanceDto GetAttendanceDetailById(int attendanceId, int studentId);
        List<MyAttendanceDto> GetAttendanceDetailByListId(List<int> ids, int studentId);
        /// <summary>
        /// Get all student attendance data by attendance ids
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        List<MyAttendanceDto> GetAllStudentAttendanceData(List<int> ids);
        MarkAttendanceSetting GetMarkAttendanceSetting(int termId, int courseId, int userId);
        MarkAttendanceSetting GetMarkAttendanceSettingByCourseId(int courseId, int userId);
    }

}