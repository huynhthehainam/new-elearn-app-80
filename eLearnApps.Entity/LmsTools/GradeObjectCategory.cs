using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.Entity.LmsTools
{
    public class GradeObjectCategory : BaseEntity
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? ShortName { get; set; }
        public bool CanExceedMax { get; set; }
        public bool ExcludeFromFinalGrade { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? Weight { get; set; }
        public decimal? MaxPoints { get; set; }
        public bool AutoPoints { get; set; }
        public int? WeightDistributionType { get; set; }
        public int NumberOfHighestToDrop { get; set; }
        public int NumberOfLowestToDrop { get; set; }
        public int? DisplayOrder { get; set; }
    }
}
