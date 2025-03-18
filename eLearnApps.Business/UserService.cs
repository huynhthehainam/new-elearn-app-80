#region USING

using eLearnApps.Business.Interface;
using eLearnApps.Data;
using eLearnApps.Data.Interface;
using eLearnApps.Data.Interface.Lmsisis;
using eLearnApps.Data.Interface.LmsTool;
using eLearnApps.Entity.LmsIsis;
using eLearnApps.Entity.LmsTools;
using eLearnApps.Entity.LmsTools.Dto;
using eLearnApps.ViewModel.Security;
using log4net;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using eLearnApps.Core;
using eLearnApps.Core.Extension;
using System.Data.Entity;
using Microsoft.Extensions.DependencyInjection;

#endregion


namespace eLearnApps.Business
{
    public class UserService : IUserService
    {
        #region REPOSITORY
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserEnrollment> _userEnrollmentRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<PermissionRole> _permissionRoleRepo;
        private readonly IRepository<Permission> _permissionRepo;
        private readonly IRepository<CategoryGroup> _categoryGroupRepository;
        private readonly IRepository<Entity.LmsTools.CourseCategory> _courseCategoryRepository;
        private readonly IRepository<UserGroup> _userGroupRepository;
        private readonly IRepository<D2LLoginLog> _D2LLoginLogRepository;
        private readonly IRepository<StudentPhoto> _studentPhotoRepository;
        private readonly IDbContext _context;
        private readonly IUserDao _userDao;
        private readonly IPS_SIS_LMS_PHOTO_VDao _PS_SIS_LMS_PHOTO_VDao;
        private readonly ILoggingService _log;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region CTOR
        public UserService(
            IDbContext context,
          IServiceProvider serviceProvider,
            IDaoFactory factory,
            LMSIsisContext lmsisisContext,
            ILoggingService loggingService
        )
        {
            _context = context;
            _userRepository = serviceProvider.GetRequiredKeyedService<IRepository<User>>("default");
            _userEnrollmentRepository = serviceProvider.GetRequiredKeyedService<IRepository<UserEnrollment>>("default");
            _roleRepository = serviceProvider.GetRequiredKeyedService<IRepository<Role>>("default");
            _permissionRoleRepo = serviceProvider.GetRequiredKeyedService<IRepository<PermissionRole>>("default");
            _permissionRepo = serviceProvider.GetRequiredKeyedService<IRepository<Permission>>("default");
            _categoryGroupRepository = serviceProvider.GetRequiredKeyedService<IRepository<CategoryGroup>>("default");
            _userGroupRepository = serviceProvider.GetRequiredKeyedService<IRepository<UserGroup>>("default");
            _courseCategoryRepository = serviceProvider.GetRequiredKeyedService<IRepository<Entity.LmsTools.CourseCategory>>("default");
            _D2LLoginLogRepository = serviceProvider.GetRequiredKeyedService<IRepository<D2LLoginLog>>("default");

            _studentPhotoRepository = new Repository<StudentPhoto>(lmsisisContext);

            _userDao = factory.UserDao;
            _PS_SIS_LMS_PHOTO_VDao = factory.PS_SIS_LMS_PHOTO_VDao;

            _log = loggingService;
        }
        #endregion

        public User GetById(int id)
        {
            return _userRepository.GetById(id);
        }

        public IEnumerable<User> GetByIds(IEnumerable<int> Ids)
        {
            var x = _userRepository.TableNoTracking.Where(u => Ids.Contains(u.Id)).ToList();
            return x;
        }

        public void Insert(User user)
        {
            _userRepository.Insert(user);
        }

        public void Update(User user)
        {
            _userRepository.Update(user);
        }

        public void Delete(User user)
        {
            _userRepository.Delete(user);
        }

        public void Insert(List<User> users)
        {
            _userRepository.Insert(users);
        }

        public User GetByOrgDefinedId(string orgDefinedId)
        {
            var queryUserEnrollments = _userEnrollmentRepository.Table;
            var queryUsers = _userRepository.Table;
            var roles = _roleRepository.Table;

            var query = from user in queryUsers
                        where user.OrgDefinedId == orgDefinedId
                        select user;

            return query.FirstOrDefault();
        }

