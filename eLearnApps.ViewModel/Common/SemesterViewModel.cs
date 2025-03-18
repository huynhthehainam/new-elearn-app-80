using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.ViewModel.Common
{
    public class SemesterViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public List<CourseViewModel> Courses { get; set; }

        public SemesterViewModel(int id, string name, string code)
        {
            Id = id;
            Name = name;
            Code = code;
            Courses = new List<CourseViewModel>();
        }
    }
}
