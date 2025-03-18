using System;

namespace eLearnApps.Entity.LmsTools
{
    public class GPTApproved : BaseEntity
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string CourseCode { get; set; } = string.Empty;
        public int GradeSubmissionId { get; set; }
        public int GPTApprovalHistoryId { get; set; }
    }
}