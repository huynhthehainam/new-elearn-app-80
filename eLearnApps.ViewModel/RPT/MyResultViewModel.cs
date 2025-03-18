using eLearnApps.Entity.LmsTools.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.ViewModel.RPT
{
    public class MyResultViewModel
    {
        public List<UserDto> Instructors { get; set; }
        public UserDto Student { get; set; }
        public List<GradeResultModel> Results { get; set; }

        public MyResultViewModel()
        {
            Instructors = new List<UserDto>();
            Results = new List<GradeResultModel>();
        }
    }
}
