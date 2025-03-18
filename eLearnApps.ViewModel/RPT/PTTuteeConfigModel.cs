using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.ViewModel.RPT
{
    public class PTTuteeConfigModel
    {
        public string STRM { get; set; }
        public string COURSE_ID { get; set; }
        public DateTime TuteeRegEndDate { get; set; }
        public List<string> TuteeExcludedRole { get; set; }
    }
}
