using System;

namespace eLearnApps.Entity.LmsTools
{
    public class SubmissionAcknowledgement : BaseEntity
    {
        public int SubmissionAcknowledgementId { get; set; }
        public int UserId { get; set; }
        public int OrgUnitId { get; set; }
        public int CourseId { get; set; }
        public int? GradeSubmissionId { get; set; }
        public string? Acknowledgement { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}