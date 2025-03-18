using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.Entity.LmsTools
{
    public class SubmittedGradeStatistic
    {
        public int OrgUnitId { get; set; }
        public int GradeObjectId { get; set; }
        public double AverageMark { get; set; }
        public string? AverageGrade { get; set; }
        public double MinMark { get; set; }
        public string? MinGrade { get; set; }
        public double MaxMark { get; set; }
        public string? MaxGrade { get; set; }
        public int LastUpdatedBy { get; set; }
        public DateTime LastUpdatedTime { get; set; }
    }
}
