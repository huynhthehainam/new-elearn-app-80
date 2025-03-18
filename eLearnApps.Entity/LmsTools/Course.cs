using System;

namespace eLearnApps.Entity.LmsTools
{
    public class Course : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public bool? IsActive { get; set; }
        public string? Path { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? CourseTemplateId { get; set; }
        public int? SemesterId { get; set; }
        public int? DepartmentId { get; set; }
        public string? Bookmark { get; set; }
    }
}