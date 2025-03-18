#region USING
using eLearnApps.Business.Interface;
using eLearnApps.Core;
using eLearnApps.Data;
using eLearnApps.Data.Interface;
using eLearnApps.Data.Interface.LmsTool;
using eLearnApps.Entity.LmsTools;
using eLearnApps.Entity.LmsTools.Dto;
using log4net;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CourseCategory = eLearnApps.Entity.LmsTools.CourseCategory;

#endregion


namespace eLearnApps.Business
{
    public class UserEnrollmentService : IUserEnrollmentService
    {
        #region SERVICE
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IDbContext _context;
        private readonly IRepository<UserEnrollment> _userEnrollmentRepository;
        private readonly IRepository<CategoryGroup> _categoryGroupRepository;
        private readonly IRepository<CourseCategory> _courseCategoryRepository;
        private readonly IRepository<Course> _courseRepository;
        private readonly IRepository<GradeResetStatus> _gradeResetStatusRepository;
        private readonly IRepository<Role> _roleRepo;
        private readonly IRepository<Permission> _permissionRepo;
        private readonly IRepository<PermissionRole> _permissionRoleRepo;
        private readonly IUserEnrollmentDao _userEnrollmentDao;
        private readonly IRepository<User> _userRepository;
        #endregion

        #region CTOR
        public UserEnrollmentService(IDbContext context,

               IDaoFactory factory,
               IServiceProvider serviceProvider
            )
        {
            _context = context;
            _userEnrollmentRepository = serviceProvider.GetRequiredKeyedService<IRepository<UserEnrollment>>("default");
            _categoryGroupRepository = serviceProvider.GetRequiredKeyedService<IRepository<CategoryGroup>>("default");
            _courseCategoryRepository = serviceProvider.GetRequiredKeyedService<IRepository<CourseCategory>>("default");
            _courseRepository = serviceProvider.GetRequiredKeyedService<IRepository<Course>>("default");
            _gradeResetStatusRepository = serviceProvider.GetRequiredKeyedService<IRepository<GradeResetStatus>>("default");
            _permissionRepo = serviceProvider.GetRequiredKeyedService<IRepository<Permission>>("default");
            _permissionRoleRepo = serviceProvider.GetRequiredKeyedService<IRepository<PermissionRole>>("default");
            _userRepository = serviceProvider.GetRequiredKeyedService<IRepository<User>>("default");
            _roleRepo = serviceProvider.GetRequiredKeyedService<IRepository<Role>>("default");

            _userEnrollmentDao = factory.UserEnrollmentDao;
        }
        #endregion

        public List<Permission> GetPermissionsForRoleId(int roleId)
        {
            var permissions = _permissionRoleRepo.TableNoTracking.Where(pr => pr.RoleId == roleId)
                .Join(_permissionRepo.TableNoTracking, pr => pr.PermissionId, p => p.Id, (pr, p) => p)
                .ToList();

            return permissions;
        }

        /// <summary>
        /// Get all student in this course
        /// </summary>
        /// <param name="courseId"></param>
        /// <returns></returns>
        public List<int> GetAllStudentByCourse(int courseId)
        {
            var studentIds = (from ue in _userEnrollmentRepository.TableNoTracking
                              join r in _roleRepo.TableNoTracking on ue.RoleId equals r.Id
                              where ue.CourseId == courseId && r.Name == Core.RoleName.STUDENT
                              select ue.UserId.Value).ToList();

            return studentIds;
        }

        public UserEnrollment GetById(int id) => _userEnrollmentRepository.GetById(id);
        public void Insert(UserEnrollment enrollment) => _userEnrollmentRepository.Insert(enrollment);
        public void Update(UserEnrollment enrollment) => _userEnrollmentRepository.Update(enrollment);
        public void Delete(UserEnrollment enrollment) => _userEnrollmentRepository.Delete(enrollment);
        public void Insert(List<UserEnrollment> enrollments) => _userEnrollmentRepository.Insert(enrollments);
        public void Delete(List<UserEnrollment> enrollments) => _userEnrollmentRepository.Delete(enrollments);

        public List<UserEnrollmentDto> GetByUserId(int userId)
        {
            var enroll = (from ue in _userEnrollmentRepository.TableNoTracking
                          join c in _courseRepository.TableNoTracking on ue.CourseId equals c.Id
                          where ue.UserId == userId
                          select new
                          {
                              UserId = ue.UserId,
                              CourseId = ue.CourseId,
                              RoleId = ue.RoleId,
                              IsClasslist = ue.IsClasslist,
                              CourseCode = c.Code,
                          })
                .AsEnumerable()
                .Select(x => new UserEnrollmentDto
                {
                    UserId = x.UserId.GetValueOrDefault(),
                    CourseId = x.CourseId.GetValueOrDefault(),
                    RoleId = x.RoleId.GetValueOrDefault(),
                    IsClassList = x.IsClasslist,
                    CourseCode = x.CourseCode,
                })
                .ToList();
            return enroll;
        }
        public List<UserEnrollmentDto> GetUserEnrolledWithCourseByUserId(int userId)
        {
            var query = from ue in _userEnrollmentRepository.TableNoTracking
                        join c in _courseRepository.TableNoTracking on ue.CourseId equals c.Id
                        join user in _userRepository.TableNoTracking on ue.UserId equals user.Id
                        join role in _roleRepo.TableNoTracking on ue.RoleId equals role.Id
                        where ue.UserId == userId
                        select new UserEnrollmentDto
                        {
                            UserId = user.Id,
                            CourseId = c.Id,
                            RoleId = ue.RoleId.Value,
                            IsClassList = ue.IsClasslist,
                            CourseCode = c.Code,
                            RoleName = role.Name
                        };
            return query.ToList();
        }

