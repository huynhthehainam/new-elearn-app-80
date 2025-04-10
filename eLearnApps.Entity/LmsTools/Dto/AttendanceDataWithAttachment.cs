using System;

namespace eLearnApps.Entity.LmsTools.Dto
{
    public class AttendanceDataWithAttachment
    {
        public int? AttendanceDataId { get; set; }
        public int? AttendanceSessionId { get; set; }
        public int? UserId { get; set; }
        public decimal? Percentage { get; set; }
        public string? Remarks { get; set; }
        public int? LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedTime { get; set; }
        public bool? IsDeleted { get; set; }
        public int? AttendanceAttachmentId { get; set; }
        public string? FileName { get; set; }
        public string? AttachmentPath { get; set; }
        public int? Participation { get; set; }
        public bool? Excused { get; set; }
    }
}