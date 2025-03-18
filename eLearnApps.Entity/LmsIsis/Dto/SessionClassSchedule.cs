using System;

namespace eLearnApps.Entity.LmsIsis.Dto
{
    public class SessionClassSchedule
    {
        public DateTime SessionDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