        public Entity.LmsTools.Dto.UserEnrollmentDto FindByUserIdAndCourseId(int userId, int courseId)
        {
            var enrollment = _userEnrollmentRepository.Table.Where(x => x.UserId == userId && x.CourseId == courseId).FirstOrDefault();
            if (enrollment != null)
            {
                var courseDetail = _courseRepository.GetById(courseId);
                var roleDetail = _roleRepo.GetById(enrollment.RoleId);
                if (courseDetail != null && roleDetail != null)
                {
                    var retValue = new Entity.LmsTools.Dto.UserEnrollmentDto()
                    {
                        UserId = enrollment.UserId.Value,
                        CourseId = enrollment.CourseId.Value,
                        CourseCode = courseDetail.Code,
                        CourseName = courseDetail.Name,
                        SemesterId = courseDetail.SemesterId.Value,
                        RoleId = roleDetail.Id,
                        RoleName = roleDetail.Name,
                        IsClassList = enrollment.IsClasslist
                    };

                    return retValue;
                }
            }

            // no enrollment found
            return null;
        }
        public List<int?> GetListRoleId(int userId) => _userEnrollmentRepository.Table.Where(x => x.UserId == userId).Select(x => x.RoleId).Distinct().ToList();
        public List<UserEnrollment> GetAll() => _userEnrollmentRepository.Table.ToList();
        public List<UserEnrollment> GetByCourseId(int courseId) => _userEnrollmentRepository.Table.Where(ue => ue.CourseId == courseId).ToList();
        public UserEnrollment GetInstructor(int courseId, int userId)
        {
            var enroll = (from ue in _userEnrollmentRepository.TableNoTracking
                          join r in _roleRepo.TableNoTracking on ue.RoleId equals r.Id
                          where ue.CourseId == courseId && ue.UserId == userId && r.Name == Core.RoleName.INSTRUCTOR
                          select ue).FirstOrDefault();
            return enroll;
        }

