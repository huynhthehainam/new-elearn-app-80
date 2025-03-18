using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.Entity.LmsIsis
{
    public class TL_CourseOfferings : BaseEntity
    {
        public string CourseOfferingCode { get; set; } = string.Empty;
        public string? ACAD_CAREER { get; set; }
        public string? STRM { get; set; }
        public Decimal? CLASS_NBR { get; set; }
        public string ACADEMIC_TERM { get; set; } = string.Empty;
        public string? ACADEMIC_YEAR { get; set; }
        public string COURSE_ID { get; set; } = string.Empty;
        public string? SECTION_ID { get; set; }
        public bool MERGE_SECTION { get; set; }
        public DateTime? START_DATE { get; set; }
        public DateTime? END_DATE { get; set; }
        public bool? INVALID { get; set; }
        public bool? WORKSHOP { get; set; }
    }
}
