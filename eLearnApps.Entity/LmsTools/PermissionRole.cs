namespace eLearnApps.Entity.LmsTools
{
    public class PermissionRole : BaseEntity
    {
        public int Id { get; set; }
        public int PermissionId { get; set; }
        public int RoleId { get; set; }
    }
}