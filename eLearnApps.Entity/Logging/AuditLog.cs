using System;

namespace eLearnApps.Entity.Logging
{
    public class AuditLog
    {
        public int UserId { get; set; }
        public required string ToolId { get; set; }
        public int OrgUnitId { get; set; }
        public int ToolAccessRoleId { get; set; }
        public required string ActionCategory { get; set; }
        public required string ActionDescription { get; set; }
        public DateTime ActionTime { get; set; }
        public required string IpAddress { get; set; }
        public string? SessionId { get; set; }
        public  string ExecutableName { get; set; }
        public string Parameters { get; set; }
        public DateTime JobStart { get; set; }
        public DateTime? JobEnd { get; set; }
        public string LogLevel { get; set; }
        public string LogContent { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}