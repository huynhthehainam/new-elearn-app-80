namespace eLearnApps.Entity.LmsIsis.Dto
{
    public class DashboardFilterDto
    {
        public int CourseId { get; set; }
        public string CourseOfferingCode { get;set; }
        public string ACAD_GROUP { get; set; }
        public string STRM { get; set; }
        public int? CLASS_NBR { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string ACADEMIC_TERM { get; set; }
        public string CLASS_SECTION { get; set; }
    }
}
