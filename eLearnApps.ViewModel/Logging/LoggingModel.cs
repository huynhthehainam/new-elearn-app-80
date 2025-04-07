using System;

namespace eLearnApps.ViewModel.Logging
{
    public class LoggingModel
    {
        public int UserId { get; set; }
        public string ToolId { get; set; }
        public int OrgUnitId { get; set; }
        public int RoleId { get; set; }
        public string Page { get; set; }
        public string ShortMessage { get; set; }
        public string FullMessage { get; set; }
        public string IpAddress { get; set; }
        public string SessionId { get; set; }
        public string ExecutableName { get; set; }
        public DateTime JobStart { get; set; }
        public DateTime JobEnd { get; set; }
        public string Parameter { get; set; }
        public string LogLevel { get; set; }
        public string UserName { get; set; }
    }
}