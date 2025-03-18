#region USING
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using eLearnApps.Business.Interface;
using eLearnApps.Core;
using eLearnApps.Core.Caching;
using eLearnApps.Core.Extension;
using eLearnApps.Data;
using eLearnApps.Data.Interface;
using eLearnApps.Entity.LmsIsis;
using eLearnApps.Entity.LmsTools;
using eLearnApps.Entity.Logging;
using eLearnApps.Entity.Logging.Dto;
using eLearnApps.Entity.Valence;
using eLearnApps.Valence;
using log4net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Role = eLearnApps.Entity.LmsTools.Role;
using User = eLearnApps.Entity.LmsTools.User;

#endregion

namespace eLearnApps.Business
{
    public class ValenceService : IValenceService
    {
        public enum ValenceRunMode
        {
            Full,
            Grade,
            GradeWithBookmark,
            EnrollmentOnly,
            Single
        }

        private readonly int _maxDegreeOfParallelism;
        private int _userEnrollmentThreshold => Convert.ToInt32(_configuration.GetValue<string>("UserEnrollThreshold"));
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly DataContext _dataContext;
        private readonly IConfiguration _configuration;
        #region CTOR

        public ValenceService(ICategoryGroupService categoryGroupService,
            ICourseCategoryService courseCategoryService,
            ICourseService courseService,
            ICourseTemplateService courseTemplateService,
            IDepartmentService departmentService,
            IRoleService roleService,
            ISemesterService semesterService,
            IUserEnrollmentService userEnrollment,
            IUserGroupService userGroupService,
            IServiceScopeFactory serviceScopeFactory,
            DataContext dataContext,
            IConfiguration configuration,
            IUserService userService)
        {
            _valence = new ValenceApi(configuration);
            _configuration = configuration;
            _courseTemplateService = courseTemplateService;
            _departmentService = departmentService;
            _semesterService = semesterService;
            _courseService = courseService;
            _userService = userService;
            _roleService = roleService;
            _userEnrollment = userEnrollment;
            _courseCategoryService = courseCategoryService;
            _categoryGroupService = categoryGroupService;
            _userGroupService = userGroupService;
            _scopeFactory = serviceScopeFactory;
            _dataContext = dataContext;
            _maxDegreeOfParallelism = Convert.ToInt32(configuration.GetValue<string>("MaxDegreeOfParallelism"));
        }

        #endregion

        /// <summary>
        ///     Primary entry point with specified runmode
        /// </summary>
        /// <param name="runmode"></param>
        public void Run(ValenceRunMode runmode)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var batchJob = new BatchJobLog
            {
                CreatedOn = DateTime.UtcNow,
                ExecutableName = nameof(ValenceService),
                JobStart = DateTime.UtcNow,
                Parameters = Convert.ToString(runmode),
                LastUpdatedOn = DateTime.UtcNow
            };

            var batchJobDetail = new BatchJobLogDetail
            {
                CreatedOn = DateTime.UtcNow,
                LogContent = Convert.ToString(runmode),
                LogLevel = LogLevel.Information.ToString()
            };

            try
            {
                switch (runmode)
                {
                    case ValenceRunMode.Full:
                        FullMode();
                        break;
                    case ValenceRunMode.Grade:
                        OnlyOpenCourseMode();
                        break;
                    case ValenceRunMode.GradeWithBookmark:
                        OnlyOpenCourseWithBookmarkMode();
                        break;
                    case ValenceRunMode.EnrollmentOnly:
                        OnlyEnrollmentMode();
                        break;
                }
            }
            catch (Exception e)
            {
                log.Error("Error encountered.", e);
            }
            finally
            {
                stopwatch.Stop();
                var msg = $"Job is Done. Elapse Time: {stopwatch.Elapsed.TotalMinutes} minutes.";

                batchJob.JobEnd = DateTime.UtcNow;
                batchJobDetail.LogContent = msg;

                log.Info(msg);
            }
        }

