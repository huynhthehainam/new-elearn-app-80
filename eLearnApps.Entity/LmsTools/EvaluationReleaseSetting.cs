using System;

namespace eLearnApps.Entity.LmsTools
{
    public class EvaluationReleaseSetting : BaseEntity
    {
        public long EvaluationReleaseSettingId { get; set; }
        public int EvaluationId { get; set; }
        public long EvaluationSessionId { get; set; }
        public long EvaluationQuestionId { get; set; }
        public bool IsReleased { get; set; }
        public DateTime StartDate { get; set; }
        public bool IsMarksDisplayed { get; set; }
        public bool IsCommentsDisplayed { get; set; }
        public bool IsDeleted { get; set; }
        public int LastUpdatedBy { get; set; }
        public DateTime LastUpdatedTime { get; set; }
    }
}