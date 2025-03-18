using System;

namespace eLearnApps.Entity.LmsTools
{
    public class AttendanceData : BaseEntity
    {
        public int AttendanceDataId { get; set; }
        public int AttendanceSessionId { get; set; }
        public int UserId { get; set; }
        public decimal? Percentage { get; set; }
        public string Remarks { get; set; } = string.Empty;
        public int LastUpdatedBy { get; set; }
        public DateTime LastUpdatedTime { get; set; }
        public bool IsDeleted { get; set; }
        public int? Participation { get; set; }

        private bool? excused;
        public bool? Excused
        {
            get
            {
                return excused;
            }
            set
            {
                if (value == false) excused = null;
                else excused = value;
            }
        }
    }
}