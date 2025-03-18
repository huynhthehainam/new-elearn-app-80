using System;

namespace eLearnApps.Entity.LmsTools
{
    public class LearningPoint : BaseEntity
    {
        public int Id { get; set; }
        public int ICSSessionId { get; set; }
        public string? Description { get; set; }
        public int RecordStatus { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? LastUpdatedOn { get; set; }
    }
}