﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace eLearnApps.Data.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Users {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Users() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("eLearnApps.Data.Resources.Users", typeof(Users).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT DISTINCT UserId FROM UserEnrollments WHERE CourseId = @CourseId AND RoleId = @RoleId .
        /// </summary>
        internal static string UserEnrollments_GetAllStudentByCourse {
            get {
                return ResourceManager.GetString("UserEnrollments_GetAllStudentByCourse", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT DISTINCT UserId FROM UserEnrollments WHERE CourseId = @CourseId AND IsClasslist = 1.
        /// </summary>
        internal static string UserEnrollments_GetListUserIdEnroll {
            get {
                return ResourceManager.GetString("UserEnrollments_GetListUserIdEnroll", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT u.[Id]
        ///      ,u.[DisplayName]
        ///      ,u.[EmailAddress]
        ///      ,u.[OrgDefinedId]
        ///      ,u.[ProfileBadgeUrl]
        ///      ,u.[ProfileIdentifier]
        ///  FROM [Users] u INNER JOIN UserEnrollments ue ON u.id = ue.UserId AND ue.CourseId = @CourseId AND u.Id = @UserId .
        /// </summary>
        internal static string UserEnrollments_GetUserEnrollByCourseIdAndUserIdAsync {
            get {
                return ResourceManager.GetString("UserEnrollments_GetUserEnrollByCourseIdAndUserIdAsync", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT 
        ///	   u.[Id]
        ///      ,u.[DisplayName]
        ///      ,u.[EmailAddress]
        ///      ,u.[OrgDefinedId]
        ///      ,u.[ProfileBadgeUrl]
        ///      ,u.[ProfileIdentifier]
        ///	  ,ue.[CourseId]
        ///    ,ue.RoleId
        ///    ,(SELECT [Name] FROM Roles WHERE Id = ue.RoleId) AS RoleName
        ///FROM Users u INNER JOIN UserEnrollments ue ON ue.UserId = u.Id AND ue.RoleId IN ({0}) AND  ue.CourseId IN ({1}) ORDER BY u.[DisplayName].
        /// </summary>
        internal static string Users_GetByListCourse {
            get {
                return ResourceManager.GetString("Users_GetByListCourse", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT us.[Id],
        ///       us.[DisplayName],
        ///       us.[EmailAddress],
        ///       us.[OrgDefinedId],
        ///       us.[ProfileBadgeUrl],
        ///       us.[ProfileIdentifier]
        ///FROM Users us
        ///    INNER JOIN UserEnrollments ue
        ///        ON us.Id = ue.UserId
        ///           AND ue.CourseId = @CourseId;.
        /// </summary>
        internal static string Users_GetUserByCourse {
            get {
                return ResourceManager.GetString("Users_GetUserByCourse", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///	SELECT DISTINCT
        ///	us.Id,
        ///	us.DisplayName,
        ///	ig.IGradeId
        ///FROM 
        ///	Users us
        ///    INNER JOIN UserEnrollments ue ON ue.UserId = us.Id AND ue.CourseId = @CourseId
        ///	LEFT JOIN IGrades ig ON ig.UserId = us.Id AND ig.CourseId = ue.CourseId AND ig.SectionId = se.SectionId
        ///WHERE
        ///	 NOT EXISTS
        ///     (
        ///         SELECT gsg.StudentId
        ///         FROM GradeSubmissions gs
        ///             INNER JOIN GradeSubmissionGrades gsg
        ///                 ON gsg.CourseId = gs.CourseId
        ///                    AND gsg.CourseOfferingCode =  [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Users_GetUserBySection {
            get {
                return ResourceManager.GetString("Users_GetUserBySection", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT us.[Id],
        ///       us.[DisplayName],
        ///       us.[EmailAddress],
        ///       us.[OrgDefinedId],
        ///       us.[ProfileBadgeUrl],
        ///       us.[ProfileIdentifier]
        ///FROM Users us
        ///    INNER JOIN UserEnrollments ue
        ///        ON us.Id = ue.UserId
        ///           AND ue.CourseId = @CourseId AND us.Id = @UserId;.
        /// </summary>
        internal static string Users_GetUserEnrollByUserIdWithCourseId {
            get {
                return ResourceManager.GetString("Users_GetUserEnrollByUserIdWithCourseId", resourceCulture);
            }
        }
    }
}
