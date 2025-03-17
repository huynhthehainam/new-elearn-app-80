using System;

namespace eLearnApps.Entity.Logging
{
    public class ErrorLog
    {
        public long ErrorLogId { get; set; }
        public int UserId { get; set; }
        public string ToolId { get; set; }
        public int OrgUnitId { get; set; }
        public int ToolAccessRoleId { get; set; }
        public string ErrorPage { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorDetails { get; set; }
        public DateTime? ErrorTime { get; set; }
        public string IpAddress { get; set; }
        public string SessionId { get; set; }
    }
}