using System;
namespace eLearnApps.ViewModel.Valence
{
    public class CourseOffering
    {
        public string Identifier { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsActive { get; set; }
        public string Path { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public BasicOrgUnit CourseTemplate { get; set; }
        public BasicOrgUnit Semester { get; set; }
        public BasicOrgUnit Department { get; set; }
    }
}
