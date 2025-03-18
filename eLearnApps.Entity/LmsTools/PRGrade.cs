using System;
using System.Text;
using System.Collections.Generic;


namespace eLearnApps.Entity.LmsTools {
    
    public class PRGrade : BaseEntity
    {
        public PRGrade(int prGradeId, string courseOfferingCode, int courseId, int sectionId, int userId, int lastUpdatedBy, DateTime lastUpdatedTime)
        {
            PRGradeId = prGradeId;
            CourseOfferingCode = courseOfferingCode;
            CourseId = courseId;
            SectionId = sectionId;
            UserId = userId;
            LastUpdatedBy = lastUpdatedBy;
            LastUpdatedTime = lastUpdatedTime;
        }

        public int PRGradeId { get; set; }
        public string CourseOfferingCode { get; set; }
        public int CourseId { get; set; }
        public int SectionId { get; set; }
        public int UserId { get; set; }
        public int LastUpdatedBy { get; set; }
        public DateTime LastUpdatedTime { get; set; }
    }
}
