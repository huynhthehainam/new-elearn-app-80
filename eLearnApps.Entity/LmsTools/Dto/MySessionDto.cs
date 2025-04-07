using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.Entity.LmsTools.Dto
{
   public class MySessionDto
    {
        public int AttendanceId { get; set; }
        public int AttendanceDataId { get; set; }
        public int SessionId { get; set; }
        public string SessionName { get; set; }
        public string AttendanceName { get; set; }
        public DateTime SessionStartTime { get; set; }
        public DateTime? EntryCloseTime { get; set; }
        public string Remarks { get; set; }
        public decimal? Percentage { get; set; }
        public bool AllowStudentEntry { get; set; }
        public bool? Excused { get; set; }
    }
}
