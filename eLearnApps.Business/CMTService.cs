using System;
using System.Collections.Generic;
using System.Linq;
using eLearnApps.Business.Interface;
using eLearnApps.Business.Resources;
using eLearnApps.Core;
using eLearnApps.Core.Cryptography;
using eLearnApps.Data;
using eLearnApps.Entity.LmsTools;
using eLearnApps.Entity.LmsTools.Dto;
using Microsoft.Extensions.DependencyInjection;
using CourseCategory = eLearnApps.Entity.LmsTools.CourseCategory;

namespace eLearnApps.Business
{
    public class CmtService : ICmtService
    {
        private readonly IAttendanceDataService _attendanceDataService;
        private readonly IRepository<ClassPhotoSetting> _classPhotoSettingRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<UserEnrollment> _userEnrollRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<CourseCategory> _courseCategoryRepository;
        private readonly IRepository<CategoryGroup> _categoryGroupRepository;
        private readonly IRepository<UserGroup> _userGroupRepository;
        private readonly IRepository<AttendanceList> _attendanceListRepository;
        private readonly IRepository<AttendanceSession> _attendanceSessionRepository;
        private readonly IRepository<AttendanceData> _attendanceDataRepository;
        private readonly IRepository<AttendanceListCategoryOrSection> _attendanceCourseRepository;
        private readonly IRepository<MarkAttendanceSetting> _markAttendanceSettingRepository;
        private readonly IDbContext _context;

        public CmtService(
            IAttendanceDataService attendanceDataService,
   IServiceProvider serviceProvider,
        IDbContext context)
        {
            _attendanceDataService = attendanceDataService;
            _userEnrollRepository = serviceProvider.GetRequiredKeyedService<IRepository<UserEnrollment>>("default");
            _roleRepository = serviceProvider.GetRequiredKeyedService<IRepository<Role>>("default");
            _userRepository = serviceProvider.GetRequiredKeyedService<IRepository<User>>("default");
            _classPhotoSettingRepository = serviceProvider.GetRequiredKeyedService<IRepository<ClassPhotoSetting>>("default");
            _courseCategoryRepository = serviceProvider.GetRequiredKeyedService<IRepository<CourseCategory>>("default");
            _categoryGroupRepository = serviceProvider.GetRequiredKeyedService<IRepository<CategoryGroup>>("default");
            _userGroupRepository = serviceProvider.GetRequiredKeyedService<IRepository<UserGroup>>("default");
            _attendanceSessionRepository = serviceProvider.GetRequiredKeyedService<IRepository<AttendanceSession>>("default");
            _attendanceListRepository = serviceProvider.GetRequiredKeyedService<IRepository<AttendanceList>>("default");
            _attendanceDataRepository = serviceProvider.GetRequiredKeyedService<IRepository<AttendanceData>>("default");
            _attendanceCourseRepository = serviceProvider.GetRequiredKeyedService<IRepository<AttendanceListCategoryOrSection>>("default");
            _markAttendanceSettingRepository = serviceProvider.GetRequiredKeyedService<IRepository<MarkAttendanceSetting>>("default");
            _context = context;
        }

        public List<UserDto> GetUsersByCourseId(int courseId)
        {
            var queryUserEnrollments = _userEnrollRepository.Table;
            var queryUsers = _userRepository.Table;
            var roles = _roleRepository.Table;
            var query = from userEnrollments in queryUserEnrollments
                        join users in queryUsers on userEnrollments.UserId equals users.Id
                        where userEnrollments.CourseId == courseId && userEnrollments.IsClasslist
                        select new UserDto
                        {
                            Id = users.Id,
                            DisplayName = users.DisplayName,
                            EmailAddress = users.EmailAddress,
                            OrgDefinedId = users.OrgDefinedId,
                            ProfileIdentifier = users.ProfileIdentifier,
                            RoleId = roles.FirstOrDefault(x => x.Id == userEnrollments.RoleId).Id,
                            RoleName = roles.FirstOrDefault(x => x.Id == userEnrollments.RoleId).Name
                        };
            return query.OrderBy(x => x.DisplayName).ToList();
        }

