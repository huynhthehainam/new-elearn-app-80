using System;

namespace eLearnApps.ViewModel.PET
{
    public class EvaluationResponseUserModel
    {
        public long EvaluationResponseId { get; set; }
        public int EvaluationId { get; set; }
        public long EvaluationSessionId { get; set; }
        public long EvaluationQuestionId { get; set; }
        public int TargetUserId { get; set; }
        public int TargetOrgUnitId { get; set; }
        public decimal? Marks { get; set; }
        public string Comments { get; set; }
        public int EvaluatorUserId { get; set; }
        public bool IsEvaluatorInstructor { get; set; }
        public DateTime LastUpdatedTime { get; set; }
        public bool IsDeleted { get; set; }
        public string DisplayName { get; set; }
    }
}