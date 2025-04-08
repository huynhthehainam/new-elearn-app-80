using System;

namespace eLearnApps.Entity.LmsTools.Dto
{
    public class LearningPointDto
    {
        public int Id { get; set; }
        public int ICSSessionId { get; set; }
        public string Description { get; set; }
        public int RecordStatus { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? LastUpdatedOn { get; set; }
        public int Progress { get; set; }
    }
}