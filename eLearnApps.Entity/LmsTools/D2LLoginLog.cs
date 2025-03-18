using System;

namespace eLearnApps.Entity.LmsTools
{
    public class D2LLoginLog : BaseEntity
    {
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public string IPAddress { get; set; } = string.Empty;
        public string D2LTokenDigest { get; set; } = string.Empty;
        public int UserId { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
