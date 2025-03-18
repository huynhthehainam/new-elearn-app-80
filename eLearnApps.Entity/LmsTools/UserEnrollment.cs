namespace eLearnApps.Entity.LmsTools
{
    public class UserEnrollment : BaseEntity
    {
        public int Id { get; set; }
        public int? CourseId { get; set; }
        public int? UserId { get; set; }
        public int? RoleId { get; set; }
        public bool IsClasslist { get; set; }
    }
}