        public ClassPhotoSetting GetClassPhotoSettingByCourseId(int courseId, int userId)
        {
            var queryClassPhotoSettings = _classPhotoSettingRepository.Table;

            var query = from classPhotoSetting in queryClassPhotoSettings
                        where classPhotoSetting.CourseId == courseId && classPhotoSetting.CreatedBy == userId
                        select classPhotoSetting;
            var result = query.FirstOrDefault();

            // when there is no setting stored, default to show t
            // photo size: medium, Display all student information, photo position: left, By course
            if (result == null) result = new ClassPhotoSetting()
            {
                PhotoSize = (int)Constants.PhotoSize.Medium,
                PhotoPosition = (int)Constants.PhotoPosition.Left,
                IsFullNameDisplayed = true,
                IsNricFinDisplayed = true,
                IsPhotoDisplayed = true,
                IsSchoolDisplayed = true,
                IsUserNameDisplayed = true,
                PageOrientation = (int)Constants.PageOrientation.Portrait,
                GroupBy = (int)Constants.GroupBy.Course
            };
            return result;
        }
        public MarkAttendanceSetting GetMarkAttendanceSetting(int termId, int courseId, int userId)
        {
            var queryMarkAttendanceSettings = _markAttendanceSettingRepository.Table;
            var query = from markAttendanceSetting in queryMarkAttendanceSettings
                        where markAttendanceSetting.TermId == termId && markAttendanceSetting.CreatedBy == userId
                        select markAttendanceSetting;
            var result = query.FirstOrDefault();
            if (result == null)
            {
                query = from markAttendanceSetting in queryMarkAttendanceSettings
                        where markAttendanceSetting.CourseId == courseId && markAttendanceSetting.CreatedBy == userId
                        select markAttendanceSetting;
                result = query.FirstOrDefault();
            }
            // when there is no setting stored, default to show 
            if (result == null) result = new MarkAttendanceSetting()
            {
                ListView = true
            };
            return result;
        }
        public MarkAttendanceSetting GetMarkAttendanceSettingByCourseId(int courseId, int userId)
        {
            var queryMarkAttendanceSettings = _markAttendanceSettingRepository.Table;
            var query = from markAttendanceSetting in queryMarkAttendanceSettings
                        where markAttendanceSetting.CourseId == courseId && markAttendanceSetting.CreatedBy == userId
                        select markAttendanceSetting;
            var result = query.FirstOrDefault();
            return result;
        }
        public List<Entity.LmsTools.Dto.CourseCategory> GetCategoriesByCourseId(int courseId)
        {
            var courseCategories = _courseCategoryRepository.Table;
            var query = courseCategories.Where(x => x.CourseId == courseId)
                .Select(x => new Entity.LmsTools.Dto.CourseCategory
                {
                    Id = x.Id,
                    Name = x.Name
                });
            return query.ToList();
        }

        public List<GroupData> GetGroupDataByCourseCategoryId(int courseCategoryId)
        {
            var queryUserGroups = _userGroupRepository.Table;
            var queryCategoryGroups = _categoryGroupRepository.Table;
            var queryCourseCategoryGroup = _courseCategoryRepository.TableNoTracking;
            var query = from categoryGroups in queryCategoryGroups
                        where categoryGroups.CourseCategoryId == courseCategoryId
                        select new GroupData
                        {
                            GroupId = categoryGroups.Id,
                            Name = categoryGroups.Name,
                            CourseCategoryGroupName = queryCourseCategoryGroup.FirstOrDefault(x => x.Id == courseCategoryId).Name,
                            Enrollments = queryUserGroups.Where(x => x.CategoryGroupId == categoryGroups.Id).Select(x => x.UserId.Value).ToList()
                        };


            return query.ToList();
        }

