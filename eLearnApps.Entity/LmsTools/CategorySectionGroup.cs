namespace eLearnApps.Entity.LmsTools.Dto
{
    public class CategorySectionGroup
    {
        public int? CategoryId { get; set; }
        public string Name { get; set; }
        public int? CourseCategoryId { get; set; }
        public string CourseCategoryName { get; set; }
        public int CourseId { get; set; }
        public int? NumOfStudent { get; set; }
    }
}