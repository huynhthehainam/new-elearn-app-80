using System;

namespace eLearnApps.Entity.LmsTools
{
    public class EvaluationSession : BaseEntity
    {
        public int EvaluationSessionId { get; set; }
        public int EvaluationId { get; set; }
        public DateTime EntryStartTime { get; set; }
        public DateTime EntryCloseTime { get; set; }
        public decimal Weight { get; set; }
        public int LastUpdatedBy { get; set; }
        public DateTime LastUpdatedTime { get; set; }
        public bool IsDeleted { get; set; }
        public string Label { get; set; } = string.Empty;
    }
}