namespace eLearnApps.Entity.LmsIsis
{
    public class PS_SIS_LMS_GRADE_TYPE : BaseEntity
    {
        public string? CourseOfferingCode { get; set; }
        public string ACAD_CAREER { get; set; } = string.Empty;
        public string STRM { get; set; } = string.Empty;
        public int CLASS_NBR { get; set; }
        public string? SIS_GRADE_TYPE { get; set; }
    }
}