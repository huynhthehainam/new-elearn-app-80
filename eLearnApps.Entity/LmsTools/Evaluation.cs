using System;

namespace eLearnApps.Entity.LmsTools
{
    public class Evaluation : BaseEntity
    {
        public int EvaluationId { get; set; }
        public int CourseId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int EvaluationTypeId { get; set; }
        public bool AllowSelfEvaluate { get; set; }
        public bool IsInstructorEvaluator { get; set; }
        public int InstructorWeight { get; set; }
        public bool AllowEvaluation { get; set; }
        public decimal QuestionScoreCalculation { get; set; }
        public int SessionScoreCalculation { get; set; }
        public int LastUpdatedBy { get; set; }
        public DateTime LastUpdatedTime { get; set; }
        public DateTime CreatedTime { get; set; }
        public bool IsDeleted { get; set; }
        public int? GradeObjectId { get; set; }
        public string? GradeObjectName { get; set; }
        public bool? AllowDisplayGroupByQuestion { get; set; }
    }
}