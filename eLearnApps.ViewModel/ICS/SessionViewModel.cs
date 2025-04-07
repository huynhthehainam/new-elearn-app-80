using System;

namespace eLearnApps.ViewModel.ICS
{
    public class SessionViewModel
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int CourseId { get; set; }
        public bool IsEditable { get; set; }
        public int Progress { get; set; }
        public DateTime StartDateTime => StartDate.Add(StartTime);
        public DateTime EndDateTime => StartDate.Add(EndTime);
        public string Statistic { get; set; }
        public int TotalUserInThisCourse { get; set; }
        public string Title { get; set; }
    }
}