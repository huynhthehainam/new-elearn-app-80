using System;
using System.ComponentModel.DataAnnotations;

namespace eLearnApps.ViewModel.FFTS
{
    public class Attendee
    {
        public int MeetingAttendeeId { get; set; }
        public int MeetingId { get; set; }
        public int InviteStatus { get; set; }
        public int AttendeeId { get; set; }
        public string AttendeeName { get; set; }
        public string AttendeeEmail { get; set; }
        [StringLength(100)]
        public string SecretKey { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? LastUpdatedOn { get; set; }
    }
}