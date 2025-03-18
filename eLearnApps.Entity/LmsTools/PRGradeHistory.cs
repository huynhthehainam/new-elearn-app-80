using System;
using System.Text;
using System.Collections.Generic;


namespace eLearnApps.Entity.LmsTools
{
    public class PRGradeHistory : BaseEntity
    {
        public PRGradeHistory() { }
        public PRGradeHistory(int prGradeHistoryId, int prGradeId, string courseOfferingCode, int courseId, int sectionId, int userId, int lastUpdatedBy, DateTime lastUpdatedTime)
        {
            PRGradeHistoryId = prGradeHistoryId;
            PRGradeId = prGradeId;
            CourseOfferingCode = courseOfferingCode;
            CourseId = courseId;
            SectionId = sectionId;
            UserId = userId;
            LastUpdatedBy = lastUpdatedBy;
            LastUpdatedTime = lastUpdatedTime;
        }

        public long PRGradeHistoryId { get; set; }
        public int PRGradeId { get; set; }
        public string CourseOfferingCode { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public int? SectionId { get; set; }
        public int UserId { get; set; }
        public int LastUpdatedBy { get; set; }
        public DateTime LastUpdatedTime { get; set; }
    }
}
