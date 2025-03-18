using System;

namespace eLearnApps.Entity.LmsTools
{
    public class GradeSubmissions : BaseEntity
    {
        public int GradeSubmissionId { get; set; }
        public int CourseId { get; set; }
        public string CourseOfferingCode { get; set; } = string.Empty;
        public DateTime SubmissionTime { get; set; }
        public int SubmittedBy { get; set; }
        public bool IsLatest { get; set; }
        public bool? AcknowledgeGradeRelease { get; set; }
    }
}