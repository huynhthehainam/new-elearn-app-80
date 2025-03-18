using eLearnApps.Business.Interface;
using eLearnApps.Core;
using eLearnApps.Core.Caching;
using eLearnApps.Entity.LmsTools;
using eLearnApps.Models;
using System.Text.Json;

namespace eLearnApps.Helpers
{
    public class ClaimHelper
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ClaimHelper(IServiceProvider serviceProvider, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }
        public void SetUserInfoIntoCache(User user)
        {

            var cacheManager = _serviceProvider.GetRequiredService<ICacheManager>();
            SetIntoCache(cacheManager, user);

        }

        public void SetUserInfoIntoCache(int userId)
        {
            var cacheManager = _serviceProvider.GetRequiredService<ICacheManager>();
            var userService = _serviceProvider.GetRequiredService<IUserService>();

            var user = userService.GetById(userId);
            SetIntoCache(cacheManager, user);

        }
        //public void SetGptAccessIntoCache(int userId)
        //{

        //    var cacheManager = _serviceProvider.GetRequiredService<ICacheManager>();
        //    var gptPermissionRolesService = lifetimeScope.Resolve<IGPTPermissionRolesService>();

        //    // user info must have already been set at this stage. if not let exception handle it
        //    var userInfo = cacheManager.Get<UserModel>(string.Format(Constants.KeyUserInfo, userId));

        //    //// append STRM info since we'll be getting this info only after has selected their term
        //    var userGptPermissions = gptPermissionRolesService.GetGptPermissionsByUserId(userId);
        //    if (userGptPermissions.Count > 0)
        //    {
        //        userInfo.HasAdmin = userGptPermissions.Where(q => q.GPTRoleId == (int)GPTRoles.Admin).Count() > 0;
        //        var userGptPermission = userGptPermissions.FirstOrDefault();
        //        // refresh cache
        //        var currentGptAccess = new GptAccessModel()
        //        {
        //            UserId = userId,
        //            Acad_Career = userGptPermission.Acad_Career,
        //            Acad_Group = userGptPermission.Acad_Group,
        //            Acad_Org = userGptPermission.Acad_Org,
        //            GptRoleId = userGptPermission.GPTRoleId,

        //        };
        //        userInfo.CurrentGptAccess = new List<GptAccessModel> { currentGptAccess };
        //        userInfo.SelectedAcadCareer = userGptPermission.Acad_Career;
        //        userInfo.SelectedAcadGroup = userGptPermission.Acad_Group;
        //        userInfo.SelectedAcadOrg = userGptPermission.Acad_Org;
        //        userInfo.SelectedRoleId = userGptPermission.GPTRoleId;
        //        cacheManager.Set(string.Format(Constants.KeyUserInfo, userId), userInfo, System.Convert.ToInt32(Constants.Timeout));
        //    }

        //}

        //public static void SetUserStrmIntoCache(int userId, string strm, string selectedTerm)
        //{
        //    using (var lifetimeScope = AutofacDependencyResolver.Current.ApplicationContainer.BeginLifetimeScope("AutofacWebRequest"))
        //    {
        //        var cacheManager = lifetimeScope.Resolve<ICacheManager>();
        //        var userEnrollmentService = lifetimeScope.Resolve<IUserEnrollmentService>();

        //        // user info must have already been set at this stage. if not let exception handle it
        //        var userInfo = cacheManager.Get<UserModel>(string.Format(Constants.KeyUserInfo, userId));

        //        // append STRM info since we'll be getting this info only after has selected their term
        //        userInfo.SelectedStrm = strm;
        //        userInfo.SelectedTerm = selectedTerm;

        //        cacheManager.Set(string.Format(Constants.KeyUserInfo, userId), userInfo, Convert.ToInt32(Constants.Timeout));
        //    }
        //}

        // For users who have only a single entry in GPTPermissionRoles
        //public static void SetUserSelectedAcadCareerIntoCache(int userId)
        //{
        //    using (var lifetimeScope = AutofacDependencyResolver.Current.ApplicationContainer.BeginLifetimeScope("AutofacWebRequest"))
        //    {
        //        var cacheManager = lifetimeScope.Resolve<ICacheManager>();
        //        var userEnrollmentService = lifetimeScope.Resolve<IUserEnrollmentService>();
        //        var gptPermissionRolesService = lifetimeScope.Resolve<IGPTPermissionRolesService>();

        //        // user info must have already been set at this stage. if not let exception handle it
        //        var userInfo = cacheManager.Get<UserModel>(string.Format(Constants.KeyUserInfo, userId));

        //        // Get the permission for this user
        //        var userGptPermissions = gptPermissionRolesService
        //                                    .GetGptPermissionsByUserId(userId)
        //                                    .FirstOrDefault();