        public List<TeamMate> GetByCourseId(int courseId)
        {
            var queryUserEnrollments = _userEnrollmentRepository.Table;
            var queryUsers = _userRepository.Table;
            var roles = _roleRepository.Table;
            var query = from userEnrollments in queryUserEnrollments
                        join users in queryUsers on userEnrollments.UserId equals users.Id
                        where userEnrollments.CourseId == courseId/* && userEnrollments.IsClasslist == true*/
                        select new TeamMate
                        {
                            Id = users.Id,
                            Name = users.DisplayName,
                            RoleName = roles.FirstOrDefault(x => x.Id == userEnrollments.RoleId) != null ? roles.FirstOrDefault(x => x.Id == userEnrollments.RoleId).Name : string.Empty,
                            OrgDefinedId = users.OrgDefinedId
                        };
            return query.ToList();
        }

        public List<TeamMate> GetByCourseCategoryId(int courseCategoryId)
        {
            var queryCourseCategory = _courseCategoryRepository.Table;
            var queryUserEnrollments = _userEnrollmentRepository.Table;
            var roles = _roleRepository.Table;
            var queryCategoryGroups = _categoryGroupRepository.Table;
            var queryUserGroups = _userGroupRepository.Table;
            var queryUsers = _userRepository.Table;

            var query = from courseCategories in queryCourseCategory
                        join categoryGroups in queryCategoryGroups on courseCategories.Id equals categoryGroups.CourseCategoryId
                        join userGroups in queryUserGroups on categoryGroups.Id equals userGroups.CategoryGroupId
                        join users in queryUsers on userGroups.UserId equals users.Id
                        join userEnrollments in queryUserEnrollments on new { UserId = users.Id, CourseId = courseCategories.CourseId.Value } equals new { UserId = userEnrollments.UserId.Value, CourseId = userEnrollments.CourseId.Value }
                        where categoryGroups.CourseCategoryId == courseCategoryId
                        select new TeamMate
                        {
                            Id = users.Id,
                            Name = users.DisplayName,
                            RoleName = roles.FirstOrDefault(x => x.Id == userEnrollments.RoleId) != null ? roles.FirstOrDefault(x => x.Id == userEnrollments.RoleId).Name : string.Empty,
                            OrgDefinedId = users.OrgDefinedId
                        };
            return query.ToList();
        }

        public List<TeamMate> GetByCategoryGroupId(int categoryGroupId)
        {
            var queryCourseCategory = _courseCategoryRepository.Table;
            var queryCategoryGroups = _categoryGroupRepository.Table;
            var queryUserGroups = _userGroupRepository.Table;
            var queryUsers = _userRepository.Table;
            var queryUserEnrollments = _userEnrollmentRepository.Table;
            var roles = _roleRepository.Table;

            var query = from courseCategories in queryCourseCategory
                        join categoryGroups in queryCategoryGroups on courseCategories.Id equals categoryGroups.CourseCategoryId
                        join userGroups in queryUserGroups on categoryGroups.Id equals userGroups.CategoryGroupId
                        join users in queryUsers on userGroups.UserId equals users.Id
                        join userEnrollments in queryUserEnrollments on new { UserId = users.Id, CourseId = courseCategories.CourseId.Value } equals new { UserId = userEnrollments.UserId.Value, CourseId = userEnrollments.CourseId.Value }
                        where categoryGroups.Id == categoryGroupId
                        select new TeamMate
                        {
                            Id = users.Id,
                            Name = users.DisplayName,
                            RoleName = roles.FirstOrDefault(x => x.Id == userEnrollments.RoleId) != null ? roles.FirstOrDefault(x => x.Id == userEnrollments.RoleId).Name : string.Empty,
                            Section = string.Empty,
                            OrgDefinedId = users.OrgDefinedId
                        };
            return query.ToList();
        }

        public List<User> GetByCategoryGroupIds(List<int> groupIds)
        {
            // when null or empty list just return empty list
            if (groupIds == null || groupIds.Count() == 0) return new List<User>();

            var query = from userGroup in _userGroupRepository.TableNoTracking
                        join user in _userRepository.TableNoTracking on userGroup.UserId equals user.Id
                        where groupIds.Contains(userGroup.CategoryGroupId.Value)
                        select user;

            return query.ToList();
        }