        public List<int?> FilterEnrollmentList(int courseId, List<int> attendanceCourseIds)
        {
            try
            {
                //get course category by course id
                var lstCourseCategoryIds = _courseCategoryRepository.Table.Where(x => x.CourseId == courseId).Select(x => x.Id);
                //get category group by course category id
                var lstCategoryGroup = _categoryGroupRepository.Table.Where(x => lstCourseCategoryIds.Contains((int)x.CourseCategoryId) && attendanceCourseIds.Contains(x.Id)).Select(x => x.Id);
                //get user enroll by category group
                var lstUserEnroll = _userEnrollmentRepository.Table.Where(x => lstCategoryGroup.Contains((int)x.CourseId) && x.IsClasslist).Select(x => x.UserId);
                return lstUserEnroll.ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        // 1 to many relation with course, need to delete
        public void Save(List<UserEnrollment> enrollments)
        {
            var courseIds = enrollments.Select(e => e.CourseId.Value).Distinct().ToList();
            var studentEnrollmentInDB = _userEnrollmentRepository.TableNoTracking
                .Where(userEnrollment => courseIds.Contains(userEnrollment.CourseId.Value))
                .Select(userEnrollment => userEnrollment)
                .ToList();

            var enrollmentToCompareQuery = from d2lUser in enrollments
                                           join dbUser in studentEnrollmentInDB on
                                                new { d2lUser.CourseId, d2lUser.UserId } equals
                                                new { dbUser.CourseId, dbUser.UserId } into joinedUsers
                                           from e in joinedUsers.DefaultIfEmpty()
                                           select new { d2ldata = d2lUser, dbdata = e };

            var enrollmentToCompare = enrollmentToCompareQuery.ToList();

            var newEnrollments = new List<UserEnrollment>();
            var enrollmentToUpdate = new List<UserEnrollment>();
            foreach (var enrollment in enrollmentToCompare)
            {
                // enrollment that does not exist in DB, is new enrollment
                if (enrollment.dbdata == null)
                    newEnrollments.Add(enrollment.d2ldata);
                else
                {
                    if (enrollment.dbdata.RoleId != enrollment.d2ldata.RoleId ||
                        enrollment.dbdata.IsClasslist != enrollment.d2ldata.IsClasslist)
                    {
                        var toUpdate = enrollment.dbdata;
                        toUpdate.RoleId = enrollment.d2ldata.RoleId;
                        toUpdate.IsClasslist = enrollment.d2ldata.IsClasslist;
                        enrollmentToUpdate.Add(toUpdate);
                    }
                }
            }

            if (newEnrollments.Count > 0) _userEnrollmentRepository.Insert(newEnrollments);
            if (enrollmentToUpdate.Count > 0) _userEnrollmentRepository.Update(enrollmentToUpdate);

            // for users who are unenrolled:
            // exist in DB, but not in d2l (i.e. d2l == null after join)
            var studentToUnEnrollInDB = (from dbUser in studentEnrollmentInDB
                                         join d2lUser in enrollments on
                                             new { k1 = dbUser.CourseId, k2 = dbUser.UserId } equals
                                             new { k1 = d2lUser.CourseId, k2 = d2lUser.UserId } into joinedUsers
                                         from u in joinedUsers.DefaultIfEmpty()
                                         where u == null
                                         select dbUser).ToList();
            _userEnrollmentRepository.Delete(studentToUnEnrollInDB);
        }

        public List<Entity.LmsTools.Dto.StudentCountDto> GetUserEnrollmentCountByCourseId(int courseId)
        {
            var studentRoleId = _roleRepo.Table.Where(r => r.Name.Trim().ToLower() == Core.RoleName.STUDENT.ToLower()).First().Id;
            var result = new Entity.LmsTools.Dto.StudentCountDto
            {
                Name = "Combined",
                Count = _userEnrollmentRepository.Table.Where(x => x.CourseId == courseId && x.RoleId == studentRoleId).Select(c => c.UserId).Count()
            };
            return new List<Entity.LmsTools.Dto.StudentCountDto> { result };
        }
        public List<Entity.LmsTools.Dto.SectionData> GetUserEnrollmentByCourseId(int courseId)
        {
            var studentRoleId = _roleRepo.Table.Where(r => r.Name.Trim().ToLower() == Core.RoleName.STUDENT.ToLower()).First().Id;
            var result = new Entity.LmsTools.Dto.SectionData
            {
                Name = "All",
                CourseId = courseId,
                Enrollment = _userEnrollmentRepository.Table.Where(x => x.CourseId == courseId && x.RoleId == studentRoleId).Select(c => c.UserId.Value).ToList()
            };
            return new List<Entity.LmsTools.Dto.SectionData> { result };
        }

        public List<Entity.LmsTools.Dto.CourseDto> GetCourseByUserIdAndRole(int userId, bool checkSubmitted)
        {
            var queryUserEnrollments = _userEnrollmentRepository.Table;
            var queryResetStatus = _gradeResetStatusRepository.Table;
            var queryCourses = _courseRepository.Table;

            var studentRoleId = _roleRepo.Table.Where(r => r.Name.Trim().ToLower() == Core.RoleName.STUDENT.ToLower()).First().Id;
            var instructorRoleId = _roleRepo.Table.Where(r => r.Name.Trim().ToLower() == Core.RoleName.INSTRUCTOR.ToLower()).First().Id;

            var query = from userEnrollment in queryUserEnrollments
                        join course in queryCourses on userEnrollment.CourseId equals course.Id
                        where userEnrollment.UserId == userId && userEnrollment.RoleId == instructorRoleId
                        select new Entity.LmsTools.Dto.CourseDto
                        {
                            Name = course.Name,
                            Code = course.Code,
                            Id = course.Id,
                            StudentCount = queryUserEnrollments.Count(x => x.CourseId == course.Id && x.RoleId == studentRoleId)
                        };
            if (checkSubmitted)
            {
                query = from courseDto in query
                        join resetStatus in queryResetStatus on courseDto.Code equals resetStatus.CourseOfferingCode
                        where resetStatus.RESET_STATUS == "N"
                        select courseDto;
            }
            return query.Distinct().ToList();
        }

        /// <summary>
        ///     Get list user id enroll
        /// </summary>
        /// <param name="courseId"></param>
        /// <returns></returns>
        public List<int> GetUserIdEnrollByCourse(int courseId)
        {
            return _userEnrollmentDao.GetUserIdEnrollByCourse(courseId);
        }
        public List<int> GetUserIdEnrollByCourses(List<int?> coursesId)
        {
            var query = from userEnroll in _userEnrollmentRepository.TableNoTracking.Where(x => coursesId.Contains(x.CourseId))
                        join role in _roleRepo.TableNoTracking.Where(x => x.Name.Equals(RoleName.STUDENT, StringComparison.OrdinalIgnoreCase)) on userEnroll.RoleId equals role.Id
                        select userEnroll.UserId.Value;
            return query.Distinct().ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<UserDto> GetUserEnrollByCourseIdAndUserIdAsync(int courseId, int userId)
        {
            var result = await _userEnrollmentDao.GetUserEnrollByCourseIdAndUserIdAsync(courseId, userId);
            if (result == null) return null;
            return new UserDto
            {
                DisplayName = result.DisplayName,
                OrgDefinedId = result.OrgDefinedId,
                EmailAddress = result.EmailAddress
            };
        }
    }
}