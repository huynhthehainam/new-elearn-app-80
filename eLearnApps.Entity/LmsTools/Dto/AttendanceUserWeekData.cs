using System.Collections.Generic;

namespace eLearnApps.Entity.LmsTools.Dto
{
    public class AttendanceUserWeekData
    {
        public User UserInfo { get; set; }
        public List<AttendanceDataWithAttachment> AttendanceDataWithAttachments { get; set; }
        public AttendanceDataWithAttachment AttendanceDataUserWithAttachment { get; set; }
        public List<AttendanceData> AttendanceData { get; set; }
        public List<AttendanceAttachment> Attachments { get; set; }
    }
}