        public List<User> GetListAttendeesById(List<int> ids)
        {
            var query = _userRepository.Table;
            var result = query.Where(x => ids.Contains(x.Id)).OrderBy(x => x.DisplayName);
            return result.ToList();
        }

        public void Save(List<User> users)
        {
            var userIds = users.Select(usr => usr.Id).ToList();
            var usersInDb = _userRepository.TableNoTracking.Where(usr => userIds.Contains(usr.Id)).ToList();
            var usersIdInDb = usersInDb.Select(usr => usr.Id).ToList();

            // if users is not in DB then it is new user
            var newUsers = users.Where(usr => !usersIdInDb.Contains(usr.Id)).ToList();
            // join 
            var userToUpdate = (from d2lUser in users
                                join dbUser in usersInDb on d2lUser.Id equals dbUser.Id
                                where d2lUser.DisplayName != dbUser.DisplayName || d2lUser.EmailAddress != dbUser.EmailAddress ||
                                     d2lUser.OrgDefinedId != dbUser.OrgDefinedId || d2lUser.ProfileBadgeUrl != dbUser.ProfileBadgeUrl ||
                                     d2lUser.ProfileIdentifier != dbUser.ProfileIdentifier
                                select new User
                                {
                                    Id = dbUser.Id,
                                    DisplayName = d2lUser.DisplayName,
                                    EmailAddress = d2lUser.EmailAddress,
                                    OrgDefinedId = d2lUser.OrgDefinedId,
                                    ProfileBadgeUrl = d2lUser.ProfileBadgeUrl,
                                    ProfileIdentifier = d2lUser.ProfileIdentifier
                                }).ToList();

            if (newUsers.Count > 0) _userRepository.Insert(newUsers);

            _userRepository.Update(userToUpdate);
        }

        public D2LLoginLog GetLastLogin(int userId, string IPAddress, string D2LTokenDigest)
        {
            var utcNow = DateTime.UtcNow;

            // find if any login with this token by this user
            var currentLoginRecordCount = _D2LLoginLogRepository.Table
                .Where(l => l.D2LTokenDigest == D2LTokenDigest && l.UserId == userId)
                .Count();
            if (currentLoginRecordCount == 0)
                _D2LLoginLogRepository.Insert(new D2LLoginLog()
                {
                    TimeStamp = utcNow,
                    IPAddress = IPAddress,
                    D2LTokenDigest = D2LTokenDigest,
                    UserId = userId,
                    CreatedOn = utcNow
                });

            var lastLoginRecord = _D2LLoginLogRepository.Table
                .Where(l => l.UserId == userId && l.D2LTokenDigest != D2LTokenDigest)
                .OrderByDescending(l => l.CreatedOn)
                .FirstOrDefault();

            return lastLoginRecord;
        }

        /// <summary>
        /// Get list user by section id and course id
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="sectionId"></param>
        /// <param name="courseOfferingCode"></param>
        /// <returns></returns>
        public List<UserDto> GetUserBySection(int courseId, int sectionId, string courseOfferingCode)
        {
            return _userDao.GetUserBySection(courseId, sectionId, courseOfferingCode);
        }

        public List<UserDto> GetUserByCourseId(int courseId)
        {
            var queryUserEnrollments = _userEnrollmentRepository.Table;
            var queryUsers = _userRepository.Table;
            var query = from userEnrollments in queryUserEnrollments
                        join users in queryUsers on userEnrollments.UserId equals users.Id
                        join role in _roleRepository.Table on userEnrollments.RoleId equals role.Id
                        where userEnrollments.CourseId == courseId
                        orderby users.DisplayName // default ordering
                        select new UserDto
                        {
                            Id = users.Id,
                            DisplayName = users.DisplayName,
                            EmailAddress = users.EmailAddress,
                            OrgDefinedId = users.OrgDefinedId,
                            ProfileBadgeUrl = users.ProfileBadgeUrl,
                            ProfileIdentifier = users.ProfileIdentifier,
                            PhysicalPhotoPath = string.Empty, // TODO find usage of this 
                            Bidding = string.Empty, // TODO find usage of this
                            School = string.Empty, // TODO find usage of this
                            RoleName = role.Name,
                            RoleId = role.Id,
                            DisplayedGrade = string.Empty, // TODO find usage of this
                            IsSubmitted = false, // TODO find usage of this
                            IgradeId = -1 // TODO find usage of this
                        };
            return query.ToList();
        }

