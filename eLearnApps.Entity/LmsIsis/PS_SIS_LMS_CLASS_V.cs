using System;

namespace eLearnApps.Entity.LmsIsis
{
    public class PS_SIS_LMS_CLASS_V : BaseEntity
    {
        public string ACAD_CAREER { get; set; } = string.Empty;
        public string STRM { get; set; } = string.Empty;
        public int CLASS_NBR { get; set; }
        public string SIS_TERM_DESCR { get; set; } = string.Empty;
        public string? DESCR { get; set; }
        public string? COURSE_TITLE_LONG { get; set; }
        public string SMU_CRSE_CD { get; set; } = string.Empty;
        public string CLASS_SECTION { get; set; } = string.Empty;
        //public double UNITS_MAXIMUM { get; set; }
        public DateTime? START_DT { get; set; }
        public DateTime? END_DT { get; set; }
        public string ACAD_GROUP { get; set; } = string.Empty;
        //public string DEPTID { get; set; }
    }
}
