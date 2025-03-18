using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.Entity.LmsTools
{
    public class GradeReleaseSetting : BaseEntity
    {
        public int GradeReleaseSettingId { get; set; }
        public int GradeReleaseId { get; set; }
        public int GradeObjectId { get; set; }
        public int OrgUnitId { get; set; }
        public int SortOrder { get; set; }
        public bool IsReleased { get; set; }
        public DateTime StartDate { get; set; }
        public bool IsPointsDisplayed { get; set; }
        public bool IsSymbolDisplayed { get; set; }
        public bool IsWeightDisplayed { get; set; }
        public bool IsAverageDisplayed { get; set; }
        public bool IsMinMaxDisplayed { get; set; }
    }
}