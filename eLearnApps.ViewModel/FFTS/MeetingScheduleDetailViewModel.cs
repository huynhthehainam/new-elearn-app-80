using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.ViewModel.FFTS
{
    public class MeetingScheduleDetailViewModel
    {
        public int? MeetingAttendeeId { get; set; }
        public int? MeetingId { get; set; }
        public int? AttendeeId { get; set; }
        public int? InviteStatus { get; set; }
        public string SecretKey { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? LastUpdatedOn { get; set; }
        public string AttendeeName { get; set; }
        public string Color { get; set; }
    }
}
