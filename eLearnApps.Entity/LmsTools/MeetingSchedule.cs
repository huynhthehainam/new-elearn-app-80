using System;
using System.Collections.Generic;

namespace eLearnApps.Entity.LmsTools
{
    public class MeetingSchedule
    {
        public int MeetingId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? Course { get; set; }
        public string? Location { get; set; }
        public int OwnerId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? LastUpdatedOn { get; set; }
        public string? OwnerName { get; set; }
        public string? OwnerEmail { get; set; }
        public int? AttendeeId { get; set; }
        public string? AttendeeName { get; set; }
        public List<MeetingScheduleDetail> MeetingScheduleDetail { get; set; } = [];
        public bool IsCanEdit { get; set; }
        public int? Status { get; set; }
        public int InviteStatus { get; set; }
    }
}