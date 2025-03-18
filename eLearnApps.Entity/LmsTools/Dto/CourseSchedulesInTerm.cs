using eLearnApps.Entity.LmsIsis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.Entity.LmsTools.Dto
{
    public class CourseSchedulesInSemester
    {
        public Semester Semester { get; set; }
        public Course Course { get; set; }
        public PS_SIS_LMS_SCHED_V Schedule { get; set; }
    }
}
