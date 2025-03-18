using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.Entity.Valence
{
    public class GradeSetupInfo
    {
        public string GradingSystem { get; set; }
        public bool IsNullGradeZero { get; set; }
        public int DefaultGradeSchemeId { get; set; }
    }
}
