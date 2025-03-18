using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.ViewModel.RPT
{
   public class CourseModel
    {
        public int Id { set; get; }
        public string Section { set; get; }
        public string Instructors { set; get; }
        public int NoOfStudents { set; get; }
        public int NoOfFinalGrade { get; set; }
        public bool HasSection { set; get; }
        public string ReleaseStatus { set; get; }
        public string SubmissionStatus { set; get; }
        public string Code { set; get; }

        /// <summary>
        /// contain 'Code' for individual course, contains individual courses code for merge course
        /// </summary>
        public List<string> CourseCodes { get; set; }
    }
}
