using System;

namespace eLearnApps.Entity.LmsTools
{
    public class GradeReportViewSetting : BaseEntity
    {
        public int OrgUnitId { get; set; }
        public int UserId { get; set; }
        public int GradeObjectId { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
    }
}