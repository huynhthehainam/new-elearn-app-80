using System;
using System.ComponentModel.DataAnnotations;

namespace eLearnApps.ViewModel.ICS
{
    public class CreateSingleSession
    {
        public string Title { get; set; }
        [Display(Name = "Session Date")] public DateTime? SessionDate { get; set; }

        public SessionTiming Timing { get; set; }
    }
}