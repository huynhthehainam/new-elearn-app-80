namespace eLearnApps.Entity.LmsTools.Dto
{
    public class AttendanceUserData
    {
        public AttendanceData UserData { get; set; }
        public AttendanceAttachment Attachment { get; set; }
        public User UserInfo { get; set; }
    }
}