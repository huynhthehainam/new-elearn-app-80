using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.ViewModel.WidgetLTI
{
    public class ClassScheduleLtiViewModel
    {
        public string Term { get; set; }
        public bool ShowingOnlyOneCourse { get; set; }
        public List<ClassScheduleLtiCourseViewModel> Courses { get; set; }
    }

    public class ClassScheduleLtiCourseViewModel
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public List<ClassScheduleLtiCourseScheduleViewModel> Schedules { get; set; }
    }

    public class ClassScheduleLtiCourseScheduleViewModel
    {
        public string Day { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public string Location { get; set; }
    }

    public class ClassScheduleDto
    {
        /// <summary>
        /// Course Start Date
        /// </summary>
        public DateTime START_DATE { get; set; }
        /// <summary>
        /// Course End Date
        /// </summary>
        public DateTime END_DATE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string ACADEMIC_TERM { get; set; }
        public string CourseOfferingCode { get; set; }
        public string CourseName { get; set; }
        public string MON { get; set; }
        public string TUES { get; set; }
        public string WED { get; set; }
        public string THURS { get; set; }
        public string FRI { get; set; }
        public string SAT { get; set; }
        public string SUN { get; set; }
        public DateTime? MEETING_TIME_START { get; set; }
        public DateTime? MEETING_TIME_END { get; set; }
        public string DESCR { get; set; }
        public string EMAIL_ADDR { get; set; }
    }
}