        public List<MySessionDto> GetCurrentSessionByStudentId(int courseId, int studentId)
        {
            var queryAttendanceLists = _attendanceListRepository.Table;
            var queryAttendanceSessions = _attendanceSessionRepository.Table;
            var queryAttendanceData = _attendanceDataRepository.Table;

            var attendanceListsInCourseId = _context.SqlQuery<int>(string.Format(Query.AttendanceList_GetListId, courseId)).ToList();
            var attListIdsForStudent = new List<int>();
            foreach (var attendanceListId in attendanceListsInCourseId)
            {
                var isExists = _attendanceDataService.CheckStudentInThisAttendanceList(attendanceListId, studentId);
                if (isExists)
                {
                    attListIdsForStudent.Add(attendanceListId);
                }
            }

            var query = from attendanceSession in queryAttendanceSessions
                        join attendanceList in queryAttendanceLists on attendanceSession.AttendanceListId equals attendanceList.AttendanceListId
                        join attendanceData in queryAttendanceData on new { Key1 = attendanceSession.AttendanceSessionId, Key2 = studentId } equals new { Key1 = attendanceData.AttendanceSessionId, Key2 = attendanceData.UserId } into data
                        from attendanceData in data.DefaultIfEmpty()
                        where attendanceSession.IsDeleted == false && attListIdsForStudent.Contains(attendanceList.AttendanceListId)
                        select new MySessionDto
                        {
                            AttendanceId = attendanceSession.AttendanceListId,
                            AttendanceName = attendanceList.Name,
                            SessionId = attendanceSession.AttendanceSessionId,
                            SessionStartTime = attendanceSession.SessionStartTime,
                            EntryCloseTime = attendanceSession.EntryCloseTime.Value,
                            Percentage = attendanceData == null ? 0 : attendanceData.Percentage,
                            Remarks = attendanceData == null ? "" : attendanceData.Remarks,
                            AttendanceDataId = attendanceData == null ? 0 : attendanceData.AttendanceDataId,
                            AllowStudentEntry = attendanceList.AllowStudentEntry ?? false
                        };
            return query.Distinct().ToList();
        }

        public MyAttendanceDto GetAttendanceDetailById(int attendanceId, int studentId)
        {
            var lstResult = new List<AttendanceData>();
            var queryAttendanceSessions = _context
                .SqlQuery<AttendanceSession>(string.Format(Query.AttendanceSession_GetById, attendanceId)).ToList();
            var sessionIds = queryAttendanceSessions.Select(x => x.AttendanceSessionId).ToList();
            var isSessionExists = sessionIds != null && sessionIds.Any();
            if (isSessionExists)
            {
                var queryAttendanceData = _context.SqlQuery<AttendanceData>(string.Format(Query.AttendanceData_GetById, studentId, string.Join(",", sessionIds))).ToList();
                var pastSessions = queryAttendanceSessions.Where(x => (x.EntryCloseTime == null && x.SessionStartTime < DateTime.UtcNow) || (x.EntryCloseTime != null && x.EntryCloseTime < DateTime.UtcNow)).ToList();
                //generate data attendance in the past
                foreach (var session in pastSessions)
                {
                    var attendance = queryAttendanceData.FirstOrDefault(x => x.AttendanceSessionId == session.AttendanceSessionId && x.UserId == studentId);
                    if (attendance == null)
                    {
                        attendance = new AttendanceData
                        {
                            AttendanceSessionId = session.AttendanceSessionId,
                            UserId = studentId,
                            IsDeleted = false,
                            Percentage = null,
                            LastUpdatedTime = DateTime.UtcNow,
                            LastUpdatedBy = studentId
                        };
                        _attendanceDataService.InsertOrUpdate(attendance);
                    }
                }
                //add existed attendance data
                lstResult.AddRange(queryAttendanceData);
            }
            bool isUpdateSummary = false;
            if (lstResult.Any())
            {
                isUpdateSummary = true;
            }
            var myAttendance = new MyAttendanceDto
            {
                AttendanceId = attendanceId
            };
            if (lstResult.Any())
            {
                var attendanceData = lstResult.Select(x => new AttendanceDataDto
                {
                    Percentage = x.Percentage,
                    AttendanceDataId = x.AttendanceDataId,
                    AttendanceListId = attendanceId,
                    AttendanceSessionId = x.AttendanceSessionId,
                    UserId = x.UserId,
                    Excused = x.Excused
                }).ToList();

                var attendance = GetMyAttendanceScore(attendanceData, attendanceId, isUpdateSummary);
                if (attendance != null) return attendance;
            }
            return myAttendance;
        }

