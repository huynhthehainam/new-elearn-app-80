namespace eLearnApps.Core.Domain.Users
{
    public class LoggedInUserInfo
    {
        public int UserId { get; set; }
        public int CourseId { get; set; }
        public string? ToolName { get; set; }
        public string? RoleName { get; set; }
    }
}
