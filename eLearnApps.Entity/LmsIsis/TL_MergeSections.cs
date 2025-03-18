namespace eLearnApps.Entity.LmsIsis
{
    public class TL_MergeSections : BaseEntity
    {
        public string? CourseOfferingCode { get; set; }
        public string? SectionCode { get; set; }
        public string? SectionName { get; set; }
        public string? IndvCourseOfferingCode { get; set; }
    }
}