        //        userInfo.SelectedAcadCareer = userGptPermissions.Acad_Career;
        //        userInfo.SelectedAcadGroup = userGptPermissions.Acad_Group;
        //        userInfo.SelectedAcadOrg = userGptPermissions.Acad_Org;
        //        userInfo.SelectedRoleId = userGptPermissions.GPTRoleId;

        //        cacheManager.Set(string.Format(Constants.KeyUserInfo, userId), userInfo, Convert.ToInt32(Constants.Timeout));
        //    }
        //}

        //public static void SetUserSelectedAcadCareerIntoCache(int userId, string acadCareer, string acadGroup, string acadOrg, int roleId)
        //{
        //    using (var lifetimeScope = AutofacDependencyResolver.Current.ApplicationContainer.BeginLifetimeScope("AutofacWebRequest"))
        //    {
        //        var cacheManager = lifetimeScope.Resolve<ICacheManager>();
        //        var userEnrollmentService = lifetimeScope.Resolve<IUserEnrollmentService>();
        //        var gptPermissionRolesService = lifetimeScope.Resolve<IGPTPermissionRolesService>();

        //        // user info must have already been set at this stage. if not let exception handle it
        //        var userInfo = cacheManager.Get<UserModel>(string.Format(Constants.KeyUserInfo, userId));

        //        // Get the associated permission for this acad career
        //        var userGptPermissions = gptPermissionRolesService
        //                                    .GetGptPermissionsByUserId(userId)
        //                                    .Where(q => q.Acad_Career == acadCareer)
        //                                    .Where(q => q.Acad_Group == acadGroup)
        //                                    .Where(q => q.Acad_Org == acadOrg)
        //                                    .Where(q => q.GPTRoleId == roleId)
        //                                    .FirstOrDefault();

        //        if (userGptPermissions != null)
        //        {
        //            // append acadcareer info for users with multiple acadcareer entries 
        //            userInfo.SelectedAcadCareer = userGptPermissions.Acad_Career;
        //            userInfo.SelectedAcadGroup = userGptPermissions.Acad_Group;
        //            userInfo.SelectedAcadOrg = userGptPermissions.Acad_Org;
        //            userInfo.SelectedRoleId = userGptPermissions.GPTRoleId;
        //        }

        //        cacheManager.Set(string.Format(Constants.KeyUserInfo, userId), userInfo, Convert.ToInt32(Constants.Timeout));
        //    }
        //}

        private void SetIntoCache(ICacheManager cacheManager, User user)
        {
            var constants = new Constants(_configuration);
            if (!cacheManager.IsSet(string.Format(constants.KeyUserInfo, user.Id)))
            {
                // user into not yet in cache. 
                var userInfo = new UserModel
                {
                    UserId = user.Id,
                    DisplayName = user.DisplayName,
                    EmailAddress = user.EmailAddress,
                    UserName = user.DisplayName,
                    CurrentLoadedCourses = new List<EnrollmentModel>(),
                    CurrentGptAccess = new List<GptAccessModel>(),
                    SelectedStrm = string.Empty,
                    SelectedTerm = string.Empty,
                    SelectedAcadCareer = string.Empty,
                    SelectedAcadGroup = string.Empty,
                    SelectedAcadOrg = string.Empty,
                    SelectedRoleId = -1
                };

                cacheManager.Set(string.Format(constants.KeyUserInfo, user.Id), userInfo, Convert.ToInt32(constants.Timeout));
            }
            else
            {
                // reset previous selection
                var userInfo = cacheManager.Get<UserModel>(string.Format(constants.KeyUserInfo, user.Id));
                userInfo.SelectedStrm = string.Empty;
                userInfo.SelectedTerm = string.Empty;
                userInfo.SelectedAcadCareer = string.Empty;
                userInfo.SelectedAcadGroup = string.Empty;
                userInfo.SelectedAcadOrg = string.Empty;
                userInfo.SelectedRoleId = -1;

            }
        }

        public UserModel GetUserInfoFromCache(int userId)
        {
            var constants = new Constants(_configuration);
            var cacheManager = _serviceProvider.GetRequiredService<ICacheManager>();
            return cacheManager.Get<UserModel>(string.Format(constants.KeyUserInfo, userId));

        }

        //public static List<GptAccessModel> GetGptAccessFromCache(int userId)
        //{
        //    using (var lifetimeScope = AutofacDependencyResolver.Current.ApplicationContainer.BeginLifetimeScope("AutofacWebRequest"))
        //    {
        //        var cacheManager = lifetimeScope.Resolve<ICacheManager>();
        //        var gptPermissionRolesService = lifetimeScope.Resolve<IGPTPermissionRolesService>();

