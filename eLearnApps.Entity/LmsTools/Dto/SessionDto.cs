using System;

namespace eLearnApps.Entity.LmsTools.Dto
{
    public class SessionDto
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
    }
    public class PeerFeedBackSessionDto
    {
        public int Id { get; set; }
        public DateTime EntryStartTime { get; set; }
        public DateTime EntryCloseTime { get; set; }
        public int LastUpdatedBy { get; set; }
        public DateTime LastUpdatedTime { get; set; }
        public string Strm { get; set; }
        public string Label { get; set; }
        public int PeerFeedbackId { get; set; }
        public string CourseOfferingCode { get; set; }
        public string PeerFeedBackName { get; set; }
    }
}