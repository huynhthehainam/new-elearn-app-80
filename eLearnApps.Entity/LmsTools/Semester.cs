namespace eLearnApps.Entity.LmsTools
{
    public class Semester : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }
}