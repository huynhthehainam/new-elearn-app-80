using System;

namespace eLearnApps.Entity.LmsTools
{
    public class EvaluationQuestion : BaseEntity
    {
        public long EvaluationQuestionId { get; set; }
        public int EvaluationId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Question { get; set; } = string.Empty;
        public decimal Weight { get; set; }
        public int EntryTypeId { get; set; }
        public decimal MinMarks { get; set; }
        public decimal MaxMarks { get; set; }
        public decimal MarksInterval { get; set; }
        public int MinSelection { get; set; }
        public int MaxSelection { get; set; }
        public long DisplayOrder { get; set; }
        public bool AllowDecimalMarks { get; set; }
        public bool AllowComments { get; set; }
        public int LastUpdatedBy { get; set; }
        public DateTime LastUpdatedTime { get; set; }
        public bool IsDeleted { get; set; }
        public int? RankMarkLabelId { get; set; }
    }
}