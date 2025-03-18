namespace eLearnApps.Entity.LmsTools
{
    public class GPTPermissionRoles : BaseEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Acad_Career { get; set; } = string.Empty;
        public string Acad_Group { get; set; } = string.Empty;
        public string? Acad_Org { get; set; }
        public int GPTRoleId { get; set; }
        public string? Notes { get; set; }
        public string? Email { get; set; }
    }
}