using System;

namespace eLearnApps.Entity.LmsTools
{
    public class UserUploadFile
    {
        public Int64 Id { get; set; }
        public int UserId { get; set; }
        public byte[] FileBlob { get; set; } = [];
        public string FileMeta { get; set; } = string.Empty;
        public string Filename { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }
}
