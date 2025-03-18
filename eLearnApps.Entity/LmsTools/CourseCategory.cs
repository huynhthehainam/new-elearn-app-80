using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.Entity.LmsTools
{
    public class CourseCategory : BaseEntity
    {
        public int Id { get; set; }
        public string EnrollmentStyle { get; set; } = string.Empty;
        public int? EnrollmentQuantity { get; set; }
        public bool AutoEnroll { get; set; }
        public bool RandomizeEnrollments { get; set; }
        public string Name { get; set; } = string.Empty;
        public string TextDescription { get; set; } = string.Empty;
        public string HtmlDescription { get; set; } = string.Empty;
        public int? MaxUsersPerGroup { get; set; }
        public int? CourseId { get; set; }
    }
}
