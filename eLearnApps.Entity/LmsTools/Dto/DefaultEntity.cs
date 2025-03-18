using System.Collections.Generic;

namespace eLearnApps.Entity.LmsTools.Dto
{
    public class DefaultEntity
    {
        public string Text { get; set; }
        public int Value { get; set; }
    }
    public class PeerFeedBackDashboardSelectOptionDto
    {
        public string Text { get; set; }
        public int Value { get; set; }
        public List<string> Courses { get; set; }
    }
    public class TextValue
    {
        public string Text { get; set; }
        public string Value { get; set; }
        public List<TextValue> Items { get; set; }
    }
    public class TextValueGroup
    {
        public TextValue Parent { get; set; }
        public List<TextValue> Childs { get; set; } 
    }
    public class CourseOfferingDto
    {
        public string STRM { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string ACADEMIC_TERM { get; set; }
        public string CourseOfferingCode { get; set; }
    }
}