        /// <summary>
        /// Get all user in this course
        /// </summary>
        /// <param name="courseId"></param>
        /// <returns></returns>
        public List<User> GetAllUserInThisCourse(int courseId)
        {
            return _userDao.GetUserByCourse(courseId);
        }

        public List<UserDto> GetUserDtoForPhotoGen()
        {
            var orgDefinedIds = _PS_SIS_LMS_PHOTO_VDao.GetAllEmpId();

            var userDto = _userRepository.TableNoTracking
                .Where(usr => orgDefinedIds.Contains(usr.OrgDefinedId))
                .Select(usr => new UserDto()
                {
                    Id = usr.Id,
                    OrgDefinedId = usr.OrgDefinedId
                })
                .ToList();

            return userDto;
        }

        // TODO: need to delete this function
        public void GeneratePhotoAtPath(string path, List<UserDto> users)
        {
            var leadingZero = new char[] { '0' };

            log.Info($"Start Generating photos for Users: {users.Select(usr => usr.Id.ToString()).Aggregate((i, j) => $"{i}, {j}")} student(s). Generating photo.");

            // NOTE: cannot query in bulk. will cause DB connection timeout.
            foreach (var user in users)
            {
                log.Info($"Generating photos for : {user.OrgDefinedId}");

                var studentPhoto = _PS_SIS_LMS_PHOTO_VDao.GetStudentPhotoByOrgDefineId(user.OrgDefinedId.Trim());

                if (studentPhoto != null)
                {
                    var filename = eLearnApps.Core.Util.GetPhotoFileName(user.OrgDefinedId, user.Id);

                    // TODO: refactor ImageFolderSize and PhotoSize to Core
                    var smallPhotoFileName = $"{path}/small/{filename}.jpg";
                    var mediumPhotoFileName = $"{path}/medium/{filename}.jpg";
                    var largePhotoFileName = $"{path}/large/{filename}.jpg";

                    try
                    {
                        // SQL to byte[] may do a left padding which turn image unreadable.
                        // we need to detect this and change to right padding
                        var photoStr = studentPhoto.Photo.ByteArrayToString();
                        var fixPhotoStr = photoStr.TrimStart(leadingZero);

                        // right pad 0 when odd length
                        if (fixPhotoStr.Length % 2 != 0)
                            fixPhotoStr += "0";

                        var photoByte = fixPhotoStr.ToByteArray();
                        using (var ms = new MemoryStream(photoByte))
                        {
                            var returnImage = Image.FromStream(ms);

                            // TODO: refactor sizes to core
                            var largePhoto = returnImage.ResizeImage((int)StudentPhotoSizeLarge.Width, (int)StudentPhotoSizeLarge.Height);
                            var mediumPhoto = returnImage.ResizeImage((int)StudentPhotoSizeMedium.Width, (int)StudentPhotoSizeMedium.Height);
                            var smallPhoto = returnImage.ResizeImage((int)StudentPhotoSizeSmall.Width, (int)StudentPhotoSizeSmall.Height);

                            smallPhoto.Save(smallPhotoFileName, ImageFormat.Jpeg);
                            mediumPhoto.Save(mediumPhotoFileName, ImageFormat.Jpeg);
                            largePhoto.Save(largePhotoFileName, ImageFormat.Jpeg);
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error($"Unable to generate/write photo to file for {user.OrgDefinedId}", ex);
                    }
                }
                else
                {
                    log.Info($"Unable to find photo for {user.OrgDefinedId}");
                }
            }
        }

        public List<Role> GetAllRoles() => _roleRepository.Table.ToList();

        public List<UserEnrollmentViewModel> GetUserEnrollmentForAllCourse(int userId)
        {
            var enrollment = (from userEnrollment in _userEnrollmentRepository.Table
                              where userEnrollment.UserId == userId
                              select new UserEnrollmentViewModel
                              {
                                  UserId = userId,
                                  CourseId = userEnrollment.CourseId.Value,
                                  RoleId = userEnrollment.RoleId.Value,
                              }).ToList();

            return enrollment;
        }

        public List<RolePermissionViewModel> GetRolePermission(List<int> roleIds)
        {
            var permissions = (from role in _roleRepository.Table
                               join pr in _permissionRoleRepo.Table on role.Id equals pr.RoleId
                               join permission in _permissionRepo.Table on pr.PermissionId equals permission.Id
                               where roleIds.Contains(role.Id)
                               select new RolePermissionViewModel
                               {
                                   RoleId = role.Id,
                                   PermissionName = permission.Name,
                                   PermissionSystemName = permission.SystemName,
                                   PermissionUrl = permission.Url,
                                   PermissionCategory = permission.Category,
                                   PermissionOrder = permission.Order
                               }).ToList();

            return permissions;
        }

        // TODO duplicate with userenrollmentservice
        public List<UserEnrollment> GetEnrollmentFor(int userId, int courseId) =>
            _userEnrollmentRepository.Table
            .Where(enrollment => enrollment.UserId == userId && enrollment.CourseId == courseId)
            .ToList();

        /// <summary>
        /// Get user by list course id
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public List<UserDto> GetByListCourse(List<int> courseId, List<int> roles)
        {
            return _userDao.GetByListCourse(courseId, roles);
        }

        /// <summary>
        /// Get user enroll by id , course id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="courseId"></param>
        /// <returns></returns>
        public User GetUserEnrollByUserIdWithCourseId(int userId, int courseId)
        {
            return _userDao.GetUserEnrollByUserIdWithCourseId(userId, courseId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orgDefinedId"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public async Task<byte[]> GetStudentPhotoByOrgDefineIdAsync(string orgDefinedId, int width, int height)
        {
            if (string.IsNullOrEmpty(orgDefinedId)) throw new ArgumentNullException(nameof(orgDefinedId));
            var leadingZero = new char[] { '0' };
            var studentPhoto = await _PS_SIS_LMS_PHOTO_VDao.GetStudentPhotoByOrgDefineIdAsync(orgDefinedId).ConfigureAwait(false);

            // ensure null and no record does not cause issue
            if (studentPhoto == null) return null;
            if (studentPhoto.Photo == null) return null;

            // SQL to byte[] may do a left padding which turn image unreadable.
            // we need to detect this and change to right padding
            var photoStr = studentPhoto.Photo.ByteArrayToString();
            var fixPhotoStr = photoStr.TrimStart(leadingZero);

            // right pad 0 when odd length
            if (fixPhotoStr.Length % 2 != 0)
                fixPhotoStr += "0";

            var photoByte = fixPhotoStr.ToByteArray();
            byte[] studentPhotoInByte;

            try
            {
                using (var ms = new MemoryStream(photoByte))
                {
                    var image = Image.FromStream(ms);
                    var resizeImage = image.ResizeImage(width, height);
                    using (MemoryStream m = new MemoryStream())
                    {
                        resizeImage.Save(m, ImageFormat.Jpeg);
                        studentPhotoInByte = m.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(0, "User Service", 0, 0,
                    "GetStudentPhotoByOrgDefineIdAsync", $"Unable to convert DB value to image bytes. OrgDefnedId: {orgDefinedId}", ex.ToString(),
                   string.Empty, string.Empty);

                studentPhotoInByte = null;
            }

            return studentPhotoInByte;
        }

        public List<ItemDto> GetUserByGroupId(List<int> UserGroupIds)
        {
            var query = from user in _userRepository.TableNoTracking
                        join usergroup in _userGroupRepository.TableNoTracking on user.Id equals usergroup.UserId
                        where UserGroupIds.Contains(usergroup.CategoryGroupId.Value)
                        select new ItemDto()
                        {
                            Id = user.Id,
                            Name = user.DisplayName,
                            DisplayName = user.DisplayName,
                            Description = string.Empty,
                            Type = 0,
                            GroupId = usergroup.CategoryGroupId.Value,
                            OrgDefinedId = user.OrgDefinedId
                        };

            return query.ToList();
        }
    }
}