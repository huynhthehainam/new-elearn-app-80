using System;
using System.ComponentModel.DataAnnotations;

namespace eLearnApps.ViewModel.ICS
{
    public class SessionTiming
    {
        public DayOfWeek Day { get; set; }

        [Display(Name = "Start")] public TimeSpan? StartTime { get; set; }

        [Display(Name = "End")] public TimeSpan? EndTime { get; set; }
    }
}