using System;

namespace eLearnApps.Entity.LmsTools
{
    public class Meeting : BaseEntity
    {
        public int MeetingId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int OwnerId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? LastUpdatedOn { get; set; }
        public string? Course { get; set; }
        public string? Location { get; set; }
        public int? Status { get; set; }
    }
}