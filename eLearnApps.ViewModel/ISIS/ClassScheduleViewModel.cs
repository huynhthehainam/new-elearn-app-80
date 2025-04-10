using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.ViewModel.ISIS
{
    public class ClassScheduleViewModel
    {
        public ICollection<DayOfWeek> SelectedDays { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
    }
}
