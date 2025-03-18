using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.Entity.LmsTools
{
    public class GradeObject : BaseEntity
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string? Name { get; set; }
        public string? ShortName { get; set; }
        public string? GradeType { get; set; }
        public int CategoryId { get; set; }
        public string? DescriptionHtml { get; set; }
        public string? DescriptionText { get; set; }
        public decimal Weight { get; set; }
        public string? ActivityId { get; set; }
        public decimal MaxPoints { get; set; }
        public bool CanExceedMaxPoints { get; set; }
        public bool IsBonus { get; set; }
        public bool ExcludeFromFinalGradeCalculation { get; set; }
        public int? GradeSchemeId { get; set; }
        public string? GradeSchemeUrl { get; set; }
        public int? DisplayOrder { get; set; }
    }
}