        // TODO: Code Smell DRY (CMTController - GetDataWeeklyAttendance)
        private MyAttendanceDto GetMyAttendanceScore(List<AttendanceDataDto> lstResult, int attendanceId, bool isUpdateSummary = false)
        {
            var lstSession = _attendanceSessionRepository.Table.Where(x => x.AttendanceListId == attendanceId && x.IsDeleted == false).ToList();
            var futureSession = lstSession.Where(x => (x.EntryCloseTime == null && x.SessionStartTime > DateTime.UtcNow) || (x.EntryCloseTime != null && x.EntryCloseTime > DateTime.UtcNow)).Select(x => x.AttendanceSessionId).ToList();
            var itemCount = lstResult.Count;
            var excused = lstResult.Count(x => x.Excused.HasValue && x.Excused.Value);
            var countFuture = lstResult.Count(x => futureSession.Contains(x.AttendanceSessionId) && !x.Percentage.HasValue && !x.Excused.HasValue);
            var countForCalculate = itemCount - countFuture - excused;

            if (countForCalculate == 0)
                return null;

            var present = lstResult.Count(x => x.Percentage == 100);
            var partial = lstResult.Count(x => x.Percentage > 0 && x.Percentage < 100);
            var partialPercent = Convert.ToDouble(lstResult
                .Where(data => data.Percentage > 0 && data.Percentage < 100).Sum(x => x.Percentage / 100));

            var myAttendance = new MyAttendanceDto
            {
                AttendanceId = attendanceId,
                Partial = partial,
                Present = present,
                Total = lstResult.Count,
                Absent = lstResult.Count(x => x.Percentage == 0),
                Score = Convert.ToDecimal(Util.CalculateAttendancePercentage(present, partialPercent, countForCalculate)),
                IsUpdateSummary = isUpdateSummary,
                AttendanceKey = AesEncrypt.Encrypt(attendanceId.ToString()),
                Excused = excused
            };
            return myAttendance;
        }
        public List<MyAttendanceDto> GetAttendanceDetailByListId(List<int> ids, int studentId)
        {
            if (ids == null || ids.Count == 0) return new List<MyAttendanceDto>();
            string query = string.Format(Query.AttendanceData_GetDataForMyAttendanceDetail, studentId, string.Join(",", ids));
            var lstResult = _context.SqlQuery<AttendanceDataDto>(query).ToList();
            var lstResponse = new List<MyAttendanceDto>();
            int countResult = lstResult.Count;
            if (countResult > 0)
            {
                var lstAttendanceListId = lstResult.Select(x => x.AttendanceListId).Distinct().ToList();
                foreach (var id in lstAttendanceListId)
                {
                    var items = lstResult.Where(x => x.AttendanceListId == id).ToList();
                    var attendance = GetMyAttendanceScore(items, id);
                    if (attendance != null) lstResponse.Add(attendance);
                }
            }
            return lstResponse;
        }

