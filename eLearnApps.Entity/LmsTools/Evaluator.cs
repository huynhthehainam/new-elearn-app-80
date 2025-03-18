using System;

namespace eLearnApps.Entity.LmsTools
{
    public class Evaluator : BaseEntity
    {
        public long EvaluatorId { get; set; }
        public int EvaluationPairingId { get; set; }
        public int UserId { get; set; }
        public int OrgUnitId { get; set; }
        public bool IsOrgUnit { get; set; }
        public bool IsDeleted { get; set; }
        public int LastUpdatedBy { get; set; }
        public DateTime LastUpdatedTime { get; set; }
    }
}