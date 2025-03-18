using eLearnApps.Entity.LmsTools;
// ReSharper disable All

namespace eLearnApps.Entity.Security
{
    /// <summary>
    /// Base roles
    /// </summary>
    public class StandardRoleProvider
    {
        public static readonly Role SuperAdministrator = new Role{Id = 101, Code = string.Empty, Name = "Super Administrator" };
        public static readonly Role Learner = new Role {Id = 103, Code = "Learner", Name = "Learner" };
        public static readonly Role Administrator = new Role {Id = 105, Code = "", Name = "Administrator" };
        public static readonly Role Instructor = new Role {Id = 107, Code = "Instructor", Name = "Instructor" };
        public static readonly Role Student = new Role {Id = 108, Code = "", Name = "Student" };
        public static readonly Role TaBasic = new Role {Id = 109, Code = string.Empty, Name = "TA (Basic)" };
        public static readonly Role CourseCatalog = new Role {Id = 110, Code = string.Empty, Name = "CourseCatalog" };
        public static readonly Role GblWebApp = new Role {Id = 112, Code = string.Empty, Name = "GBLwebapp", };
        public static readonly Role TaContentDropbox = new Role {Id = 113, Code = string.Empty, Name = "TA (Content_Dropbox)" };
    }
}