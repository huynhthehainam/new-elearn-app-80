using System;

namespace eLearnApps.Entity.LmsTools
{
    public class GradeModeration
    {
        public int ModerationId { get; set; }
        public int CourseId { get; set; }
        public int GradeObjectId { get; set; }
        public int ModerationTypeId { get; set; }
        public double? MarkRange1 { get; set; }
        public double? MarkRange2 { get; set; }
        public double? AdjustMarks { get; set; }
        public int LastUpdatedBy { get; set; }
        public DateTime LastUpdatedTime { get; set; }
        public bool IsDeleted { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedTime { get; set; }
    }
}