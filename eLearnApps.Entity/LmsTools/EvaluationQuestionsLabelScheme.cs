using System;

namespace eLearnApps.Entity.LmsTools
{
    public class EvaluationQuestionsLabelScheme : BaseEntity
    {
        public int EvaluationQuestionsLabelSchemesId { get; set; }
        public long EvaluationQuestionId { get; set; }
        public int EvaluationLabelSchemeId { get; set; }
        public int LastUpdatedBy { get; set; }
        public DateTime LastUpdatedTime { get; set; }
        public bool IsDeleted { get; set; }
    }
}