        //        var userInfo = cacheManager.Get<UserModel>(string.Format(Constants.KeyUserInfo, userId));
        //        if (userInfo == null) return null;

        //        // We are currently not looking up this info in cache, but instead we are
        //        // calling the DB. In future we will modify this.

        //        var userGptPermissions = gptPermissionRolesService.GetGptPermissionsByUserId(userId);
        //        if (userGptPermissions != null)
        //        {
        //            userInfo.CurrentGptAccess = new List<GptAccessModel>();

        //            foreach (var permissions in userGptPermissions)
        //            {
        //                userInfo.CurrentGptAccess.Add(
        //                    new GptAccessModel()
        //                    {
        //                        UserId = userId,
        //                        Acad_Career = permissions.Acad_Career,
        //                        Acad_Group = permissions.Acad_Group,
        //                        Acad_Org = permissions.Acad_Org,
        //                        GptRoleId = permissions.GPTRoleId,
        //                    }
        //                );
        //            }
        //            cacheManager.Set(string.Format(Constants.KeyUserInfo, userId), userInfo, System.Convert.ToInt32(Constants.Timeout));
        //        }
        //        else return null;

        //        return userInfo.CurrentGptAccess.ToList();
        //    }

        //}

        public EnrollmentModel GetEnrollmentFromCache(int userId, int courseId)
        {
            var constants = new Constants(_configuration);
            var cacheManager = _serviceProvider.GetRequiredService<ICacheManager>();
            var userEnrollmentService = _serviceProvider.GetRequiredService<IUserEnrollmentService>();

            // user info must have already been set at this stage. if not let exception handle it
            var userInfo = cacheManager.Get<UserModel>(string.Format(constants.KeyUserInfo, userId));
            if (userInfo == null) return null;

            // look for permission in cache, if not in cache then load from DB.
            var currentCourseEnrollment = userInfo.CurrentLoadedCourses.FirstOrDefault(c => c.CourseId == courseId);
            if (currentCourseEnrollment == null)
            {
                // course not found in cache, try to load from DB
                var enrollment = userEnrollmentService.FindByUserIdAndCourseId(userId, courseId);
                if (enrollment != null)
                {
                    // refresh cache
                    currentCourseEnrollment = new EnrollmentModel()
                    {
                        CourseId = enrollment.CourseId,
                        CourseCode = enrollment.CourseCode,
                        CourseName = enrollment.CourseName,
                        RoleId = enrollment.RoleId,
                        RoleName = enrollment.RoleName,
                        SemesterId = enrollment.SemesterId
                    };
                    userInfo.CurrentLoadedCourses.Add(currentCourseEnrollment);
                    cacheManager.Set(string.Format(constants.KeyUserInfo, userId), userInfo, System.Convert.ToInt32(constants.Timeout));
                }
                else return null;
            }

            return currentCourseEnrollment;
        }


        /// <summary>
        /// UserInfo must have already been set into cache.
        /// This function add enrollment information for courseId into userInfo pointed by UserId
        /// enrollment info already existing, this function will not do anything
        /// </summary>
        /// <param name="userId">UserId which is stored in claim/identity</param>
        /// <param name="courseId">courseId which is stored in querystring</param>
        private UserModel SetPermissionIntoCache(int userId, int courseId)
        {
            var constants = new Constants(_configuration);
            var cacheManager = _serviceProvider.GetRequiredService<ICacheManager>();
            var userEnrollmentService = _serviceProvider.GetRequiredService<IUserEnrollmentService>();

            // user info must have already been set at this stage. if not let exception handle it
            var userInfo = cacheManager.Get<UserModel>(string.Format(constants.KeyUserInfo, userId));
            // look for permission in cache, if not in cache then load from DB.
            var currentCourseEnrollment = userInfo.CurrentLoadedCourses.FirstOrDefault(c => c.CourseId == courseId);
            if (currentCourseEnrollment == null)
            {
                // course not found in cache, try to load from DB
                var enrollment = userEnrollmentService.FindByUserIdAndCourseId(userId, courseId);
                if (enrollment != null)
                {
                    // refresh cache
                    currentCourseEnrollment = new EnrollmentModel()
                    {
                        CourseId = courseId,
                        CourseCode = enrollment.CourseCode,
                        RoleId = enrollment.RoleId,
                        RoleName = enrollment.RoleName,
                        SemesterId = enrollment.SemesterId
                    };
                    userInfo.CurrentLoadedCourses.Add(currentCourseEnrollment);
                    cacheManager.Set(string.Format(constants.KeyUserInfo, userId), userInfo, System.Convert.ToInt32(constants.Timeout));
                }
            }

            return userInfo;

        }

