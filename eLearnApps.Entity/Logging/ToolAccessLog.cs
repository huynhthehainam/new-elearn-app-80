using System;

namespace eLearnApps.Entity.Logging
{
    public class ToolAccessLog
    {
        public long ToolAccessLogId { get; set; }
        public string ToolId { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public DateTime AccessTime { get; set; }
        public string IpAddress { get; set; }
        public int OrgUnitId { get; set; }
        public int OrgUnitRoleId { get; set; }
        public string SessionId { get; set; }
    }
}