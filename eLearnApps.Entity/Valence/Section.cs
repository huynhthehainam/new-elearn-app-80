using System.Collections.Generic;

namespace eLearnApps.Entity.Valence
{
    public class Section
    {
        public int SectionId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public Description Description { get; set; }
        public List<int> Enrollments { get; set; }
    }

    public class Description
    {
        public string Text { get; set; }
        public string Html { get; set; }
    }
}