        /// <summary>
        /// get permissions for given roleId.
        /// when roleId not found in cache or when reload flag given, then load from DB and refresh cache
        /// </summary>
        /// <param name="roleId">Role id of which permissions is sought</param>
        /// <param name="reload">Will refresh content from DB when true</param>
        public List<PermissionViewModel> GetPermissionCache(int roleId, bool reload = false)
        {
            var constants = new Constants(_configuration);
            var cacheManager = _serviceProvider.GetRequiredService<ICacheManager>();
            var userEnrollmentService = _serviceProvider.GetRequiredService<IUserEnrollmentService>();

            // set empty role permission cache if needed
            if (!cacheManager.IsSet(constants.KeyRolePermission))
            {
                // get role permission and set it
                cacheManager.Set(constants.KeyRolePermission, new List<RolePermissionViewModel>(), 60);
            }

            // get main container for role permissions
            var rolePermissions = cacheManager.Get<List<RolePermissionViewModel>>(constants.KeyRolePermission);
            if (rolePermissions == null)
                rolePermissions = new List<RolePermissionViewModel>();
            rolePermissions = rolePermissions.ToList();

            var rolePermission = rolePermissions.ToList() // add to list to copy. 
                .Where(rp => rp.RoleId == roleId).FirstOrDefault();
            if (rolePermission == null)
            {
                rolePermission = new RolePermissionViewModel();
                rolePermissions.Add(rolePermission);

                // permission not found in cache, load from DB
                reload = true;
            }

            // load from DB and refresh cache
            if (reload)
            {
                // get permissions from DB
                var permissions = userEnrollmentService.GetPermissionsForRoleId(roleId).Select(p => new PermissionViewModel()
                {
                    Id = p.Id,
                    Name = p.Name,
                    SystemName = p.SystemName,
                    Category = p.Category,
                    Url = p.Url,
                    Order = p.Order
                })
                    .ToList();

                // refresh cache
                rolePermission.Permissions = permissions;
                cacheManager.Set(constants.KeyRolePermission, rolePermissions, Convert.ToInt32(constants.Timeout));
            }

            return rolePermission.Permissions;


        }

        public void ClearUserFromCache(int userId)
        {
            var constants = new Constants(_configuration);
            var cacheManager = _serviceProvider.GetRequiredService<ICacheManager>();

            cacheManager.Remove(string.Format(constants.KeyUserInfo, userId));
        }
        public void SetPFAccessIntoCache(User user)
        {
            var constants = new Constants(_configuration);
            var cacheManager = _serviceProvider.GetRequiredService<ICacheManager>();
            var enrollmentService = _serviceProvider.GetRequiredService<IUserEnrollmentService>();

            var enrolls = enrollmentService.GetUserEnrolledWithCourseByUserId(user.Id);
            var roles = enrolls?.Select(x => x.RoleId).Distinct().ToList();
            var roleService = _serviceProvider.GetRequiredService<IRoleService>();
            var rolesByUser = roleService.GetRoleByUserId(user.Id);
            var isExistsRole = roles.Any() && rolesByUser.Any(x => roles.Contains(x.Id));
            if (isExistsRole)
            {
                var courses = enrolls.Select(x => new EnrollmentModel
                {
                    RoleId = x.RoleId,
                    CourseId = x.CourseId,
                    RoleName = x.RoleName,
                    CourseCode = x.CourseCode,
                }).ToList();
                var isStudent = rolesByUser
                    .Any(x => string.Equals(x.Name, RoleName.Student.ToString(), StringComparison.OrdinalIgnoreCase));
                var hasAdmin = constants.PeerFeedbackAdmins.IndexOf(user.Id.ToString(), StringComparison.Ordinal) > -1;
                var isInstructor = rolesByUser
                    .Any(x => string.Equals(x.Name, RoleName.Instructor.ToString(), StringComparison.OrdinalIgnoreCase));
                // user info must have already been set at this stage. if not let exception handle it
                var userInfo = new UserModel
                {
                    UserId = user.Id,
                    DisplayName = user.DisplayName,
                    EmailAddress = user.EmailAddress,
                    UserName = user.DisplayName,
                    SelectedStrm = string.Empty,
                    SelectedTerm = string.Empty,
                    SelectedAcadCareer = string.Empty,
                    SelectedAcadGroup = string.Empty,
                    SelectedAcadOrg = string.Empty,
                    SelectedRoleId = -1,
                    IsStudent = isStudent,
                    HasAdmin = hasAdmin,
                    IsInstructor = isInstructor,
                    CurrentLoadedCourses = courses
                };
                var context = _httpContextAccessor.HttpContext;
                context.Session.SetString(constants.SessionUserKey, JsonSerializer.Serialize(userInfo));
            }
            else
            {
                throw new ArgumentNullException("role doesn't exist.");
            }

        }

    }
}