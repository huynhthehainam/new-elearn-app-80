using System.Collections.Generic;

namespace eLearnApps.Entity.LmsTools.Dto
{
    public class SectionData
    {
        public int SectionId { get; set; }
        public int CourseId { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public string Html { get; set; }
        public List<int> Enrollment { get; set; }
    }
}