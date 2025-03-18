using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.ViewModel.RPT
{

    public class GradeReleaseSettingModel
    {
        public int GradeReleaseSettingId { set; get; }
        public int GradeReleaseId { set; get; }
        public int GradeObjectId { set; get; }
        public string GradeType { get; set; }
        public int OrgUnitId { set; get; }

        public string Grade { set; get; }

        public bool IsReleased { set; get; }
        public string StartDate { set; get; }
        public bool IsPointsDisplayed { set; get; }
        public bool IsSymbolDisplayed { set; get; }
        public bool IsWeightDisplayed { set; get; }
        public bool IsAverageDisplayed { set; get; }
        public bool IsMinMaxDisplayed { set; get; }
    }
}
