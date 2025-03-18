using System;
using System.Text;
using System.Collections.Generic;


namespace eLearnApps.Entity.LmsTools
{
    public class IGradeHistories : BaseEntity
    {
        public long IGradeHistoryId { get; set; }
        public int IGradeId { get; set; }
        public string CourseOfferingCode { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public int? SectionId { get; set; }
        public int UserId { get; set; }
        public int LastUpdatedBy { get; set; }
        public DateTime LastUpdatedTime { get; set; }
    }
}
