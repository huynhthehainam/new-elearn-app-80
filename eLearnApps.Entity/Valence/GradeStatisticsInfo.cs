using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.Entity.Valence
{
    public class GradeStatisticsInfo
    {
        public int OrgUnitId { get; set; }
        public int GradeObjectId { get; set; }
        public double Minimum { get; set; }
        public double Maximum { get; set; }
        public double Average { get; set; }
        public List<double> Mode { get; set; }
        public double Median { get; set; }
        public double StandardDeviation { get; set; }

    }
}
