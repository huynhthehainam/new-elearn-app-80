using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.Entity.LmsTools
{
    public class GradeResetStatus : BaseEntity
    {
        public int GradeSubmissionId { get; set; }
        public int CourseId { get; set; }
        public string CourseOfferingCode { get; set; } = string.Empty;
        public string ACAD_CAREER { get; set; } = string.Empty;
        public string STRM { get; set; } = string.Empty;
        public Decimal CLASS_NBR { get; set; }
        public string RESET_STATUS { get; set; } = string.Empty;
        public DateTime SUBMISSION_DATE { get; set; }
    }
}