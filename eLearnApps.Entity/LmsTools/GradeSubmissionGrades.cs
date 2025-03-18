using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.Entity.LmsTools
{
    public class GradeSubmissionGrades : BaseEntity
    {
        public int GradeSubmissionId { get; set; }
        public int StudentId { get; set; }
        public int GradeObjectId { get; set; }
        public string StudentOrgDefinedId { get; set; } = string.Empty;
        public double OriginalFinalMarks { get; set; }
        public int FinalMarks { get; set; }
        public string FinalGrade { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public string CourseOfferingCode { get; set; } = string.Empty;
    }
}