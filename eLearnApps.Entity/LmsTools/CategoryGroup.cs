namespace eLearnApps.Entity.LmsTools
{
    public class CategoryGroup : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string TextDescription { get; set; } = string.Empty;
        public string HtmlDescription { get; set; } = string.Empty;
        public int? CourseCategoryId { get; set; }
    }
}