        /// <summary>
        /// Get all student attendance data by attendance ids
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public List<MyAttendanceDto> GetAllStudentAttendanceData(List<int> ids)
        {
            if (ids == null || ids.Count == 0) return new List<MyAttendanceDto>();
            string query = string.Format(Query.AttendanceData_GetAllStudentAttendanceDataByListIds, string.Join(",", ids));
            var lstResult = _context.SqlQuery<AttendanceDataDto>(query).ToList();
            var lstResponse = new List<MyAttendanceDto>();
            int countResult = lstResult.Count;
            if (countResult > 0)
            {
                var lstAttendanceListId = lstResult.Select(x => x.AttendanceListId).ToList();
                foreach (var id in lstAttendanceListId)
                {
                    var items = lstResult.Where(x => x.AttendanceListId == id).ToList();
                    var itemCount = items.Count;
                    lstResponse.Add(new MyAttendanceDto
                    {
                        AttendanceId = id,
                        Partial = items.Count(x => x.Percentage > 0 && x.Percentage < 100),
                        Present = items.Count(x => x.Percentage == 100),
                        Total = itemCount,
                        Absent = items.Count(x => x.Percentage == 0),
                        Score = items.Sum(x => x.Percentage) / itemCount,
                        AttendanceKey = AesEncrypt.Encrypt(id.ToString())
                    });
                }
            }
            return lstResponse;
        }

        public List<MyAttendanceDto> GetAllAttendanceByStudentId(int courseId, int studentId)
        {
            // get attendance list by courseid
            var attendanceListsInCourseId = _attendanceListRepository.Table.Where(al => al.CourseId == courseId && al.IsDeleted == false && al.IsVisible == true).ToList();
            // find selected
            var result = new List<MyAttendanceDto>();
            foreach (var attendanceList in attendanceListsInCourseId)
            {
                var userIds = _attendanceDataService.GetSelectedUserForAttendanceList(attendanceList.AttendanceListId);
                if (userIds.Contains(studentId))
                {
                    result.Add(new MyAttendanceDto()
                    {
                        Name = attendanceList.Name,
                        AttendanceId = attendanceList.AttendanceListId
                    });
                }
            }

            return result;
        }

        public List<MySessionDto> GetAttendanceSessionByAttendanceId(int attendanceId, int studentId)
        {
            var attendanceList = _attendanceListRepository.Table.Where(x => x.AttendanceListId == attendanceId && x.IsDeleted == false && x.IsVisible == true).FirstOrDefault();
            if (attendanceList == null) throw new ArgumentNullException(nameof(attendanceId));

            var queryAttendanceSessions = _attendanceSessionRepository.Table.Where(x => x.AttendanceListId == attendanceId && x.IsDeleted == false).OrderByDescending(x => x.SessionStartTime).ToList();
            var results = new List<MySessionDto>();
            foreach (var attendanceSession in queryAttendanceSessions)
            {
                var isPassSession = (attendanceSession.EntryCloseTime == null && attendanceSession.SessionStartTime < DateTime.UtcNow) || (attendanceSession.EntryCloseTime != null && attendanceSession.EntryCloseTime < DateTime.UtcNow);
                var attendanceData = _attendanceDataRepository.Table.Where(x => x.AttendanceSessionId == attendanceSession.AttendanceSessionId && x.IsDeleted == false && x.UserId == studentId).FirstOrDefault();
                if (isPassSession && attendanceData == null)
                {
                    var attendance = new AttendanceData
                    {
                        AttendanceSessionId = attendanceSession.AttendanceSessionId,
                        UserId = studentId,
                        IsDeleted = false,
                        Percentage = null,
                        LastUpdatedTime = DateTime.UtcNow,
                        LastUpdatedBy = studentId
                    };
                    _attendanceDataService.InsertOrUpdate(attendance);
                }
                var dto = new MySessionDto
                {
                    AttendanceId = attendanceSession.AttendanceListId,
                    SessionId = attendanceSession.AttendanceSessionId,
                    SessionStartTime = attendanceSession.SessionStartTime,
                    EntryCloseTime = attendanceSession.EntryCloseTime,
                    Percentage = attendanceData == null ? null : attendanceData.Percentage,
                    Remarks = attendanceData == null ? "" : attendanceData.Remarks,
                    AttendanceDataId = attendanceData == null ? 0 : attendanceData.AttendanceDataId,
                    AllowStudentEntry = attendanceList.AllowStudentEntry ?? false,
                    Excused = attendanceData == null ? null : attendanceData.Excused
                };
                results.Add(dto);
            }


            return results;
        }

    }
}