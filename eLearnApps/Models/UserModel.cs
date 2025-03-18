using eLearnApps.Entity.LmsTools;
using System.Collections.Generic;

namespace eLearnApps.Models
{
    public class UserModel
    {
        public int UserId { get; set; }

        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public string DisplayName { get; set; }
        public string SelectedStrm { get; set; }
        public string SelectedTerm { get; set; }
        public string SelectedAcadCareer { get; set; }
        public string SelectedAcadGroup { get; set; }
        public string SelectedAcadOrg { get; set; }
        public int SelectedRoleId { get; set; }
        public bool HasAdmin { get; set; }
        public string UserKey { get; set; }
        public bool IsStudent { get; set; }
        public bool IsInstructor { get; set; }
        public List<EnrollmentModel> CurrentLoadedCourses { get; set; }
        public List<GptAccessModel> CurrentGptAccess { get; set; }
    }

    public class EnrollmentModel
    {
        public int SemesterId { get; set; }
        public int CourseId { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public int UserId { get; set; }
    }
    public class GptAccessModel
    {
        public int UserId { get; set; }
        public string Acad_Career { get; set; }
        public string Acad_Group { get; set; }
        public string Acad_Org { get; set; }
        public int GptRoleId { get; set; }
        public string GptRoleName { get; set; }
    }

    public class RolePermissionViewModel
    {
        public int RoleId { get; set; }
        public List<PermissionViewModel> Permissions { get; set; }
    }

    public class PermissionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SystemName { get; set; }
        public string Category { get; set; }
        public string Url { get; set; }
        public int Order { get; set; }
    }

    // Not Sure how to implement this
    public enum RoleType
    {
        Admin,
        Instructor,
        TA,
        Student,
        NA
    }
}