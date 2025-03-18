using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.ViewModel.RPT
{
    public class GradeReleaseDescriptionViewModel
    {
        public string SectionName { set; get; }
        public List<ComponentGradeReleaseDescriptionViewModel> ComponentGradeReleaseDescription { get; set; }
        public bool IsSubmitted { get; set; }
    }


    public class ComponentGradeReleaseDescriptionViewModel
    {
        public string Color { get; set; }
        public string GradeObjectName { set; get; }
        public DateTime? ReleaseTime { set; get; }
        public List<string> ReleasedInfo { get; set; }
    }
}
