using System;

namespace eLearnApps.Entity.Logging
{
    public class DebugLog
    {
        public int Id { get; set; }
        public string ShortMessage { get; set; }
        public string FullMessage { get; set; }
        public string IpAddress { get; set; }
        public string PageUrl { get; set; }
        public string ReferrerUrl { get; set; }
        public int UserId { get; set; }
        public int OrgUnitId { get; set; }
        public string SessionId { get; set; }
        public string ToolId { get; set; }
        public string Browser { get; set; }
        public string QueryString { get; set; }
        public string RequestBody { get; set; }
        public DateTime CreateOn { get; set; } = DateTime.UtcNow;
    }
}
