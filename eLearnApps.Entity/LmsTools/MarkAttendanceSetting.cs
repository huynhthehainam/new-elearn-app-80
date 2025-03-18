using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.Entity.LmsTools
{
    public class MarkAttendanceSetting : BaseEntity
    {
        public int MarkAttendanceSettingId { get; set; }
        public int? CourseId { get; set; }
        public int? TermId { get; set; }
        public bool ListView { get; set; }
        public int CreatedBy { get; set; }
        public DateTime LastUpdatedOn { get; set; }
    }
}
