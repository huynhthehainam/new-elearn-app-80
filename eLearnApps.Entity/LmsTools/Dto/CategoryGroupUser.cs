namespace eLearnApps.Entity.LmsTools.Dto
{
    public class CategoryGroupUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CourseCategoryId { get; set; }
        public int UserId { get; set; }
        public string SectionName { get; set; }
    }
}