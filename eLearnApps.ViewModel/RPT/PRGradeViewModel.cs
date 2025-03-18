using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.ViewModel.RPT
{
    public class PRGradeViewModel
    {
        public string SectionKey { get; set; }
        public string SectionName { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedDate { get; set; }
        public List<Common.UserViewModel> PRGradeStudents { get; set; }
    }

}
