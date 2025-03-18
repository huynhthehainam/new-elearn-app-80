using System;

namespace eLearnApps.Entity.LmsTools
{
    public class EvaluationPairing : BaseEntity
    {
        public int EvaluationPairingId { get; set; }
        public int EvaluationId { get; set; }
        public bool IsDeleted { get; set; }
        public int LastUpdatedBy { get; set; }
        public DateTime LastUpdatedTime { get; set; }
    }
}