using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.ViewModel.RPT
{
    public class ReportViewModel
    {
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string Instructors { get; set; }
        public string UserName { get; set; }
        public List<string> Submissions { get; set; }
    }
}
