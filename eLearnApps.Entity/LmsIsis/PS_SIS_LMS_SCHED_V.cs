using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.Entity.LmsIsis
{
    public class PS_SIS_LMS_SCHED_V : BaseEntity
    {
        public string ACAD_CAREER { get; set; } = string.Empty;
        public string STRM { get; set; } = string.Empty;
        public int CLASS_NBR { get; set; }
        public int? CLASS_MTG_NBR { get; set; }
        public string? MON { get; set; }
        public string? TUES { get; set; }
        public string? WED { get; set; }
        public string? THURS { get; set; }
        public string? FRI { get; set; }
        public string? SAT { get; set; }
        public string? SUN { get; set; }
        public DateTime? MEETING_TIME_START { get; set; }
        public DateTime? MEETING_TIME_END { get; set; }
        public string? DESCR { get; set; }
    }
}
