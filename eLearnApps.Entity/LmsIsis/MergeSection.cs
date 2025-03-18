namespace eLearnApps.Entity.LmsIsis
{
    public class MergeSection : BaseEntity
    {
        public string? CourseOfferingCode { get; set; }
        public string? SectionCode { get; set; }
        public string? SectionName { get; set; }
        public string? IndvCourseOfferingCode { get; set; }
    }
}