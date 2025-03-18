using System;

namespace eLearnApps.Entity.LmsTools
{
    public class MeetingAttendee : BaseEntity
    {
        public int MeetingAttendeeId { get; set; }
        public int MeetingId { get; set; }
        public int AttendeeId { get; set; }
        public int InviteStatus { get; set; }
        public string SecretKey { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public DateTime? LastUpdatedOn { get; set; }
        public string? Email { get; set; }
        public int? Status { get; set; }
    }
}