        public void RunSingle(int orgUnitId)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                SyncEnrollmentOnly(new List<int>() { orgUnitId });
            }
            catch (Exception e)
            {
                log.Error("Error encountered.", e);
            }
            finally
            {
                stopwatch.Stop();
                var msg = $"Job is Done. Elapse Time: {stopwatch.Elapsed.TotalMinutes} minutes.";

                log.Info(msg);
            }
        }

        public List<eLearnApps.ViewModel.Valence.UserDataViewModel> FindUserByEmail(string email)
        {
            var usersFound = _valence.FindUserByEmail(email);
            if (usersFound == null) return null;
            else
            {
                var vm = usersFound.Select(userData => new eLearnApps.ViewModel.Valence.UserDataViewModel()
                {
                    OrgId = userData.OrgId,
                    UserId = userData.UserId,
                    FirstName = userData.FirstName,
                    MiddleName = userData.MiddleName,
                    LastName = userData.LastName,
                    UserName = userData.UserName,
                    ExternalEmail = userData.ExternalEmail,
                    OrgDefinedId = userData.OrgDefinedId,
                    UniqueIdentifier = userData.UniqueIdentifier,
                    IsActive = userData.Activation.IsActive,
                    LastAccessedDate = userData.LastAccessedDate
                }).ToList();

                return vm;
            }
        }

        public ViewModel.Valence.EnrollmentData GetEnrollment(int orgUnitId, int userId)
        {
            var enrollment = _valence.GetEnrollmentFor(userId, orgUnitId);
            if (enrollment == null) return null;
            else
            {
                var vm = new eLearnApps.ViewModel.Valence.EnrollmentData()
                {
                    OrgUnitId = enrollment.OrgUnitId,
                    UserId = enrollment.UserId,
                    RoleId = enrollment.RoleId,
                    IsCascading = enrollment.IsCascading
                };

                return vm;
            }

        }

        /// <summary>
        ///     Start from empty bookmark, sync courses and enrollment in 1 loop
        /// </summary>
        private void FullMode()
        {
            var keepReading = true;
            var bookmark = string.Empty;

            var totalOrgUnitIds = new List<KeyValuePair<int, string>>();
            while (keepReading)
            {
                log.Info($"Get All Course From Api Start. bookmark:{bookmark}");
                var myOrgUnitInfo = _valence.GetAllCourses(bookmark);
                log.Info("Get All Course From Api Done ");
                var orgUnitIds = myOrgUnitInfo.Items
                    .Select(o => new KeyValuePair<int, string>(o.OrgUnit.Id, bookmark))
                    .ToList();

                totalOrgUnitIds.AddRange(orgUnitIds);

                // if has more page, update bookmark, else stop loop
                if (myOrgUnitInfo.PagingInfo.HasMoreItems)
                    bookmark = myOrgUnitInfo.PagingInfo.Bookmark;
                else keepReading = false;
            }

            // split courseIds into pages of 100, then process them in parallel
            var pagedCourseIds = Core.Util.SplitList(totalOrgUnitIds, 100);
            Parallel.ForEach(pagedCourseIds, new ParallelOptions { MaxDegreeOfParallelism = _maxDegreeOfParallelism },
                page =>
                {
                    SyncCourseOnly(page);
                    var cIds = page.Select(list => list.Key).ToList();
                    SyncEnrollmentOnly(cIds);
                });

            log.Info("Completed Successfully.");
        }

        /// <summary>
        ///     Sync all Courses information: update when there is changes, add when there is new
        ///     Get all "Open" CourseIds (Course /w gradebook not been submitted to RO) from DB
        ///     Sync only enrollment of these course Ids
        /// </summary>
        private void OnlyOpenCourseMode()
        {
            log.Info("Valence Sync job on Open Course Mode started");

            var keepReading = true;
            var bookmark = string.Empty;

            // Download all courses information, update if there is any differ from D2L, insert if new from D2L
            var totalOrgUnitIds = new List<KeyValuePair<int, string>>();
            while (keepReading)
            {
                log.Info($"Get All Course From Api Start. bookmark:{bookmark}");
                var myOrgUnitInfo = _valence.GetAllCourses(bookmark);
                log.Info("Get All Course From Api Done ");
                var orgUnitIds = myOrgUnitInfo.Items
                    .Select(o => new KeyValuePair<int, string>(o.OrgUnit.Id, bookmark))
                    .ToList();

                totalOrgUnitIds.AddRange(orgUnitIds);

                // if has more page, update bookmark, else stop loop
                if (myOrgUnitInfo.PagingInfo.HasMoreItems)
                    bookmark = myOrgUnitInfo.PagingInfo.Bookmark;
                else keepReading = false;
            }

            // update courses detail if there are changes, insert if there are new. no deletion
            var pagedCourseIdsAndBookmarks = Core.Util.SplitList(totalOrgUnitIds, 100);
            Parallel.ForEach(pagedCourseIdsAndBookmarks, page => SyncCourseOnly(page));

            // Sync only courses whose course has not been submitted
            var courseIdNotPreviouslySubmitted = _courseService.GetCourseIdNotPreviouslySubmitted();
            // split courseIds into pages of 100s, then process it
            var pagedCourseIds = Core.Util.SplitList(courseIdNotPreviouslySubmitted, 100);
            Parallel.ForEach(pagedCourseIds, new ParallelOptions { MaxDegreeOfParallelism = _maxDegreeOfParallelism },
                page => SyncEnrollmentOnly(page));

            log.Info("Get roles From Api");
            var roles = GetRoles();
            Save(roles);

            log.Info("Completed Successfully.");
        }

        private void OnlyEnrollmentMode()
        {
            log.Info("Starte with enrollment only.");

            // Sync only courses whose course has not been submitted
            var courseIdWithGradesNotSubmitted = new List<int>() { 6642 }; // _courseService.GetCourseIdWithGradesNotSubmitted();

            // split courseIds into pages of 100s, then process it
            var pagedCourseIds = Core.Util.SplitList(courseIdWithGradesNotSubmitted, 100);
            Parallel.ForEach(pagedCourseIds, new ParallelOptions { MaxDegreeOfParallelism = _maxDegreeOfParallelism },
                page => SyncEnrollmentOnly(page));

            log.Info("Get roles From Api");
            var roles = GetRoles();
            Save(roles);

            log.Info("Completed Successfully.");
        }

        public List<Role> GetRoles()
        {
            log.Info($"Get All Roles From Api.");
            var valenceRoles = _valence.GetRoles();
            var roles = valenceRoles.Select(vr => new Role()
            {
                Id = vr.Identifier,
                Code = vr.Code,
                Name = vr.DisplayName
            }).ToList();
            return roles;
        }

        /// <summary>
        ///     Sync Courses information from last bookmark to get new courses
        ///     Get all CourseIds those gradebook has not been submitted to RO
        ///     Sync course detail and enrollment of these course Ids, except bookmark
        /// </summary>
        private void OnlyOpenCourseWithBookmarkMode()
        {
            log.Info("Valence Sync job on Open Course With Bookmark Mode started");

            PagedResultSetDynamic<MyOrgUnitInfo> myOrgUnitInfo;
            // start from last read bookmark, in case last page is not full 100 records and there are only a few new courses
            var bookmark = _courseService.GetLastBookmark();

            var keepReading = true;
            while (keepReading)
            {
                log.Info($"Get All Course From Api Start. bookmark:{bookmark}");
                myOrgUnitInfo = _valence.GetAllCourses(bookmark);
                var orgUnitIds = myOrgUnitInfo.Items
                    .Select(o => new KeyValuePair<int, string>(o.OrgUnit.Id, bookmark))
                    .ToList();

                log.Info($"Sync Course detail. bookmark:{bookmark}. CourseIds: {string.Join(",", orgUnitIds)}");
                SyncCourseOnly(orgUnitIds);

                // if has more page, update bookmark, else stop loop
                if (myOrgUnitInfo.PagingInfo.HasMoreItems)
                    bookmark = myOrgUnitInfo.PagingInfo.Bookmark;
                else keepReading = false;
            }

            // Sync courses that is not previously submitted
            var courseIdNotPreviouslySubmitted = _courseService.GetCourseIdNotPreviouslySubmitted();
            // split courseIds into pages of 100s, then process it
            var pagedCourseIds = Core.Util.SplitList(courseIdNotPreviouslySubmitted, 100);
            Parallel.ForEach(pagedCourseIds, new ParallelOptions { MaxDegreeOfParallelism = _maxDegreeOfParallelism },
                page =>
                {
                    bookmark = ""; // when book mark is blank, no update to bookmark value
                    var courseWBlankBookmark =
                        page.Select(cid => new KeyValuePair<int, string>(cid, bookmark)).ToList();
                    SyncCourseOnly(courseWBlankBookmark);
                    SyncEnrollmentOnly(page);
                });

            log.Info("Completed Successfully.");
        }

        #region LOCK OBJECT

        // these lock is to allow some form of concurrency while prevent process and insert of same record
        private readonly object _userLock = new object();
        private readonly object _roleLock = new object();
        private readonly object _userEnrollmentLock = new object();
        private readonly object _courseCategoryLock = new object();
        private readonly object _categoryGroupLock = new object();
        private readonly object _userGroupLock = new object();
        private readonly object _sectionLock = new object();
        private readonly object _sectionEnrollmentLock = new object();
        private readonly object _courseLock = new object();
        private readonly object _courseTemplateLock = new object();
        private readonly object _departmentLock = new object();
        private readonly object _semesterLock = new object();

        #endregion

        #region SERVICE

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICategoryGroupService _categoryGroupService;
        private readonly ICourseCategoryService _courseCategoryService;
        private readonly ICourseService _courseService;
        private readonly ICourseTemplateService _courseTemplateService;
        private readonly IDepartmentService _departmentService;
        private readonly IRoleService _roleService;
        private readonly ISemesterService _semesterService;
        private readonly IUserEnrollmentService _userEnrollment;
        private readonly IUserGroupService _userGroupService;
        private readonly IUserService _userService;

        private readonly ValenceApi _valence;

        #endregion

        #region private support function

        public void SyncEnrollment(int userId, int courseId)
        {
            var enrollmentData = _valence.GetEnrollmentFor(userId, courseId);
            if (enrollmentData != null)
            {
                var classListUser = _valence.GetClasslistUser(courseId);
                bool isClassList = false;
                if (classListUser.Where(usr => usr.Identifier == userId.ToString()).Any())
                    isClassList = true;


                // get DB data
                var userEnrollmentRepository = new Repository<UserEnrollment>(_dataContext);
                var dbEnrollmentData = userEnrollmentRepository.Table
                    .Where(enroll => enroll.UserId == userId && enroll.CourseId == courseId)
                    .FirstOrDefault();

                if (dbEnrollmentData == null)
                {
                    // no DB data, insert new one
                    userEnrollmentRepository.Insert(new UserEnrollment()
                    {
                        UserId = userId,
                        CourseId = courseId,
                        RoleId = enrollmentData.RoleId,
                        IsClasslist = isClassList
                    });
                }
                else
                {
                    // check if need to update
                    if (enrollmentData.RoleId != dbEnrollmentData.RoleId || isClassList != dbEnrollmentData.IsClasslist)
                    {
                        // update
                        dbEnrollmentData.RoleId = enrollmentData.RoleId;
                        dbEnrollmentData.IsClasslist = isClassList;
                        userEnrollmentRepository.Update(dbEnrollmentData);
                    }
                }



            }
        }

        /// <summary>
        ///     Sync courses enrollment with D2L and DB
        /// </summary>
        /// <param name="courseIds">Ids of Courses to sync</param>
        public void SyncEnrollmentOnly(List<int> courseIds)
        {
            log.Info($"Sync enrollments. CourseIds: {string.Join(",", courseIds)}.");

            log.Info("Get courseAndUserEnrollment From Api");
            var courseAndUserEnrollment = GetUserEnrollmentDetail(courseIds);

            log.Info("Get course category");
            var courseCategories = GetCategoryByCourseIds(courseIds);

            log.Info("Get category's Group and Student");
            var categoryGroupAndStudents = GetGroupAndStudentByCategories(courseCategories);

            log.Info("Get course sections if there is any");
            var sections = GetSections(courseIds);

            log.Info("ExtractUsers");
            var users = ExtractUsers(courseAndUserEnrollment);

            Save(courseAndUserEnrollment, users, courseCategories, categoryGroupAndStudents, sections);
        }

        /// <summary>
        /// Sync only courses detail with D2L and DB
        /// </summary>
        /// <param name="courseIdsAndBookmarks">Ids of Courses to sync</param>
        private void SyncCourseOnly(List<KeyValuePair<int, string>> courseIdsAndBookmarks)
        {
            var courseIds = courseIdsAndBookmarks.Select(kvp => kvp.Key).ToList();

            log.Info($"Sync courses. CourseIds: {string.Join(",", courseIds)}.");

            // filter out courses that has been submitted
            log.Info("Get GetCoursesDetail From Api ");
            var courseOfferings = GetCoursesDetail(courseIds);

            log.Info("ExtractSemesters");
            var semesters = ExtractSemesters(courseOfferings);

            log.Info("ExtractCourseTemplates");
            var courseTemplates = ExtractCourseTemplates(courseOfferings);

            log.Info("ExtractDepartments");
            var departments = ExtractDepartments(courseOfferings);

            log.Info("ExtractCourses");
            var courses = ExtractCourses(courseOfferings);

            // update bookmarks if it is not blank
            foreach (var course in courses)
            {
                var bookmark = courseIdsAndBookmarks.Where(kvp => kvp.Key == course.Id).Select(kvp => kvp.Value)
                    .FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(bookmark)) course.Bookmark = bookmark;
            }

            Save(courses, courseTemplates, departments, semesters);
        }

        private Dictionary<int, List<SectionData>> GetSections(List<int> orgUnitIds)
        {
            var coursesSectionData = new Dictionary<int, List<SectionData>>();
            foreach (var orgUnitId in orgUnitIds)
            {
                var sectionData = _valence.GetSectionsByCourseId(orgUnitId);
                if (sectionData != null) coursesSectionData.Add(orgUnitId, sectionData);
            }

            return coursesSectionData;
        }

        private List<CourseOffering> GetCoursesDetail(List<int> orgUnitIds)
        {
            var courseOfferings = new List<CourseOffering>();
            foreach (var orgUnitId in orgUnitIds)
            {
                var co = _valence.GetCourceDetailById(orgUnitId);
                courseOfferings.Add(co);
            }

            courseOfferings = courseOfferings.Where(co => co != null).ToList();
            return courseOfferings;
        }

        private Dictionary<int, List<OrgUnitUser>> GetUserEnrollmentDetail(List<int> orgUnitIds)
        {
            var courseAndEnrolledUser = new Dictionary<int, List<OrgUnitUser>>();
            foreach (var orgUnitId in orgUnitIds)
            {
                var enrolledUser = _valence.GetEnrolledUser(orgUnitId);
                var classListUser = _valence.GetClasslistUser(orgUnitId);
                var classListUserIds = new List<string>();
                if (classListUser != null)
                    classListUserIds = classListUser.Select(u => u.Identifier).ToList();

                foreach (var user in enrolledUser)
                    if (classListUserIds.Contains(user.User.Identifier))
                        user.User.IsClasslist = true;
                    else
                        user.User.IsClasslist = false;

                courseAndEnrolledUser.Add(orgUnitId, enrolledUser);
            }

            return courseAndEnrolledUser;
        }

        private Dictionary<int, List<GroupData>> GetGroupAndStudentByCategories(
            Dictionary<int, List<GroupCategoryData>> courseCategories)
        {
            var groupAndStudentsInCategories = new Dictionary<int, List<GroupData>>();

            foreach (var categories in courseCategories)
            {
                var courseId = categories.Key;
                if (categories.Value != null)
                    foreach (var categoriesData in categories.Value)
                    {
                        var groupAndStudentsInCategory =
                            _valence.GetGroupAndStudentEnrollmentsByCategoryId(courseId,
                                categoriesData.GroupCategoryId);
                        groupAndStudentsInCategories.Add(categoriesData.GroupCategoryId, groupAndStudentsInCategory);
                    }
            }

            return groupAndStudentsInCategories;
        }

        private Dictionary<int, List<GroupCategoryData>> GetCategoryByCourseIds(List<int> orgUnitIds)
        {
            var courseAndCategories = new Dictionary<int, List<GroupCategoryData>>();
            foreach (var orgUnitId in orgUnitIds)
            {
                var groupCategoryData = _valence.GetCategory(orgUnitId);
                courseAndCategories.Add(orgUnitId, groupCategoryData);
            }

            return courseAndCategories;
        }

        private List<Course> ExtractCourses(List<CourseOffering> courseOfferings)
        {
            var sqlMinDateTime = (DateTime)SqlDateTime.MinValue;

            return courseOfferings
                .DistinctBy(co => co.Identifier)
                .Select(co => new Course
                {
                    Id = Convert.ToInt32(co.Identifier),
                    Name = co.Name,
                    Code = co.Code,
                    IsActive = co.IsActive,
                    Path = co.Path,
                    StartDate = co.StartDate >= sqlMinDateTime ? co.StartDate : sqlMinDateTime,
                    EndDate = co.EndDate >= sqlMinDateTime ? co.EndDate : sqlMinDateTime,
                    CourseTemplateId = Convert.ToInt32(co.CourseTemplate?.Identifier ?? "0"),
                    SemesterId = Convert.ToInt32(co.Semester?.Identifier ?? "0"),
                    DepartmentId = Convert.ToInt32(co.Department?.Identifier ?? "0")
                })
                .ToList();
        }

        private List<Semester> ExtractSemesters(List<CourseOffering> courseOfferings)
        {
            return courseOfferings
                .Where(co => co.Semester != null)
                .Select(co => co.Semester)
                .GroupBy(ct => ct.Identifier)
                .Select(g => g.First())
                .Select(s => new Semester
                {
                    Id = Convert.ToInt32(s.Identifier),
                    Name = s.Name ?? "",
                    Code = s.Code ?? ""
                })
                .ToList();
        }

        private List<Department> ExtractDepartments(List<CourseOffering> courseOfferings)
        {
            return courseOfferings
                .Where(co => co.Department != null)
                .Select(co => co.Department)
                .GroupBy(ct => ct.Identifier)
                .Select(g => g.First())
                .Select(d => new Department
                {
                    Id = Convert.ToInt32(d.Identifier),
                    Name = d.Name ?? "",
                    Code = d.Code ?? ""
                })
                .ToList();
        }

        public List<CourseTemplate> ExtractCourseTemplates(List<CourseOffering> courseOfferings)
        {
            return courseOfferings
                .Where(co => co.CourseTemplate != null)
                .Select(co => co.CourseTemplate)
                .GroupBy(ct => ct.Identifier)
                .Select(g => g.First())
                .Select(ct => new CourseTemplate
                {
                    Id = Convert.ToInt32(ct.Identifier),
                    Name = ct.Name ?? "",
                    Code = ct.Code ?? ""
                })
                .ToList();
        }

        private List<User> ExtractUsers(Dictionary<int, List<OrgUnitUser>> courseAndUserEnrollment)
        {
            var users = courseAndUserEnrollment
                .SelectMany(e => e.Value.Select(ue => ue.User))
                .GroupBy(u => u.Identifier)
                .Select(g => g.First())
                .Select(ue => new User
                {
                    Id = Convert.ToInt32(ue.Identifier),
                    DisplayName = ue.DisplayName,
                    EmailAddress = ue.EmailAddress,
                    OrgDefinedId = ue.OrgDefinedId,
                    ProfileBadgeUrl = ue.ProfileBadgeUrl,
                    ProfileIdentifier = ue.ProfileIdentifier
                })
                .DistinctBy(usr => usr.Id)
                .ToList();

            return users;
        }

        /// <summary>
        ///     This functions must be threadsafe
        /// </summary>
        /// <param name="courses"></param>
        /// <param name="courseTemplates"></param>
        /// <param name="departments"></param>
        /// <param name="semesters"></param>
        private void Save(List<Course> courses, List<CourseTemplate> courseTemplates,
            List<Department> departments, List<Semester> semesters)
        {

            using var scope = _scopeFactory.CreateScope();
            var dbctx = scope.ServiceProvider.GetRequiredService<DataContext>();
            var isisContext = scope.ServiceProvider.GetRequiredService<LMSIsisContext>();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var serviceProvider = scope.ServiceProvider;
            var courseRepository = new Repository<Course>(dbctx);
            var userEnrollmentRepository = new Repository<UserEnrollment>(dbctx);
            var roleRepository = new Repository<Role>(dbctx);
            var permissionRoleRepository = new Repository<PermissionRole>(dbctx);
            var permissionRepository = new Repository<Permission>(dbctx);
            var semesterRepository = new Repository<Semester>(dbctx);
            var meetingRepository = new Repository<Meeting>(dbctx);
            var gradeResetStatusRepository = new Repository<GradeResetStatus>(dbctx);
            var courseTemplateRepository = new Repository<CourseTemplate>(dbctx);
            var departmentRepository = new Repository<Department>(dbctx);
            var mergeSectionRepo = new Repository<MergeSection>(dbctx);
            var gradeSubmissionsRepository = new Repository<GradeSubmissions>(dbctx);
            var peerFeedbackSessionsRepository = new Repository<PeerFeedbackSessions>(dbctx);
            var userRepository = new Repository<User>(dbctx);

            var courseService = new CourseService(serviceProvider, isisContext, dbctx, configuration);
            var courseTemplateService = new CourseTemplateService(serviceProvider);
            var departmentService = new DepartmentService(serviceProvider);
            var semesterService =
                new SemesterService(serviceProvider);

            lock (_courseLock)
            {
                log.Info("insert-update Course Course");
                courseService.Save(courses); // shared across multiple courses, cannot delete
            }

            lock (_courseTemplateLock)
            {
                log.Info("insert-update Course Template");
                courseTemplateService.Save(courseTemplates); // shared across multiple courses, cannot delete
            }

            lock (_departmentLock)
            {
                log.Info("insert-update Course Department");
                departmentService.Save(departments); // shared across multiple courses, cannot delete
            }

            lock (_semesterLock)
            {
                log.Info("insert-update Course Semester");
                semesterService.Save(semesters); // shared across multiple courses, cannot delete
            }

        }

        /// <summary>
        ///     This functions must be threadsafe
        /// </summary>
        /// <param name="courseAndUserEnrollment"></param>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <param name="courseCategoryData"></param>
        /// <param name="categoryGroupAndStudents"></param>
        /// <param name="courseSections"></param>
        private void Save(
            Dictionary<int, List<OrgUnitUser>> courseAndUserEnrollment,
            List<User> user,
            Dictionary<int, List<GroupCategoryData>> courseCategoryData,
            Dictionary<int, List<GroupData>> categoryGroupAndStudents,
            Dictionary<int, List<SectionData>> courseSections)
        {
            // cannot use DI for threading
            using var scope = _scopeFactory.CreateScope();
            var dbctx = scope.ServiceProvider.GetRequiredService<DataContext>();
            var isisContext = scope.ServiceProvider.GetRequiredService<LMSIsisContext>();
            var daoFactory = scope.ServiceProvider.GetRequiredService<IDaoFactory>();
            var cacheManager = scope.ServiceProvider.GetRequiredService<ICacheManager>();
            var courseService = scope.ServiceProvider.GetRequiredService<ICourseService>();
            var serviceProvider = scope.ServiceProvider;
            var userRepository = new Repository<User>(dbctx);
            var userEnrollmentRepository = new Repository<UserEnrollment>(dbctx);
            var roleRepository = new Repository<Role>(dbctx);
            var permissionRoleRepository = new Repository<PermissionRole>(dbctx);
            var permissionRepository = new Repository<Permission>(dbctx);
            var categoryGroupRepository = new Repository<CategoryGroup>(dbctx);
            var userGroupRepository = new Repository<UserGroup>(dbctx);
            var courseCategoryRepository = new Repository<CourseCategory>(dbctx);
            var d2LLoginLogRepository = new Repository<D2LLoginLog>(dbctx);
            var courseRepository = new Repository<Course>(dbctx);
            var gradeResetStatusRepository = new Repository<GradeResetStatus>(dbctx);

            var audit = new AuditLogService(daoFactory);
            var gptDebugLogService = new GPTDebugLogService(daoFactory);
            var gptAuditLogService = new GPTAuditLogService(daoFactory);
            var batchJobLogService = new BatchJobLogService(daoFactory);
            var batchJobLogDetailService = new BatchJobLogDetailService(daoFactory);
            var errorLogService = new ErrorLogService(daoFactory);
            var toolAccessLogService = new ToolAccessLogService(daoFactory);
            var loggingService = new LoggingService(audit, gptDebugLogService, gptAuditLogService, batchJobLogService, batchJobLogDetailService, errorLogService, daoFactory, toolAccessLogService);
            var userService = new UserService(dbctx, serviceProvider, daoFactory, isisContext, loggingService);
            var roleService = new RoleService(serviceProvider);
            var userEnrollmentService = new UserEnrollmentService(dbctx, daoFactory, serviceProvider);
            var courseCategoryService = new CourseCategoryService(dbctx, serviceProvider);
            var categoryGroupService = new CategoryGroupService(dbctx, serviceProvider, cacheManager, daoFactory, courseService);
            var userGroupService = new UserGroupService(dbctx, serviceProvider);

            lock (_userLock)
            {
                var usersTokens = Core.Util.SplitList(user, 500);
                foreach (var userToken in usersTokens)
                {
                    log.Info($"insert-update users {userToken?.Count}/{user?.Count}");
                    userService.Save(userToken); // shared across multiple courses, cannot delete
                }
            }

            lock (_userEnrollmentLock)
            {
                log.Info("insert-update-delete User Enrollment");
                // flatten user enrollment
                var userenrollments = courseAndUserEnrollment.SelectMany(dict => dict.Value.Select(l =>
                    new UserEnrollment
                    {
                        CourseId = dict.Key,
                        UserId = Convert.ToInt32(l.User.Identifier),
                        RoleId = l.Role.Id,
                        IsClasslist = l.User.IsClasslist
                    })).DistinctBy(ue => new { ue.CourseId, ue.UserId })
                    .ToList();

                foreach (var userEnrollmentForOneCourse in courseAndUserEnrollment)
                {
                    var enrollments = userEnrollmentForOneCourse.Value.Select(u =>
                    new UserEnrollment
                    {
                        CourseId = userEnrollmentForOneCourse.Key,
                        UserId = Convert.ToInt32(u.User.Identifier),
                        RoleId = u.Role.Id,
                        IsClasslist = u.User.IsClasslist
                    })
                    .ToList();

                    if (enrollments?.Count > _userEnrollmentThreshold)
                    {
                        log.Info($"skipping bc over threshold - User Enrollment {enrollments?.Count} enrollments for {userEnrollmentForOneCourse.Key}");
                    }
                    else
                    {
                        log.Info($"insert-update-delete User Enrollment {enrollments?.Count} enrollments for {userEnrollmentForOneCourse.Key}");
                        userEnrollmentService.Save(enrollments);
                    }
                }
            }

            lock (_courseCategoryLock)
            {
                log.Info("insert-update-delete Course Category");
                var courseCategories = courseCategoryData.Where(dict => dict.Value?.Count > 0)
                    .SelectMany(dict => dict.Value?.Select(gcd => new CourseCategory
                    {
                        Id = gcd.GroupCategoryId,
                        EnrollmentStyle = gcd.EnrollmentStyle,
                        EnrollmentQuantity = gcd.EnrollmentQuantity,
                        AutoEnroll = gcd.AutoEnroll,
                        RandomizeEnrollments = gcd.RandomizeEnrollments,
                        Name = gcd.Name,
                        TextDescription = gcd.Description.Text.Truncate(100),
                        HtmlDescription = gcd.Description.Html.Truncate(100),
                        MaxUsersPerGroup = gcd.MaxUsersPerGroup,
                        CourseId = dict.Key
                    })).ToList();
                courseCategoryService.Save(courseCategories);
            }

            lock (_categoryGroupLock)
            {
                log.Info("insert-update-delete Category Groups");
                var categoryGroups = categoryGroupAndStudents.SelectMany(dict => dict.Value.Where(v => v != null)
                    .Select(c => new CategoryGroup
                    {
                        Id = c.GroupId,
                        Name = c.Name.Truncate(100),
                        TextDescription = c.Description.Text.Truncate(100),
                        HtmlDescription = c.Description.Html.Truncate(100),
                        CourseCategoryId = dict.Key
                    })).ToList();
                categoryGroupService.Save(categoryGroups);
            }

            lock (_userGroupLock)
            {
                log.Info("insert-update-delete User Groups");
                var userGroups = categoryGroupAndStudents.SelectMany(dict => dict.Value.Where(v => v != null)
                    .SelectMany(grpData => grpData.Enrollments.Select(uid => new UserGroup
                    {
                        CategoryGroupId = grpData.GroupId,
                        UserId = uid
                    }))).ToList();
                userGroupService.Save(userGroups);
            }

        }


        private void Save(List<Role> roles)
        {
            // cannot use DI for threading
            using var scope = _scopeFactory.CreateScope();
            var dbctx = scope.ServiceProvider.GetRequiredService<DataContext>();
            var serviceProvider = scope.ServiceProvider;
            var roleRepository = new Repository<Role>(dbctx);
            var userEnrollmentRepository = new Repository<UserEnrollment>(dbctx);
            var roleService = new RoleService(serviceProvider);

            lock (_roleLock)
            {
                log.Info($"insert-update Roles {roles?.Count}");
                roleService.Save(roles); // shared across multiple courses, cannot delete
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="courseId"></param>
        /// <returns></returns>
        public List<Entity.Valence.Section> GetSectionByCourseId(int courseId)
        {
            return _valence.GetSectionByCourseId(courseId);
        }
        #endregion
    }
}