namespace eLearnApps.Entity.LmsTools
{
    public class CourseTemplate : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }
}