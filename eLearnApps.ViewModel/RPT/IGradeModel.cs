using System.Collections.Generic;

namespace eLearnApps.ViewModel.RPT
{
    public class IGradeModel
    {
        public string UserKey { get; set; }
        public string DisplayName { get; set; }
        public bool IsSubmitted { get; set; }
        public string IgradeKey { get; set; }
        public bool IsPRGrade { get; set; }
    }

    public class IGradeEditModel
    {
        public string SectionKey { get; set; }
        public List<IGradeModel> ListIGradeModel { get; set; }
        public string DataSource { get; set; }
        public ICollection<string> UserKeys { get; set; }
        public int TotalCount { get; set; }
        public string SectionName { get; set; }
    }

    public class IndCourseOfferingDetail
    {
        public int OrgUnitId { get; set; }
        public string CourseOfferingCodes { get; set; }
        public string SectionName { get; set; }
    }

    public class IGradeViewModel
    {
        public string SectionKey { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string LastModifiedDate { get; set; }
        public string IGradeStudent { get; set; }
    }
}