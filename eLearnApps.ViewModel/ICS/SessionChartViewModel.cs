using eLearnApps.Entity.LmsTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.ViewModel.ICS
{
    public class SessionChartViewModel
    {
        public DateTime StartDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public List<UserSenseViewModel> Senses { get; set; }
    }
}
