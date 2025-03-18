using System;

namespace eLearnApps.Entity.LmsTools
{
    public class AttendanceList : BaseEntity
    {
        public int AttendanceListId { get; set; }
        public int CourseId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int WarningLevel { get; set; }
        public bool? IsVisible { get; set; }
        public bool? IncludeAllUsers { get; set; }
        public bool? AllowStudentEntry { get; set; }
        public int? EntryCloseAfter { get; set; }
        public bool IsDeleted { get; set; }
        public int LastUpdatedBy { get; set; }
        public DateTime LastUpdatedTime { get; set; }
    }
}