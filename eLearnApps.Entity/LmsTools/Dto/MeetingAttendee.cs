using System;

namespace eLearnApps.Entity.LmsTools.Dto
{
    public class MeetingAttendee
    {
        public int MeetingAttendeeId { get; set; }
        public int MeetingId { get; set; }
        public int AttendeeId { get; set; }
        public string AttendeeName { get; set; }
        public string AttendeeEmail { get; set; }
        public int InviteStatus { get; set; }
        public string SecretKey { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? LastUpdatedOn { get; set; }
        public int? Status { get; set; }
    }
}