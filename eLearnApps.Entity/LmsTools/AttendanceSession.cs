using System;

namespace eLearnApps.Entity.LmsTools
{
    public class AttendanceSession : BaseEntity
    {
        public int AttendanceSessionId { get; set; }
        public int AttendanceListId { get; set; }
        public DateTime SessionStartTime { get; set; }
        public DateTime? EntryCloseTime { get; set; }
        public int LastUpdatedBy { get; set; }
        public DateTime LastUpdatedTime { get; set; }
        public bool IsDeleted { get; set; }
    }
}