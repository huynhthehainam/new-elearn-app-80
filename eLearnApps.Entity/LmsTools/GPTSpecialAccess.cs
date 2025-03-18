namespace eLearnApps.Entity.LmsTools
{
    public class GPTSpecialAccess : BaseEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Course { get; set; } = string.Empty;
        public string? Section { get; set; }
        public int RoleId { get; set; }
        public int FilterLevel { get; set; }
        public string AcadCareer { get; set; } = string.Empty;
        public string AcadGroup { get; set; } = string.Empty;
        public string AcadOrg { get; set; } = string.Empty;
    }   
}
