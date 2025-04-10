using System.ComponentModel.DataAnnotations;

namespace eLearnApps.Entity.LmsTools.Dto
{
    public class AttendanceUserData
    {
        public required AttendanceData UserData { get; set; }
        public required AttendanceAttachment Attachment { get; set; }
        public User? UserInfo { get; set; }
    }
}