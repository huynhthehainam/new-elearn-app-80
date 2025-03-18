#region USING

using System.Collections.Generic;
using System.Linq;
using eLearnApps.Business.Interface;
using eLearnApps.Data;
using eLearnApps.Entity.LmsTools;
using eLearnApps.Entity.LmsIsis;
using System;
using eLearnApps.ViewModel.ISIS;
using eLearnApps.Core;
using eLearnApps.Core.Cryptography;
using System.Configuration;
using System.Data.Entity;
using eLearnApps.ViewModel.WidgetLTI;
using eLearnApps.ViewModel.Common;
using eLearnApps.Entity.Security;
using System.Data.SqlClient;
using log4net;
using System.Reflection;
using System.Threading.Tasks;
using eLearnApps.Entity.LmsIsis.Dto;
using eLearnApps.ViewModel.KendoUI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace eLearnApps.Business
{
    public class CourseService : ICourseService
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IRepository<Course> _courseRepository;
        private readonly IRepository<Semester> _semesterRepository;
        private readonly IRepository<UserEnrollment> _userEnrollmentRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<PermissionRole> _permissionRoleRepository;
        private readonly IRepository<Permission> _permissionRepository;
        private readonly IRepository<Meeting> _meetingRepository;
        private readonly IRepository<GradeResetStatus> _gradeResetStatusRepository;
        private readonly IRepository<GradeSubmissions> _gradeSubmissionRepository;
        private readonly IRepository<MergeSection> _mergeSectionRepository;
        private readonly IRepository<TL_CourseOfferings> _tlCourseOfferingRepo;

        private readonly IRepository<PS_SIS_LMS_SCHED_V> _psSisLmsScheduleRepository;
        private readonly IRepository<PeerFeedbackSessions> _peerFeedbackSessionsRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IConfiguration _configuration;

        private readonly DataContext _dataContext;
        private readonly LMSIsisContext _isisContext;
        public CourseService(
          IServiceProvider serviceProvider,
            LMSIsisContext isisContext,
            DataContext dataContext,
            IConfiguration configuration
            )
        {
            _configuration = configuration;
            _courseRepository = serviceProvider.GetRequiredKeyedService<IRepository<Course>>("default");
            _userEnrollmentRepository = serviceProvider.GetRequiredKeyedService<IRepository<UserEnrollment>>("default");
            _roleRepository = serviceProvider.GetRequiredKeyedService<IRepository<Role>>("default");
            _permissionRoleRepository = serviceProvider.GetRequiredKeyedService<IRepository<PermissionRole>>("default");
            _permissionRepository = serviceProvider.GetRequiredKeyedService<IRepository<Permission>>("default");
            _semesterRepository = serviceProvider.GetRequiredKeyedService<IRepository<Semester>>("default");
            _meetingRepository = serviceProvider.GetRequiredKeyedService<IRepository<Meeting>>("default");
            _gradeResetStatusRepository = serviceProvider.GetRequiredKeyedService<IRepository<GradeResetStatus>>("default");
            _gradeSubmissionRepository = serviceProvider.GetRequiredKeyedService<IRepository<GradeSubmissions>>("default");
            _dataContext = dataContext;
            _isisContext = isisContext;
            _tlCourseOfferingRepo = new Repository<TL_CourseOfferings>(isisContext);
            _mergeSectionRepository = new Repository<MergeSection>(isisContext);
            _psSisLmsScheduleRepository = new Repository<PS_SIS_LMS_SCHED_V>(isisContext);
            _peerFeedbackSessionsRepository = serviceProvider.GetRequiredKeyedService<IRepository<PeerFeedbackSessions>>("default");
            _userRepository = serviceProvider.GetRequiredKeyedService<IRepository<User>>("default");
        }

        #region Interface
        public Course GetById(int id)
        {
            return _courseRepository.GetById(id);
        }

        public void Insert(Course course)
        {
            _courseRepository.Insert(course);
        }

        public void Update(Course course)
        {
            _courseRepository.Update(course);
        }

        public void Delete(Course course)
        {
            _courseRepository.Delete(course);
        }

        public void Insert(List<Course> courses)
        {
            _courseRepository.Insert(courses);
        }

        public List<Course> GetBySemesterId(int userId, int semesterId)
        {
            var course = _courseRepository.Table;
            var enroll = _userEnrollmentRepository.Table;
            var semester = _semesterRepository.Table;

            var query = from c in course
                        join s in semester on c.SemesterId equals s.Id
                        join e in enroll on c.Id equals e.CourseId
                        where e.UserId == userId && c.SemesterId == semesterId
                        select c;

            return query.Distinct().ToList();
        }

        public List<TreeViewItem> GetTermAndCourse(int userId, int courseId)
        {
            var query = from c in _courseRepository.TableNoTracking
                        join s in _semesterRepository.TableNoTracking on c.SemesterId equals s.Id
                        join e in _userEnrollmentRepository.TableNoTracking on c.Id equals e.CourseId
                        where e.UserId == userId
                        select new { Semester = s, Course = c };

            var semesters = query.GroupBy(grp => grp.Semester);
            var result = new List<TreeViewItem>();
            foreach (var semester in semesters)
            {
                var key = semester.Key;
                var values = semester.Select(s => s.Course).DistinctBy(c => c.Id).ToList();

                if (values.Count > 0)
                {
                    bool expanded = false;
                    if (values.Any(c => c.Id == courseId)) expanded = true;
                    var tvi = new TreeViewItem()
                    {
                        id = semester.Key.Id,
                        text = semester.Key.Name,
                        expanded = expanded,
                        hasChildren = true,
                        items = values.OrderBy(course => course.Name).Select(c => new NodeItem()
                        {
                            id = c.Id,
                            text = c.Name
                        }).ToList()
                    };

                    result.Add(tvi);
                }
            }

            result = result.OrderByDescending(item => item.id).ToList();

            return result;
        }

        public async Task<List<Course>> GetBySemesterIdAsync(int userId, int semesterId)
        {
            var course = _courseRepository.Table;
            var enroll = _userEnrollmentRepository.Table;
            var semester = _semesterRepository.Table;

            var query = from c in course
                        join s in semester on c.SemesterId equals s.Id
                        join e in enroll on c.Id equals e.CourseId
                        where e.UserId == userId && c.SemesterId == semesterId
                        select c;

            return await query.Distinct().ToListAsync();
        }
        public List<Course> GetByInstructor(int userId, int semesterId)
        {
            var course = _courseRepository.Table;
            var enroll = _userEnrollmentRepository.Table;
            var semester = _semesterRepository.Table;

            var query = from c in course
                        join s in semester on c.SemesterId equals s.Id
                        join e in enroll on c.Id equals e.CourseId
                        join r in _roleRepository.TableNoTracking on e.RoleId equals r.Id
                        where e.UserId == userId && c.SemesterId == semesterId && r.Name == Core.RoleName.INSTRUCTOR
                        select c;

            return query.Distinct().OrderBy(x => x.Name).ToList();
        }

        public List<SemesterViewModel> GetCourseByUserAndRole(int userId, string roleName)
        {
            var course = _courseRepository.Table;
            var enroll = _userEnrollmentRepository.Table;
            var semesterRepo = _semesterRepository.Table;

            var query = from c in course
                        join s in semesterRepo on c.SemesterId equals s.Id
                        join e in enroll on c.Id equals e.CourseId
                        join r in _roleRepository.TableNoTracking on e.RoleId equals r.Id
                        where e.UserId == userId && r.Name == roleName
                        select new { Semester = s, Course = c };

            var semesterVMs = new List<SemesterViewModel>();
            var coursesGroupBySemester = query.GroupBy(c => c.Semester).OrderByDescending(grp => grp.Key.Name);
            foreach (var semester in coursesGroupBySemester)
            {
                var semesterVM = new SemesterViewModel(semester.Key.Id, semester.Key.Name, semester.Key.Code);
                semesterVM.Courses = semester.Select(c => new CourseViewModel()
                {
                    Id = c.Course.Id,
                    Name = c.Course.Name,
                    Code = c.Course.Code,
                    SectionName = string.Empty,
                    ACAD_CAREER = string.Empty,
                    CLASS_NBR = null,
                    STRM = string.Empty,
                    StartDate = c.Course.StartDate,
                    EndDate = c.Course.EndDate,
                    SemesterId = c.Semester.Id,
                    DepartmentId = null,
                    IsMerge = false,
                    IsIsis = false,
                    CourseDisplayName = string.Empty
                })
                    .ToList();
                semesterVMs.Add(semesterVM);
            }

            return semesterVMs;
        }

        public List<Course> GetByStudentId(int studentId)
        {
            var course = _courseRepository.Table;
            var enroll = _userEnrollmentRepository.Table;

            var query = course
                .Join(enroll,
                    m => m.Id,
                    v => v.CourseId,
                    (m, v) => new { m, v })
                .Where(x => (x.m.Id == x.v.CourseId) & (x.v.UserId == studentId))
                .Select(x => x.m);

            return query.ToList();
        }

        public void Save(List<Course> courses)
        {
            var itemToCompareQuery = from nc in courses
                                     join cr in _courseRepository.TableNoTracking on nc.Id equals cr.Id into dbCourses
                                     from c in dbCourses.DefaultIfEmpty()
                                     select new { incoming = nc, existing = c };

            var itemsToCompare = itemToCompareQuery.ToList();
            var newItem = new List<Course>();
            var itemToUpdate = new List<Course>();
            foreach (var item in itemsToCompare)
            {
                if (item.existing == null) newItem.Add(item.incoming);
                else
                {
                    if (item.existing.Name != item.incoming.Name || item.existing.Code != item.incoming.Code ||
                        item.existing.IsActive != item.incoming.IsActive || item.existing.Path != item.incoming.Path ||
                        item.existing.CourseTemplateId != item.incoming.CourseTemplateId || item.existing.SemesterId != item.incoming.SemesterId ||
                        item.existing.DepartmentId != item.incoming.DepartmentId || item.existing.Bookmark != item.incoming.Bookmark)
                    {
                        var toupdate = item.existing;
                        toupdate.Name = item.incoming.Name;
                        toupdate.Code = item.incoming.Code;
                        toupdate.IsActive = item.incoming.IsActive;
                        toupdate.Path = item.incoming.Path;
                        toupdate.CourseTemplateId = item.incoming.CourseTemplateId;
                        toupdate.SemesterId = item.incoming.SemesterId;
                        toupdate.DepartmentId = item.incoming.DepartmentId;
                        // dont save if blank
                        if (!string.IsNullOrWhiteSpace(item.incoming.Bookmark)) toupdate.Bookmark = item.incoming.Bookmark;
                        itemToUpdate.Add(toupdate);
                    }
                }
            }

            if (newItem.Count > 0) _courseRepository.Insert(newItem);
            if (itemToUpdate.Count > 0) _courseRepository.Update(itemToUpdate);
        }

        public Course GetById(int id, int userId)
        {
            var course = _courseRepository.Table;
            var enroll = _userEnrollmentRepository.Table;
            var semester = _semesterRepository.Table;

            var query = from c in course
                        join s in semester on c.SemesterId equals s.Id
                        join e in enroll on c.Id equals e.CourseId
                        where e.UserId == userId && c.Id == id
                        select c;

            return query.FirstOrDefault();
        }

        public List<Course> GetAll()
        {
            return _courseRepository.Table.ToList();
        }

        public List<int> GetAllCourseId()
        {
            return _courseRepository.Table.Select(x => x.Id).ToList();
        }

        public void CopyCourseEnrollmentToEnrolledUserMeeting(int semesterId)
        {

            var isisClassSchedules = new Repository<PS_SIS_LMS_SCHED_V>(_isisContext).Table;
            var isisCourseOfferingsRepo = new Repository<TL_CourseOfferings>(_isisContext).Table;
            // some record has CLASS_NBR null, since we will be using this value as part of complex FK, this value cannot be null in our join
            var isisCourseOfferings = from cos in isisCourseOfferingsRepo
                                      where cos.CLASS_NBR.HasValue
                                      select cos;

            var coursesTable = new Repository<Course>(_dataContext).Table;
            var enrollments = new Repository<UserEnrollment>(_dataContext).Table;
            var meetingRepo = new Repository<Meeting>(_dataContext);
            var meetingAttendeeRepo = new Repository<MeetingAttendee>(_dataContext);

            var courses = coursesTable.Where(ct => ct.SemesterId == semesterId).ToList();

            // extract all courses enrollment on current semester
            var courseDetails = (from c in courses
                                 join co in isisCourseOfferings on c.Code equals co.CourseOfferingCode
                                 select new { CourseId = c.Id, CourseName = c.Name, c.Code, co.STRM, co.CLASS_NBR, StartDate = co.START_DATE.Value, EndDate = co.END_DATE.Value });

            // get course detail + enrollment
            var courseEnrollment = (from cd in courseDetails
                                    join e in enrollments on cd.CourseId equals e.CourseId into courseEnrollments
                                    select new
                                    {
                                        CourseId = cd.CourseId,
                                        CourseName = cd.CourseName,
                                        STRM = cd.STRM,
                                        CLASS_NBR = cd.CLASS_NBR,
                                        StartDate = cd.StartDate,
                                        EndDate = cd.EndDate,
                                        Enrollments = courseEnrollments.ToList()
                                    })
                                    .ToList();

            // extract schedules of current course
            var courseUserAndSchedules = (from ce in courseEnrollment
                                          join cs in isisClassSchedules on new { Strm = ce.STRM, ClassNbr = Convert.ToInt32(ce.CLASS_NBR.Value) } equals new { Strm = cs.STRM, ClassNbr = cs.CLASS_NBR }
                                          select new UserISISClassSchedule
                                          {
                                              CourseId = ce.CourseId,
                                              CourseName = ce.CourseName,
                                              StartDate = ce.StartDate,
                                              EndDate = ce.EndDate,
                                              Enrollments = ce.Enrollments,
                                              Schedules = cs
                                          })
                                          .ToList();

            var adminOrgUnitId = Convert.ToInt32(_configuration.GetValue<string>("AdminOrgUnitId"));
            // find if any meeting with this param has been created. we skip if any has been created
            var courseNames = courseUserAndSchedules.Select(cus => cus.CourseName).ToList();
            var meetingCreated = meetingRepo.Table.Where(m => courseNames.Contains(m.Course) && m.OwnerId == adminOrgUnitId).Count();
            if (meetingCreated == 0 && courseNames.Count > 0)
            {
                var mondayClasses = courseUserAndSchedules.Where(cus => cus.Schedules.MON.ToLower() == "y").ToList();
                var tuesdayClasses = courseUserAndSchedules.Where(us => us.Schedules.TUES.ToLower() == "y").ToList();
                var wednesdayClasses = courseUserAndSchedules.Where(us => us.Schedules.WED.ToLower() == "y").ToList();
                var thursdayClasses = courseUserAndSchedules.Where(us => us.Schedules.THURS.ToLower() == "y").ToList();
                var fridayClasses = courseUserAndSchedules.Where(us => us.Schedules.FRI.ToLower() == "y").ToList();
                var saturdayClasses = courseUserAndSchedules.Where(us => us.Schedules.SAT.ToLower() == "y").ToList();
                var sundayClasses = courseUserAndSchedules.Where(us => us.Schedules.SUN.ToLower() == "y").ToList();

                // cycle through each day from term start to course end date
                // NOTE: we must assume all course in assigned term have the same start and end date.
                // NOTE: courses start and end = term start and end
                var startDate = courseUserAndSchedules.Min(cus => cus.StartDate);
                var endDate = courseUserAndSchedules.Max(cus => cus.EndDate);

                // turn schedule into class.
                for (var dt = startDate; dt <= endDate; dt = dt.AddDays(1))
                {
                    // for each day, check if there is any class scheduled
                    switch (dt.DayOfWeek)
                    {
                        case DayOfWeek.Monday:
                            createClassInMeeting(meetingRepo, meetingAttendeeRepo, dt, mondayClasses, adminOrgUnitId);
                            break;
                        case DayOfWeek.Tuesday:
                            createClassInMeeting(meetingRepo, meetingAttendeeRepo, dt, tuesdayClasses, adminOrgUnitId);
                            break;
                        case DayOfWeek.Wednesday:
                            createClassInMeeting(meetingRepo, meetingAttendeeRepo, dt, wednesdayClasses, adminOrgUnitId);
                            break;
                        case DayOfWeek.Thursday:
                            createClassInMeeting(meetingRepo, meetingAttendeeRepo, dt, thursdayClasses, adminOrgUnitId);
                            break;
                        case DayOfWeek.Friday:
                            createClassInMeeting(meetingRepo, meetingAttendeeRepo, dt, fridayClasses, adminOrgUnitId);
                            break;
                        case DayOfWeek.Saturday:
                            createClassInMeeting(meetingRepo, meetingAttendeeRepo, dt, saturdayClasses, adminOrgUnitId);
                            break;
                        case DayOfWeek.Sunday:
                            createClassInMeeting(meetingRepo, meetingAttendeeRepo, dt, sundayClasses, adminOrgUnitId);
                            break;
                    }
                }
            }
        }


        public List<eLearnApps.Entity.LmsTools.Dto.CourseSchedulesInSemester> GetUserEnrolledCourses(int userId, int courseId)
        {

            var isisClassSchedules = new Repository<PS_SIS_LMS_SCHED_V>(_isisContext).Table;
            var isisCourseOfferingsRepo = new Repository<TL_CourseOfferings>(_isisContext).Table;
            // some record has CLASS_NBR null, since we will be using this value as part of complex FK, this value cannot be null in our join
            var isisCourseOfferings = from cos in isisCourseOfferingsRepo
                                      where cos.CLASS_NBR.HasValue
                                      select cos;

            var semester = new Repository<Semester>(_dataContext).Table;
            var coursesTable = new Repository<Course>(_dataContext).Table;
            var enrollmentTable = new Repository<UserEnrollment>(_dataContext).Table;

            // get enrolled courses
            var coursesQuery = from c in coursesTable
                               join e in enrollmentTable on c.Id equals e.CourseId
                               join s in semester on c.SemesterId.Value equals s.Id
                               where c.StartDate <= DateTime.Now && c.EndDate >= DateTime.Now && e.UserId == userId
                                 && (courseId == 0 ? true : c.Id == courseId)
                               select new { Term = s, Course = c };
            var courses = coursesQuery.ToList();

            // extract all courses enrollment on current semester
            var courseDetails = (from c in courses
                                 join co in isisCourseOfferings on c.Course.Code equals co.CourseOfferingCode
                                 join csch in isisClassSchedules on new { Strm = co.STRM, ClassNbr = Convert.ToInt32(co.CLASS_NBR.Value) } equals new { Strm = csch.STRM, ClassNbr = csch.CLASS_NBR }
                                 select new eLearnApps.Entity.LmsTools.Dto.CourseSchedulesInSemester() { Semester = c.Term, Course = c.Course, Schedule = csch });

            return courseDetails.ToList();

        }

        public List<int> GetCourseIdPreviouslySubmitted()
        {
            var courseIdWithGradesSubmitted = _gradeSubmissionRepository.Table
                .Select(grs => grs.CourseId).Distinct()
                .ToList();

            return courseIdWithGradesSubmitted;
        }

        public List<int> GetCourseIdNotPreviouslySubmitted()
        {
            var courseIdSubmittedPreviously = GetCourseIdPreviouslySubmitted();
            var courseIdNotPreviouslySubmitted = _courseRepository.Table
                .Where(c => !courseIdSubmittedPreviously.Contains(c.Id))
                .Select(c => c.Id)
                .ToList();

            return courseIdNotPreviouslySubmitted;
        }

        /// <summary>
        /// Get bookmark value of last courses acquired from D2L 
        /// </summary>
        /// <returns>null if no course found, else bookmark value of last record</returns>
        public string GetLastBookmark()
        {
            var lastBookmark = _courseRepository.Table
                .OrderByDescending(c => c.Id)
                .Select(c => c.Bookmark)
                .FirstOrDefault();

            if (lastBookmark == null) return null;
            else return lastBookmark;
        }

        public TL_CourseOfferings FindISISCourseOffering(string courseOfferingCode)
        {
            return _tlCourseOfferingRepo.Table.Where(co => co.CourseOfferingCode == courseOfferingCode).FirstOrDefault();
        }

        public List<TL_CourseOfferings> FindISISCourseOffering(List<string> courseOfferingCodes)
        {
            return _tlCourseOfferingRepo.TableNoTracking
                .Where(co => courseOfferingCodes.Contains(co.CourseOfferingCode))
                .DistinctBy(co => co.CourseOfferingCode)
                .ToList();
        }

        public Course GetCoursebyCode(string courseOfferingCode)
        {
            return _courseRepository.Table
                .Where(c => c.Code.Trim().ToLower() == courseOfferingCode.Trim().ToLower())
                .OrderByDescending(c => c.StartDate)
                .FirstOrDefault();
        }

        public IEnumerable<ClassScheduleDto> GetClassSchedule(string userEmail, int orgUnitId = 0)
        {
            bool isStudent;
            if (userEmail.EndsWith("@smu.edu.sg")) isStudent = false;
            else isStudent = true;

            string sqlQuery;
            if (isStudent) // student query
                sqlQuery = @"
                    SELECT DISTINCT PS_SIS_LMS_STD_VW.EMAIL_ADDR, TL_CourseOfferings.START_DATE, 
                    TL_CourseOfferings.END_DATE, TL_CourseOfferings.CourseOfferingCode, 
                    TL_CourseOfferings.ACADEMIC_YEAR, TL_CourseOfferings.ACADEMIC_TERM, 
                    TL_CourseList.CourseCode + '-' + TL_CourseList.CourseName + '-' + TL_CourseOfferings.SECTION_ID CourseName, 
                    PS_SIS_LMS_SCHED_V.*
                    FROM PS_SIS_LMS_STD_VW, 
                    PS_SIS_LMS_SCHED_V, 
                    TL_CourseOfferings, 
                    PS_SIS_LMS_ENRL_VW, 
                    TL_CourseList
                    WHERE PS_SIS_LMS_STD_VW.EMPLID = PS_SIS_LMS_ENRL_VW.EMPLID
                    AND PS_SIS_LMS_ENRL_VW.STDNT_ENRL_STATUS = 'E'
                    AND PS_SIS_LMS_ENRL_VW.STRM = TL_CourseOfferings.STRM
                    AND PS_SIS_LMS_ENRL_VW.CLASS_NBR = TL_CourseOfferings.CLASS_NBR
                    AND TL_CourseOfferings.STRM = PS_SIS_LMS_SCHED_V.STRM
                    AND TL_CourseOfferings.CLASS_NBR = PS_SIS_LMS_SCHED_V.CLASS_NBR
                    AND TL_CourseOfferings.START_DATE <= SYSDATETIME()
                    AND TL_CourseOfferings.END_DATE >= SYSDATETIME()
                    AND TL_CourseList.STRM = TL_CourseOfferings.STRM
                    AND TL_CourseList.CourseCode = TL_CourseOfferings.COURSE_ID
                    AND EMAIL_ADDR = @UserEmail";
            else // Staff/faculty Query
                sqlQuery = @"
                    SELECT SIS_LCMS_USER.USER_EMAIL as EMAIL_ADDR, TL_CourseOfferings.START_DATE, 
                    TL_CourseOfferings.END_DATE, TL_CourseOfferings.CourseOfferingCode, TL_CourseOfferings.ACADEMIC_YEAR, 
                    TL_CourseOfferings.ACADEMIC_TERM, 
                    TL_CourseList.CourseCode + '-' + TL_CourseList.CourseName + '-' + TL_CourseOfferings.SECTION_ID CourseName, 
                    PS_SIS_LMS_SCHED_V.*
                    FROM SIS_LCMS_USER, PS_SIS_LMS_SCHED_V, 
                    TL_CourseOfferings, PS_SIS_LMS_INSTR_V, TL_CourseList
                    WHERE SIS_LCMS_USER.USER_LOGON_ID = 'smustf\' + PS_SIS_LMS_INSTR_V.OPRID
                    AND PS_SIS_LMS_INSTR_V.STRM = TL_CourseOfferings.STRM
                    AND PS_SIS_LMS_INSTR_V.CLASS_NBR = TL_CourseOfferings.CLASS_NBR
                    AND TL_CourseOfferings.STRM = PS_SIS_LMS_SCHED_V.STRM
                    AND TL_CourseOfferings.CLASS_NBR = PS_SIS_LMS_SCHED_V.CLASS_NBR
                    AND TL_CourseOfferings.START_DATE <= SYSDATETIME()
                    AND TL_CourseOfferings.END_DATE >= SYSDATETIME()
                    AND TL_CourseList.STRM = TL_CourseOfferings.STRM
                    AND TL_CourseList.CourseCode = TL_CourseOfferings.COURSE_ID
                    AND USER_EMAIL = @UserEmail";


            var param = new SqlParameter("@UserEmail", userEmail);
            var classSchedules = _isisContext.SqlQuery<ClassScheduleDto>(sqlQuery, param).ToList();

            // filter result if user only want 1 course
            if (orgUnitId != 0)
            {
                var courseDetail = _courseRepository.TableNoTracking.Where(c => c.Id == orgUnitId).FirstOrDefault();
                if (courseDetail == null)
                {
                    // when courses not found, we assume rquest came from landing page
                    // TODO when switch to datahub, need to filter by orgunittype
                    return new List<ClassScheduleDto>();
                }
                else
                {
                    var courseOfferingCode = courseDetail.Code;
                    classSchedules = classSchedules
                        .Where(cs => string.Equals(cs.CourseOfferingCode, courseOfferingCode, StringComparison.InvariantCultureIgnoreCase))
                        .ToList();
                }
            }

            return classSchedules;

        }

        public IEnumerable<ClassScheduleDto> GetClassSchedule(List<string> userEmails, int orgUnitId = 0)
        {
            if (userEmails.Count == 0) return new List<ClassScheduleDto>();

            // Student Query union Staff/faculty Query
            var sqlQuery = @"
                SELECT DISTINCT PS_SIS_LMS_STD_VW.EMAIL_ADDR, TL_CourseOfferings.START_DATE, TL_CourseOfferings.END_DATE, TL_CourseOfferings.CourseOfferingCode, 
                LMSISIS.dbo.TL_CourseOfferings.ACADEMIC_YEAR, LMSISIS.dbo.TL_CourseOfferings.ACADEMIC_TERM, V_D2L_COURSE_OFFERING.CourseName, PS_SIS_LMS_SCHED_V.*
                FROM LMSISIS.dbo.PS_SIS_LMS_STD_VW, LMSISIS.dbo.PS_SIS_LMS_SCHED_V, LMSISIS.dbo.TL_CourseOfferings, LMSISIS.dbo.PS_SIS_LMS_ENRL_VW, V_D2L_COURSE_OFFERING
                WHERE PS_SIS_LMS_STD_VW.EMPLID = PS_SIS_LMS_ENRL_VW.EMPLID
                AND PS_SIS_LMS_ENRL_VW.STRM = TL_CourseOfferings.STRM
                AND PS_SIS_LMS_ENRL_VW.CLASS_NBR = TL_CourseOfferings.CLASS_NBR
                AND TL_CourseOfferings.STRM = PS_SIS_LMS_SCHED_V.STRM
                AND TL_CourseOfferings.CLASS_NBR = PS_SIS_LMS_SCHED_V.CLASS_NBR
                AND TL_CourseOfferings.START_DATE <= SYSDATETIME()
                AND TL_CourseOfferings.END_DATE >= SYSDATETIME()
                AND V_D2L_COURSE_OFFERING.CourseId = TL_CourseOfferings.CourseOfferingCode
                AND EMAIL_ADDR IN @UserEmails
                UNION 
                SELECT SIS_LCMS_USER.USER_EMAIL as EMAIL_ADDR, TL_CourseOfferings.START_DATE, TL_CourseOfferings.END_DATE, TL_CourseOfferings.CourseOfferingCode,
                LMSISIS.dbo.TL_CourseOfferings.ACADEMIC_YEAR, LMSISIS.dbo.TL_CourseOfferings.ACADEMIC_TERM, V_D2L_COURSE_OFFERING.CourseName,PS_SIS_LMS_SCHED_V.*
                FROM LMSISIS.dbo.SIS_LCMS_USER, LMSISIS.dbo.PS_SIS_LMS_SCHED_V, LMSISIS.dbo.TL_CourseOfferings, LMSISIS.dbo.PS_SIS_LMS_INSTR_V, V_D2L_COURSE_OFFERING
                WHERE REPLACE(SIS_LCMS_USER.USER_LOGON_ID, 'smustf\','') = PS_SIS_LMS_INSTR_V.OPRID
                AND PS_SIS_LMS_INSTR_V.STRM = TL_CourseOfferings.STRM
                AND PS_SIS_LMS_INSTR_V.CLASS_NBR = TL_CourseOfferings.CLASS_NBR
                AND TL_CourseOfferings.STRM = PS_SIS_LMS_SCHED_V.STRM
				AND TL_CourseOfferings.CLASS_NBR = PS_SIS_LMS_SCHED_V.CLASS_NBR
                AND TL_CourseOfferings.START_DATE <= SYSDATETIME()
                AND TL_CourseOfferings.END_DATE >= SYSDATETIME()
                AND V_D2L_COURSE_OFFERING.CourseId = TL_CourseOfferings.CourseOfferingCode
                AND USER_EMAIL IN @UserEmails";


            var param = "";
            foreach (var email in userEmails)
            {
                param += "'" + email + "',";
            }
            if (param != "")
            {
                param = "(" + param.TrimEnd(',') + ")";
            }
            var classSchedules = _isisContext.SqlQuery<ClassScheduleDto>(sqlQuery.Replace("@UserEmails", param)).ToList();

            // filter result if user only want 1 course
            if (orgUnitId != 0)
            {
                var courseDetail = _courseRepository.TableNoTracking.Where(c => c.Id == orgUnitId).FirstOrDefault();
                if (courseDetail == null)
                {
                    // when courses not found, we assume rquest came from landing page
                    // TODO when switch to datahub, need to filter by orgunittype
                    return new List<ClassScheduleDto>();
                }
                else
                {
                    var courseOfferingCode = courseDetail.Code;
                    classSchedules = classSchedules
                        .Where(cs => string.Equals(cs.CourseOfferingCode, courseOfferingCode, StringComparison.InvariantCultureIgnoreCase))
                        .ToList();
                }
            }

            return classSchedules;

        }

        public List<CourseViewModel> GetCourseSections(int orgUnitId)
        {
            var result = new List<CourseViewModel>();
            var courseDetail = GetById(orgUnitId);

            if (courseDetail != null)
            {
                var isisCourseOffering = _tlCourseOfferingRepo.TableNoTracking.Where(c => c.CourseOfferingCode == courseDetail.Code).FirstOrDefault();
                if (isisCourseOffering == null)
                {
                    // NON ISIS course
                    result.Add(new CourseViewModel()
                    {
                        Id = courseDetail.Id,
                        Code = courseDetail.Code,
                        Name = courseDetail.Name,
                        SectionName = courseDetail.Code,
                        COURSE_ID = string.Empty,
                        ACAD_CAREER = string.Empty,
                        CLASS_NBR = null,
                        STRM = string.Empty,
                        StartDate = courseDetail.StartDate,
                        EndDate = courseDetail.EndDate,
                        SemesterId = courseDetail.SemesterId,
                        DepartmentId = courseDetail.DepartmentId,
                        IsMerge = false,
                        IsIsis = false
                    });
                }
                else
                {
                    // ISIS course
                    if (isisCourseOffering.MERGE_SECTION)
                    {
                        result.Add(new CourseViewModel()
                        {
                            Id = courseDetail.Id,
                            Code = courseDetail.Code,
                            Name = courseDetail.Name,
                            SectionName = isisCourseOffering.SECTION_ID,
                            COURSE_ID = isisCourseOffering.COURSE_ID,
                            ACAD_CAREER = isisCourseOffering.ACAD_CAREER,
                            CLASS_NBR = isisCourseOffering.CLASS_NBR,
                            STRM = isisCourseOffering.STRM,
                            StartDate = courseDetail.StartDate,
                            EndDate = courseDetail.EndDate,
                            SemesterId = courseDetail.SemesterId,
                            DepartmentId = courseDetail.DepartmentId,
                            IsMerge = true,
                            IsIsis = true
                        });

                        var isisIndvCourses = _mergeSectionRepository.TableNoTracking
                            .Where(merge => merge.CourseOfferingCode == isisCourseOffering.CourseOfferingCode)
                            .ToList();
                        var relatedCourseCode = isisIndvCourses.Select(merge => merge.IndvCourseOfferingCode).ToList();
                        var elearnCourses = _courseRepository.TableNoTracking.Where(c => relatedCourseCode.Contains(c.Code)).ToList();
                        var isisCourses = _tlCourseOfferingRepo.TableNoTracking.Where(c => relatedCourseCode.Contains(c.CourseOfferingCode)).ToList();

                        var indCourses = (from elearnCourse in elearnCourses
                                          join isisCourse in isisCourses on elearnCourse.Code equals isisCourse.CourseOfferingCode
                                          select new CourseViewModel
                                          {
                                              Id = elearnCourse.Id,
                                              Code = elearnCourse.Code,
                                              Name = elearnCourse.Name,
                                              COURSE_ID = isisCourse.COURSE_ID,
                                              SectionName = isisCourse.SECTION_ID,
                                              ACAD_CAREER = isisCourse.ACAD_CAREER,
                                              CLASS_NBR = isisCourse.CLASS_NBR,
                                              STRM = isisCourse.STRM,
                                              StartDate = elearnCourse.StartDate,
                                              EndDate = elearnCourse.EndDate,
                                              SemesterId = elearnCourse.SemesterId,
                                              DepartmentId = elearnCourse.DepartmentId,
                                              IsMerge = false,
                                              IsIsis = true
                                          })
                                .ToList();

                        // possibly there is duplicate course code in dirty environment. we pick the latest record
                        var groups = indCourses.GroupBy(c => c.Code);
                        var valence = new Valence.ValenceApi(_configuration);
                        foreach (var group in groups)
                        {
                            if (group.Count() > 1)
                            {
                                foreach (var course in group)
                                {
                                    var courseOffering = valence.GetCourceDetailById(course.Id);
                                    if (courseOffering != null && courseOffering.IsActive)
                                    {
                                        result.Add(course);
                                    }
                                }
                            }
                            else
                            {
                                result.Add(group.First());
                            }
                        }
                    }
                    else
                    {
                        result.Add(new CourseViewModel()
                        {
                            Id = courseDetail.Id,
                            Code = courseDetail.Code,
                            Name = courseDetail.Name,
                            SectionName = isisCourseOffering.SECTION_ID,
                            COURSE_ID = isisCourseOffering.COURSE_ID,
                            ACAD_CAREER = isisCourseOffering.ACAD_CAREER,
                            CLASS_NBR = isisCourseOffering.CLASS_NBR,
                            STRM = isisCourseOffering.STRM,
                            StartDate = courseDetail.StartDate,
                            EndDate = courseDetail.EndDate,
                            SemesterId = courseDetail.SemesterId,
                            DepartmentId = courseDetail.DepartmentId,
                            IsMerge = false,
                            IsIsis = true
                        });
                    }
                }
            }

            return result;
        }

        public List<CourseViewModel> GetSiblingSessions(int orgUnitId)
        {
            var result = new List<CourseViewModel>();
            var courseDetail = GetById(orgUnitId);

            if (courseDetail != null)
            {
                // Siblings definition is derived from tlcourseoffering, hence non isis course cannot have siblings
                var isisCourseInfo = _tlCourseOfferingRepo.TableNoTracking.Where(c => c.CourseOfferingCode == courseDetail.Code).FirstOrDefault();
                if (isisCourseInfo != null)
                {
                    // siblings => same year, term and code
                    var isisIndvCourses = _tlCourseOfferingRepo.TableNoTracking
                        .Where(c => c.ACADEMIC_YEAR == isisCourseInfo.ACADEMIC_YEAR &&
                                    c.ACADEMIC_TERM == isisCourseInfo.ACADEMIC_TERM &&
                                    c.COURSE_ID == isisCourseInfo.COURSE_ID &&
                                    c.MERGE_SECTION == false)
                        .ToList();

                    result = (
                        from isis in isisIndvCourses
                        join course in _courseRepository.TableNoTracking on isis.CourseOfferingCode equals course.Code
                        select new CourseViewModel
                        {
                            Id = course.Id,
                            Code = course.Code,
                            Name = course.Name,
                            SectionName = isis.SECTION_ID,
                            ACAD_CAREER = isis.ACAD_CAREER,
                            CLASS_NBR = isis.CLASS_NBR,
                            STRM = isis.STRM,
                            StartDate = course.StartDate,
                            EndDate = course.EndDate,
                            SemesterId = course.SemesterId,
                            DepartmentId = course.DepartmentId,
                            IsMerge = false,
                            IsIsis = true
                        })
                        .ToList();
                }
            }

            return result;
        }
        public List<CourseViewModel> GetCoursesWithSameCourseIDandTerm(int orgUnitId)
        {
            var result = new List<CourseViewModel>();
            var courseDetail = GetById(orgUnitId);

            if (courseDetail != null)
            {
                var isisCourseOffering = _tlCourseOfferingRepo.TableNoTracking.Where(c => c.CourseOfferingCode == courseDetail.Code).FirstOrDefault();
                if (isisCourseOffering == null)
                {
                    // NON ISIS course
                    result.Add(new CourseViewModel()
                    {
                        Id = courseDetail.Id,
                        Code = courseDetail.Code,
                        Name = courseDetail.Name,
                        SectionName = courseDetail.Code,
                        ACAD_CAREER = string.Empty,
                        CLASS_NBR = null,
                        STRM = string.Empty,
                        StartDate = courseDetail.StartDate,
                        EndDate = courseDetail.EndDate,
                        SemesterId = courseDetail.SemesterId,
                        DepartmentId = courseDetail.DepartmentId,
                        IsMerge = false,
                        IsIsis = false,
                        CourseDisplayName = courseDetail.Code
                    });
                }
                else
                {
                    // ISIS course, get all individual course with same term, year and courseid
                    var courseOfferings = _tlCourseOfferingRepo.TableNoTracking
                        .Where(co =>
                            co.STRM == isisCourseOffering.STRM &&
                            co.COURSE_ID == isisCourseOffering.COURSE_ID
                        )
                        .ToList();

                    var courses =
                        (from courseOffering in courseOfferings
                         join course in _courseRepository.TableNoTracking on courseOffering.CourseOfferingCode equals course.Code
                         select new CourseViewModel
                         {
                             Id = course.Id,
                             Code = course.Code,
                             Name = course.Name,
                             SectionName = courseOffering.SECTION_ID,
                             ACAD_CAREER = courseOffering.ACAD_CAREER,
                             CLASS_NBR = courseOffering.CLASS_NBR,
                             STRM = courseOffering.STRM,
                             StartDate = course.StartDate,
                             EndDate = course.EndDate,
                             SemesterId = course.SemesterId,
                             DepartmentId = course.DepartmentId,
                             IsMerge = courseOffering.MERGE_SECTION,
                             IsIsis = true,
                             CourseDisplayName = $"{courseOffering.COURSE_ID} {courseOffering.ACADEMIC_YEAR} Term {courseOffering.ACADEMIC_TERM}"
                         })
                        .ToList();

                    // possibly there is duplicate course code in dirty environment. we pick the latest record
                    var groups = courses.GroupBy(c => c.Code);
                    var valence = new Valence.ValenceApi(_configuration);
                    foreach (var group in groups)
                    {
                        if (group.Count() > 1)
                        {
                            // if have duplicate, then query d2l for active course
                            foreach (var course in group)
                            {
                                var courseOffering = valence.GetCourceDetailById(course.Id);
                                if (courseOffering != null && courseOffering.IsActive)
                                {
                                    result.Add(course);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            result.Add(group.First());
                        }
                    }
                }
            }
            return result;
        }
        private List<ClassScheduleViewModel> ExtractSchedules(List<PS_SIS_LMS_SCHED_V> classSchedules)
        {
            var schedules = new List<ClassScheduleViewModel>();

            foreach (var classSchedule in classSchedules)
            {
                if (classSchedule.MEETING_TIME_START == null) continue;
                if (classSchedule.MEETING_TIME_END == null) continue;

                var start = new TimeSpan(classSchedule.MEETING_TIME_START.Value.Hour,
                    classSchedule.MEETING_TIME_START.Value.Minute, classSchedule.MEETING_TIME_START.Value.Second);
                var end = new TimeSpan(classSchedule.MEETING_TIME_END.Value.Hour,
                    classSchedule.MEETING_TIME_END.Value.Minute, classSchedule.MEETING_TIME_END.Value.Second);
                var tempSchedule = new ClassScheduleViewModel
                {
                    SelectedDays = new List<DayOfWeek>(),
                    Start = start,
                    End = end
                };

                if (classSchedule.SUN.Trim().ToLower() == "y")
                    tempSchedule.SelectedDays.Add(DayOfWeek.Sunday);

                if (classSchedule.MON.Trim().ToLower() == "y")
                    tempSchedule.SelectedDays.Add(DayOfWeek.Monday);

                if (classSchedule.TUES.Trim().ToLower() == "y")
                    tempSchedule.SelectedDays.Add(DayOfWeek.Tuesday);

                if (classSchedule.WED.Trim().ToLower() == "y")
                    tempSchedule.SelectedDays.Add(DayOfWeek.Wednesday);

                if (classSchedule.THURS.Trim().ToLower() == "y")
                    tempSchedule.SelectedDays.Add(DayOfWeek.Thursday);

                if (classSchedule.FRI.Trim().ToLower() == "y")
                    tempSchedule.SelectedDays.Add(DayOfWeek.Friday);

                if (classSchedule.SAT.Trim().ToLower() == "y")
                    tempSchedule.SelectedDays.Add(DayOfWeek.Saturday);

                schedules.Add(tempSchedule);
            }

            return schedules;
        }

        public async Task<List<SessionClassSchedule>> GetSessionFromClassScheduleByCourse(int courseId)
        {
            var sessions = new List<SessionClassSchedule>();

            // VALIDATE: must be a valid courseId
            var selectedCourse = await _courseRepository.Table.Where(x => x.Id == courseId).FirstOrDefaultAsync(c => c.Id == courseId);
            if (selectedCourse == null) return sessions;

            // VALIDATE: Must be a valid ISIS Course
            // we can only get class schedule for ISIS courses
            var courseDetail = await _tlCourseOfferingRepo.Table.FirstOrDefaultAsync(c => c.CourseOfferingCode == selectedCourse.Code);
            if (courseDetail == null) return sessions;

            var strm = courseDetail.STRM;
            var classNbr = Convert.ToInt32(courseDetail.CLASS_NBR ?? 0);

            // get class schedule from isis
            var classSchedules = await _psSisLmsScheduleRepository.Table.Where(cls => cls.STRM == strm && cls.CLASS_NBR == classNbr).ToListAsync();
            var schedules = ExtractSchedules(classSchedules);

            // VALIDATE: must have START and END DATE
            if (courseDetail.END_DATE == null) return sessions;
            if (courseDetail.START_DATE == null) return sessions;

            // cycle through each day from course start to course end date
            var diff = courseDetail.END_DATE.Value - courseDetail.START_DATE.Value;
            var days = diff.Days;
            for (var i = 0; i <= days; i++)
            {
                // for each day, check if there is any class scheduled
                var tempDate = courseDetail.START_DATE.Value.AddDays(i);
                var classesScheduledForTempDate = schedules.Where(s => s.SelectedDays.Contains(tempDate.DayOfWeek)).ToList();

                // create session based on class schedule
                foreach (var classScheduled in classesScheduledForTempDate)
                {
                    var newSession = new SessionClassSchedule
                    {
                        SessionDate = tempDate,
                        StartTime = classScheduled.Start,
                        EndTime = classScheduled.End
                    };
                    sessions.Add(newSession);
                }
            }

            return sessions;
        }

        public List<int> GetCourseByFeedbackSession(int? userId, List<int> sessions)
        {
            //Get course offering code from peer feedback session
            var courseOfferingCode = _peerFeedbackSessionsRepository.TableNoTracking
                .Where(x => sessions.Contains(x.Id) && x.IsDeleted == false)
                .Select(p => p.CourseOfferingCode).ToList();

            var courseOfferingCodeList = courseOfferingCode.SelectMany(c => c.Split(',')).ToList(); //Flatten course offering code

            //Get course id and code from userEnrollment
            var courseCode = from course in _courseRepository.TableNoTracking
                             join enrolled in _userEnrollmentRepository.TableNoTracking
                                 on course.Id equals enrolled.CourseId
                             join user in _userRepository.TableNoTracking.Where(u => userId == null || u.Id == userId)
                                 on enrolled.UserId equals user.Id
                             select new
                             {
                                 Id = course.Id,
                                 Code = course.Code
                             };

            var result = courseCode.Where(c => courseOfferingCodeList.Any(co => co.Equals(c.Code))).Select(c => c.Id).Distinct().ToList();
            return result;
        }
        public List<int> GetCourseIdByListCode(List<string> courseOfferingCodes)
        {
            if (courseOfferingCodes == null || !courseOfferingCodes.Any()) return null;
            courseOfferingCodes = courseOfferingCodes.Select(x => x.ToLower()).ToList();
            return _courseRepository.Table.Where(c => courseOfferingCodes.Contains(c.Code.ToLower())).Select(x => x.Id).ToList();
        }
        #endregion

        #region Private Functions
        private void createClassInMeeting(Repository<Meeting> meetingRepo, Repository<Entity.LmsTools.MeetingAttendee> meetingAttendeeRepo, DateTime date, List<UserISISClassSchedule> classSchedules
            , int adminOrgUnitId)
        {
            DateTime start, end;
            Meeting newMeeting;
            List<MeetingAttendee> attendees = new List<MeetingAttendee>();
            foreach (var schedule in classSchedules)
            {
                start = new DateTime(
                        date.Year, date.Month, date.Day,
                        schedule.Schedules.MEETING_TIME_START.Value.Hour,
                        schedule.Schedules.MEETING_TIME_START.Value.Minute,
                        schedule.Schedules.MEETING_TIME_START.Value.Second);

                end = new DateTime(
                        date.Year, date.Month, date.Day,
                        schedule.Schedules.MEETING_TIME_END.Value.Hour,
                        schedule.Schedules.MEETING_TIME_END.Value.Minute,
                        schedule.Schedules.MEETING_TIME_END.Value.Second);

                // create meeting
                newMeeting = new Meeting()
                {
                    LastUpdatedOn = DateTime.UtcNow,
                    CreatedOn = DateTime.UtcNow,
                    OwnerId = adminOrgUnitId,
                    Title = $"Class: {schedule.CourseName}",
                    StartDate = start,
                    EndDate = end,
                    Description = "Class Schedule",
                    Course = schedule.CourseName,
                    Location = schedule.Schedules.DESCR,
                    Status = (int)RecordStatus.Active,
                };

                // check if meeting has been created, if existing, do nothing
                meetingRepo.Insert(newMeeting);

                // add student attendee
                foreach (var attendee in schedule.Enrollments)
                {
                    var encrypt =
                        $"{attendee.UserId.Value}{Core.Constants.SplitKey}{schedule.CourseId}{Core.Constants.SplitKey}{DateTime.Now.Ticks}";
                    attendees.Add(new MeetingAttendee
                    {
                        MeetingId = newMeeting.MeetingId,
                        AttendeeId = attendee.UserId.Value,
                        CreatedOn = DateTime.UtcNow,
                        InviteStatus = Convert.ToInt32(InviteStatus.Accept),
                        SecretKey = AesEncrypt.Encrypt(encrypt),
                        Status = (int)RecordStatus.Active
                    });
                }
            }

            meetingAttendeeRepo.Insert(attendees);
        }

        private struct UserISISClassSchedule
        {
            public int CourseId { get; set; }
            public string CourseName { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public List<UserEnrollment> Enrollments { get; set; }
            public PS_SIS_LMS_SCHED_V Schedules { get; set; }
        }

        #endregion
    }
}