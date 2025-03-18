using System;

namespace eLearnApps.Entity.LmsTools
{
    public class MeetingScheduleDetail
    {
        public int MeetingAttendeeId { get; set; }
        public int MeetingId { get; set; }
        public int AttendeeId { get; set; }
        public int InviteStatus { get; set; }
        public string SecretKey { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public DateTime? LastUpdatedOn { get; set; }
        public string? AttendeeName { get; set; }
        public string? Color { get; set; }
        public int? Status { get; set; }
    }
}