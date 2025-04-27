using System;
using System.Collections.Generic;

namespace eLearnApps.ViewModel.FFTS
{
    public class TemplateViewModel
    {
        public string AttendeeName { get; set; }
        public string OwnerName { get; set; }
        public string Description { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public string Link { get; set; }
        public string MeetingTitle { get; set; }
        public string StartTimeInSGTimezone { get; set; }
        public string EndTimeInSGTimezone { get; set; }
        public List<string> AttendeesName { get; set; }
        public int InviteUserStatus { get; set; }
        public string SecretKey { get; set; }
        public int AttendeeId { get; set; }
        public string Subject { get; set; }
        public string ToMailAddress { get; set; }
        public string ToName { get; set; }
        public string Location { get; set; }
        public string HeadingDescription { get; set; }

    }
}