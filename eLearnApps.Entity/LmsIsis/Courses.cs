namespace eLearnApps.Entity.LmsIsis
{
    public class Courses : BaseEntity
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
    }
}
