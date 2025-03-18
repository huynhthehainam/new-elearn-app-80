using System;

namespace eLearnApps.Entity.LmsTools
{
    public class AttendanceAttachment : BaseEntity
    {
        public int AttendanceAttachmentId { get; set; }
        public int AttendanceDataId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public byte[]? AttachFile { get; set; }
    }
}