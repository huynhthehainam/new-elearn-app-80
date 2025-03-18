using System.Collections.Generic;
using System.Threading.Tasks;
using eLearnApps.Entity.LmsIsis;
using eLearnApps.Entity.LmsIsis.Dto;
using eLearnApps.Entity.LmsTools;
using eLearnApps.ViewModel.Common;
using eLearnApps.ViewModel.KendoUI;
using eLearnApps.ViewModel.WidgetLTI;

namespace eLearnApps.Business.Interface
{
    public interface ICourseService
    {
        Course GetById(int id);
        void Insert(Course course);
        void Update(Course course);
        void Delete(Course course);
        void Insert(List<Course> courses);
        List<Course> GetBySemesterId(int userId, int semesterId);
        Task<List<Course>> GetBySemesterIdAsync(int userId, int semesterId);
        List<SemesterViewModel> GetCourseByUserAndRole(int userId, string roleName);
        List<Course> GetByStudentId(int studentId);
        void Save(List<Course> courses);
        Course GetById(int id, int userId);
        List<Course> GetAll();
        List<int> GetAllCourseId();
        void CopyCourseEnrollmentToEnrolledUserMeeting(int courseId);
        List<eLearnApps.Entity.LmsTools.Dto.CourseSchedulesInSemester> GetUserEnrolledCourses(int userId, int courseId);
        List<int> GetCourseIdPreviouslySubmitted();
        List<int> GetCourseIdNotPreviouslySubmitted();
        string GetLastBookmark();

        TL_CourseOfferings FindISISCourseOffering(string courseOfferingCode);
        List<TL_CourseOfferings> FindISISCourseOffering(List<string> courseOfferingCodes);
        Course GetCoursebyCode(string courseOfferingCode);

        IEnumerable<ClassScheduleDto> GetClassSchedule(string userEmail, int courseId = 0);

        IEnumerable<ClassScheduleDto> GetClassSchedule(List<string> userEmail, int orgUnitId = 0);

        List<CourseViewModel> GetCourseSections(int orgUnitId);

        List<Course> GetByInstructor(int userId, int semesterId);

        List<CourseViewModel> GetCoursesWithSameCourseIDandTerm(int orgUnitId);
        Task<List<SessionClassSchedule>> GetSessionFromClassScheduleByCourse(int courseId);

        /// <summary>
        /// Get list of treeview item which is a viewmodel for term/course dropdown
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="courseId"></param>
        /// <returns></returns>
        List<TreeViewItem> GetTermAndCourse(int userId, int courseId);

        List<int> GetCourseByFeedbackSession(int? userId, List<int> sessions);
        List<int> GetCourseIdByListCode(List<string> courseOfferingCodes);
    }

}