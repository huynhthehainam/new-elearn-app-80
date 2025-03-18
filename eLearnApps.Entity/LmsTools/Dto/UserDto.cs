namespace eLearnApps.Entity.LmsTools.Dto
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Identifier { get; set; }
        public string DisplayName { get; set; }
        public string EmailAddress { get; set; }
        public string OrgDefinedId { get; set; }
        public string ProfileBadgeUrl { get; set; }
        public string ProfileIdentifier { get; set; }
        public string PhysicalPhotoPath { get; set; }
        public string Bidding { get; set; }
        public string School { get; set; }
        public string RoleName { get; set; }
        public int RoleId { get; set; }
        public string DisplayedGrade { get; set; }
        public bool IsSubmitted { get; set; }
        public int IgradeId { get; set; }
        public int CourseId { get; set; }
        public string SectionName { get; set; }
        public byte[] Avatar { get; set; }
        public string InstructorInfo { get; set; }
    }
}