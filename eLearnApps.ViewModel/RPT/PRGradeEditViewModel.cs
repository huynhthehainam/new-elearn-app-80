using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.ViewModel.RPT
{
    public class PRGradeEditViewModel
    {
        public string SectionKey { get; set; }
        public string CourseOfferingCode { get; set; }
        public List<Common.RPTUserViewModel> Students { get; set; }
    }
}
