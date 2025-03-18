using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.ViewModel.RPT
{

    public class GradeReleaseModel
    {
        public int GradeReleaseId { set; get; }
        public int OrgUnitId { set; get; }
        public string SectionName { set; get; }
        public string ReleasedGrades { set; get; }
        public string LastUpdatedBy { set; get; }
        public string LastUpdatedTime { set; get; }
    }

    public class GradeReleaseAlsoApplyToViewModel
    {
        public eLearnApps.ViewModel.Common.CourseViewModel SelectedCourse { get; set; }
        public List<eLearnApps.ViewModel.Common.CourseViewModel> SelectableSections { get; set; }
    }
}
