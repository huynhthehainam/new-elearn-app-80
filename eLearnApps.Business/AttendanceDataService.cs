#region USING

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using eLearnApps.Business.Interface;
using eLearnApps.Business.Resources;
using eLearnApps.Core;
using eLearnApps.Data;
using eLearnApps.Data.Interface;
using eLearnApps.Data.Interface.LmsTool;
using eLearnApps.Entity.LmsTools;
using eLearnApps.Entity.LmsTools.Dto;
using eLearnApps.Entity.Security;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace eLearnApps.Business
{
    public class AttendanceDataService : IAttendanceDataService
    {
        #region CTOR

        /// <summary>
        ///     CTOR
        /// </summary>
        public AttendanceDataService(IDbContext context,

            IUserEnrollmentService userEnrollmentService,
            IUserGroupService userGroupService,
            IDaoFactory factory,
            IServiceProvider serviceProvider
          )
        {
            _context = context;
            _attendanceDataRepository = serviceProvider.GetRequiredKeyedService<IRepository<AttendanceData>>("default");
            _attendanceListRepository = serviceProvider.GetRequiredKeyedService<IRepository<AttendanceList>>("default");
            _attendanceSessionRepository = serviceProvider.GetRequiredKeyedService<IRepository<AttendanceSession>>("default");
            _attendanceCourseRepository = serviceProvider.GetRequiredKeyedService<IRepository<AttendanceListCategoryOrSection>>("default");
            _userEnrollmentRepository = serviceProvider.GetRequiredKeyedService<IRepository<UserEnrollment>>("default");
            _userRepository = serviceProvider.GetRequiredKeyedService<IRepository<User>>("default");
            _attendanceAttachmentRepository = serviceProvider.GetRequiredKeyedService<IRepository<AttendanceAttachment>>("default");
            _userEnrollmentService = userEnrollmentService;
            _userGroupService = userGroupService;
            _userGroupRepository = serviceProvider.GetRequiredKeyedService<IRepository<UserGroup>>("default");
            _roleRepo = serviceProvider.GetRequiredKeyedService<IRepository<Role>>("default");

            _attendanceDataDao = factory.AttendanceDataDao;
        }

        #endregion

        /// <summary>
        ///     get attendance data by attendance list id
        /// </summary>
        /// <param name="attendanceListId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<AttendanceUserWeekData> GetAttendanceWeeklyByAttendanceId(int attendanceListId, ref List<AttendanceData> listInsert, ref List<AttendanceData> listUpdate, int userId = 0)
        {
            var userIds = GetSelectedUserForAttendanceList(attendanceListId);
            var userData = _userRepository.Table.Where(user => userIds.Contains(user.Id)).OrderBy(x => x.DisplayName);
            var lstUsers = userData.ToList();
            var ids = lstUsers.Select(x => x.Id);
            var sessions =
                _attendanceSessionRepository.Table.Where(x =>
                    x.AttendanceListId == attendanceListId && x.IsDeleted == false);
            var lstResult = new List<AttendanceData>();
            var sessionIds = sessions.Select(x => x.AttendanceSessionId);
            var dataAttendance = _attendanceDataRepository.Table.Where(x =>
                x.IsDeleted == false && ids.Contains(x.UserId) && sessionIds.Contains(x.AttendanceSessionId)).ToList();
            //generate data attendance in the past
            var pastSessions = sessions.Where(x => (x.EntryCloseTime == null && x.SessionStartTime < DateTime.UtcNow) || (x.EntryCloseTime != null && x.EntryCloseTime < DateTime.UtcNow)).ToList();
            foreach (var session in pastSessions)
            {
                foreach (var user in userIds)
                {
                    var attendance = dataAttendance.FirstOrDefault(x => x.AttendanceSessionId == session.AttendanceSessionId && x.UserId == user);
                    if (attendance == null)
                    {
                        attendance = new AttendanceData
                        {
                            AttendanceSessionId = session.AttendanceSessionId,
                            UserId = user,
                            IsDeleted = false,
                            Percentage = null,
                            LastUpdatedTime = DateTime.UtcNow,
                            LastUpdatedBy = userId
                        };
                        lstResult.Add(attendance);
                    }
                }
            }

            if (lstResult.Count > 0)
            {
                listInsert.AddRange(lstResult);
            }
            //add existed attendance data
            lstResult.AddRange(dataAttendance);

            var lstDataId = lstResult.Select(x => x.AttendanceDataId).ToList();
            var attachments = _attendanceAttachmentRepository.Table.Where(x => lstDataId.Contains(x.AttendanceDataId)).ToList();

            var result = lstUsers.GroupJoin(lstResult, user => user.Id, data => data.UserId,
                (user, data) =>
                    new AttendanceUserWeekData
                    {
                        UserInfo = user,
                        AttendanceData = data.ToList(),
                        Attachments = attachments
                    }).ToList();
            return result;
        }

        public List<int> GetSelectedUserForAttendanceList(int attendanceListId)
        {
            var userIds = new List<int>();
            var attendanceList = _attendanceListRepository.GetById(attendanceListId);
            if (attendanceList != null)
            {
                if (attendanceList.IncludeAllUsers.HasValue && attendanceList.IncludeAllUsers.Value)
                {
                    // all user enrollment
                    userIds = (from ue in _userEnrollmentRepository.TableNoTracking
                               join r in _roleRepo.TableNoTracking on ue.RoleId equals r.Id
                               where ue.CourseId == attendanceList.CourseId && ue.IsClasslist &&
                                    r.Name == Core.RoleName.STUDENT
                               select ue.UserId.Value)
                              .ToList();
                }
                else
                {
                    // only selected user in Section or Category
                    var courseCategoryOrSections = _attendanceCourseRepository.Table
                        .Where(x => x.AttendanceListId == attendanceListId).ToList();
                    if (courseCategoryOrSections.Count > 0)
                    {
                        var sectionIds = courseCategoryOrSections
                            .Where(obj => obj.Type == (int)AttendanceCourseType.Section)
                            .Select(obj => obj.CategoryOrSectionId).ToList();

                        var categoryIds = courseCategoryOrSections
                            .Where(obj => obj.Type == (int)AttendanceCourseType.Category)
                            .Select(obj => obj.CategoryOrSectionId).ToList();
                        if (sectionIds.Count > 0)
                        {
                            var lstUser = _userEnrollmentService.GetAllStudentByCourse(attendanceList.CourseId);

                            // get all user enrolled into sections
                            var lstUserEnrollByCourseId = (from ue in _userEnrollmentRepository.Table
                                                           join r in _roleRepo.TableNoTracking on ue.RoleId equals r.Id
                                                           where sectionIds.Contains(ue.CourseId.Value) && r.Name == Core.RoleName.STUDENT &&
                                                           lstUser.Contains(ue.UserId.Value)
                                                           select ue.UserId.Value).Distinct().ToList();

                            userIds.AddRange(lstUserEnrollByCourseId);
                        }

                        // get all users enrolled into a group
                        var lstGroupCategory = _userGroupRepository.Table.Where(x => categoryIds.Contains(x.CategoryGroupId.Value) && !userIds.Contains(x.UserId.Value)).Select(x => x.UserId.Value).ToList();
                        userIds.AddRange(lstGroupCategory);
                    }
                }
            }

            return userIds;
        }
        public bool CheckStudentInThisAttendanceList(int attendanceListId, int studentId)
        {
            var attendanceList = _attendanceListRepository.GetById(attendanceListId);
            if (attendanceList != null)
            {
                if (attendanceList.IncludeAllUsers.HasValue && attendanceList.IncludeAllUsers.Value)
                {
                    return (from ue in _userEnrollmentRepository.TableNoTracking
                            join r in _roleRepo.TableNoTracking on ue.RoleId equals r.Id
                            where ue.CourseId == attendanceList.CourseId && ue.IsClasslist
                                && ue.UserId == studentId && r.Name == Core.RoleName.STUDENT
                            select ue).Any();
                }

                // only selected user in Section or Category
                var courseCategoryOrSections = _attendanceCourseRepository.Table
                    .Where(x => x.AttendanceListId == attendanceListId).ToList();
                if (courseCategoryOrSections.Count > 0)
                {
                    var sectionIds = courseCategoryOrSections
                        .Where(obj => obj.Type == (int)AttendanceCourseType.Section)
                        .Select(obj => obj.CategoryOrSectionId).ToList();

                    var categoryIds = courseCategoryOrSections
                        .Where(obj => obj.Type == (int)AttendanceCourseType.Category)
                        .Select(obj => obj.CategoryOrSectionId).ToList();

                    // get all user enrolled into sections
                    if (_userEnrollmentRepository.Table
                        .Any(x => sectionIds.Contains(x.CourseId.Value) && x.UserId.Value == studentId)) return true;

                    // get all users enrolled into a group
                    if (_userGroupRepository.Table
                        .Any(x => categoryIds.Contains(x.CategoryGroupId.Value) && x.UserId.Value == studentId)) return true;
                }
            }

            return false;
        }
        /// <summary>
        /// update absent
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int UpdateAbsent(int courseId, int userId)
        {
            string query = string.Format(Query.AttendanceData_UpdateAbsent, userId, courseId);
            return _context.ExecuteSqlCommand(query);
        }

        /// <summary>
        /// Get data for class photo summary
        /// </summary>
        /// <param name="attendanceListId"></param>
        /// <param name="userId"></param>
        /// <param name="listInsert"></param>
        /// <param name="listUpdate"></param>
        /// <returns></returns>
        public List<AttendanceUserWeekData> GetAttendanceWeeklyByAttendanceIdForClassPhoto(int attendanceListId,
            int userId, ref List<AttendanceData> listInsert, ref List<AttendanceData> listUpdate)
        {
            // get students in this attendanceList
            var userIds = GetSelectedUserForAttendanceList(attendanceListId);
            var userData = _userRepository.Table
                .Where(user => userIds.Contains(user.Id)).OrderBy(x => x.DisplayName);
            var lstUsers = userData.ToList();
            var ids = lstUsers.Select(x => x.Id);

            var sessions = _attendanceSessionRepository.Table
                .Where(x => x.AttendanceListId == attendanceListId && x.IsDeleted == false);

            var lstResult = new List<AttendanceData>();

            // get current attendance data 
            var sessionIds = sessions.Select(x => x.AttendanceSessionId);
            var dataAttendance = _attendanceDataRepository.Table
                .Where(x => x.IsDeleted == false && ids.Contains(x.UserId) &&
                            sessionIds.Contains(x.AttendanceSessionId))
                .ToList();

            // update attendance for all session in the  past (change unfilled into absent)
            var pastSessions = sessions
                .Where(x => (x.EntryCloseTime == null && x.SessionStartTime < DateTime.UtcNow) ||
                            (x.EntryCloseTime != null && x.EntryCloseTime < DateTime.UtcNow))
                .ToList();
            foreach (var session in pastSessions)
            {
                foreach (var user in userIds)
                {
                    var attendance = dataAttendance.FirstOrDefault(x => x.AttendanceSessionId == session.AttendanceSessionId && x.UserId == user);
                    if (attendance == null)
                    {
                        attendance = new AttendanceData
                        {
                            AttendanceSessionId = session.AttendanceSessionId,
                            UserId = user,
                            IsDeleted = false,
                            Percentage = null,
                            LastUpdatedTime = DateTime.UtcNow,
                            LastUpdatedBy = userId
                        };
                        listInsert.Add(attendance);
                    }

                    // add attendance data to result container
                    lstResult.Add(attendance);
                }
            }

            // There is no students in this attendance list, but there is attendance data. we do some clean up
            // NOTE: these attendance will not be in result container
            if (userIds == null || userIds.Count == 0)
            {
                listUpdate.AddRange(dataAttendance.Select(c => { c.IsDeleted = true; return c; }).ToList());
            }

            //add existed attendance data
            var result = lstUsers.GroupJoin(lstResult, user => user.Id, data => data.UserId,
                (user, data) =>
                    new AttendanceUserWeekData
                    {
                        UserInfo = user,
                        AttendanceData = data.ToList()
                    }).ToList();
            return result;
        }

        /// <summary>
        /// Save silent
        /// </summary>
        /// <param name="listInsert"></param>
        /// <param name="listUpdate"></param>
        /// <returns></returns>
        public void UpdateAttendanceDataSilent(List<AttendanceData> listInsert, List<AttendanceData> listUpdate)
        {
            if (listInsert != null && listInsert.Count > 0)
            {
                _attendanceDataDao.Insert(listInsert);
            }
            if (listUpdate != null && listUpdate.Count > 0)
            {
                _attendanceDataDao.Update(listUpdate);
            }
        }

        /// <summary>
        /// Insert or update
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public int Save(List<AttendanceData> list)
        {
            return _attendanceDataDao.Save(list);
        }

        /// <summary>
        /// Get attendance data by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public AttendanceData GetById(int id)
        {
            return _attendanceDataRepository.Table.FirstOrDefault(x => x.AttendanceDataId == id && x.IsDeleted == false);
        }

        /// <summary>
        ///  Bulk insert attendance data
        /// </summary>
        /// <param name="attendanceList"></param>
        /// <param name="listSessionId"></param>
        /// <param name="userId"></param>
        public void InsertAttendanceData(int attendanceList, List<int> listSessionId, int userId)
        {
            var userIds = GetSelectedUserForAttendanceList(attendanceList);
            DataTable dtSource = _attendanceDataDao.GetAttendanceDataSchema();
            AttendanceData schema = new AttendanceData();
            foreach (var sessionId in listSessionId)
            {
                foreach (var id in userIds)
                {
                    DataRow row = dtSource.NewRow();
                    row[nameof(schema.Percentage)] = DBNull.Value;
                    row[nameof(schema.Remarks)] = DBNull.Value;
                    row[nameof(schema.UserId)] = id;
                    row[nameof(schema.AttendanceSessionId)] = sessionId;
                    row[nameof(schema.IsDeleted)] = false;
                    row[nameof(schema.LastUpdatedBy)] = userId;
                    row[nameof(schema.LastUpdatedTime)] = DateTime.UtcNow;
                    dtSource.Rows.Add(row);
                }
            }
            _attendanceDataDao.InsertAttendanceData(dtSource);
        }

        /// <summary>
        /// update attendance data
        /// </summary>
        /// <param name="attendanceData"></param>
        public void SetAllAttendance(AttendanceData attendanceData)
        {
            if (attendanceData == null)
            {
                throw new ArgumentNullException("attendanceData is not valid.");
            }

            // create record for those still missing data
            var attendanceSession = _attendanceSessionRepository.GetById(attendanceData.AttendanceSessionId);
            var userIds = GetSelectedUserForAttendanceList(attendanceSession.AttendanceListId);
            var userWithAttendanceData = _attendanceDataRepository.Table.Where(ad => ad.AttendanceSessionId == attendanceSession.AttendanceSessionId).Select(ad => ad.UserId).Distinct();
            var userToCreateAttendanceData = userIds.Where(id => !userWithAttendanceData.Contains(id))
                .Select(id => new AttendanceData()
                {
                    AttendanceSessionId = attendanceSession.AttendanceSessionId,
                    UserId = id,
                    IsDeleted = false,
                    Percentage = null,
                    LastUpdatedTime = DateTime.UtcNow,
                    LastUpdatedBy = attendanceData.LastUpdatedBy
                }).ToList();
            _attendanceDataDao.Insert(userToCreateAttendanceData);

            // update all to required data
            _attendanceDataDao.UpdateForSetAll(attendanceData);
        }

        /// <summary>
        /// Update attendance list data
        /// </summary>
        /// <param name="listAttendance"></param>
        public void UpdateAttendanceData(List<AttendanceData> listAttendance)
        {
            listAttendance.ForEach(attd =>
            {
                InsertOrUpdate(attd);
            });
        }
        public void SaveAttendanceData(List<AttendanceData> listAttendance)
        {
            if (listAttendance == null || !listAttendance.Any())
                throw new ArgumentNullException(nameof(listAttendance));

            var sessionId = listAttendance[0].AttendanceSessionId;
            using (var scope = new System.Transactions.TransactionScope())
            {
                var attendanceBySession = _attendanceDataDao.GetBySession(sessionId);
                for (int i = 0; i < listAttendance.Count; i++)
                {
                    var data = listAttendance[i];
                    if (data.AttendanceDataId > 0)
                    {
                        // get attendance data by user
                        var attendanceRecords = attendanceBySession.Where(x => x.UserId == data.UserId).ToList();
                        if (attendanceRecords.Any())
                        {
                            // in case of dirty excess is soft deleted
                            if (attendanceRecords.Count > 1)
                            {
                                var AttendanceDataIdsToSoftDelete = attendanceRecords.Select(x => x.AttendanceDataId).ToList();
                                _attendanceDataDao.SoftDelete(AttendanceDataIdsToSoftDelete);
                            }
                            // for bulk update, we do not update remark
                            var recordToUpdate = new AttendanceData
                            {
                                Percentage = data.Percentage,
                                Remarks = attendanceRecords.First().Remarks,
                                LastUpdatedBy = data.LastUpdatedBy,
                                LastUpdatedTime = data.LastUpdatedTime,
                                Excused = data.Excused,
                                Participation = data.Participation,
                                IsDeleted = false,
                                AttendanceDataId = data.AttendanceDataId
                            };
                            _attendanceDataDao.Update(recordToUpdate);
                        }
                        else
                        {
                            _attendanceDataDao.Insert(data);
                        }
                    }
                    else
                    {
                        _attendanceDataDao.Insert(data);
                    }
                }
                //// confirm
                scope.Complete();
            }
        }
        /// <summary>
        /// Get data for My Attendance
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="studentId"></param>
        /// <returns></returns>
        public List<MyAttendanceDto> GetAllAttendanceByStudentId(int courseId, int studentId)
        {
            var lstAttendance = _attendanceListRepository.Table.Where(al =>
                al.CourseId == courseId && al.IsDeleted == false && al.IsVisible == true).ToList();
            List<MyAttendanceDto> lstResult = new List<MyAttendanceDto>();
            foreach (var attendance in lstAttendance)
            {
                var isExists = CheckStudentInThisAttendanceList(attendance.AttendanceListId, studentId);
                if (isExists)
                {
                    lstResult.Add(new MyAttendanceDto
                    {
                        Name = attendance.Name,
                        AttendanceId = attendance.AttendanceListId
                    });
                }
            }

            return lstResult;
        }

        /// <summary>
        ///     Get attendance data
        /// </summary>
        /// <param name="attendanceListId"></param>
        /// <param name="attendanceSessionId"></param>
        /// <returns></returns>
        public List<AttendanceUserWeekData> GetAttendanceData(int attendanceListId, int attendanceSessionId
        )
        {
            var userIds = GetSelectedUserForAttendanceList(attendanceListId);
            var userData = _userRepository.Table.Where(x => userIds.Contains(x.Id)).OrderBy(x => x.DisplayName);


            var lstUsers = userData.ToList();
            if (lstUsers.Any())
            {
                var ids = lstUsers.Select(x => x.Id);
                var dataAttendance = _attendanceDataRepository.Table
                    .Where(x => x.AttendanceSessionId == attendanceSessionId && x.IsDeleted == false && ids.Contains(x.UserId)).Select(p => new AttendanceDataWithAttachment
                    {
                        UserId = p.UserId,
                        AttendanceSessionId = p.AttendanceSessionId,
                        AttendanceDataId = p.AttendanceDataId,
                        Percentage = p.Percentage,
                        Remarks = string.IsNullOrEmpty(p.Remarks) ? string.Empty : p.Remarks,
                        Participation = p.Participation,
                        IsDeleted = p.IsDeleted,
                        Excused = p.Excused
                    });
                var result = lstUsers.GroupJoin(dataAttendance, user => user.Id, data => data.UserId,
                    (user, data) =>
                        new AttendanceUserWeekData
                        {
                            UserInfo = user,
                            AttendanceDataUserWithAttachment = data.FirstOrDefault()
                        }).ToList();
                return result;
            }

            return new List<AttendanceUserWeekData>();
        }

        /// <summary>
        ///     Get attendance data by userid, attendance session id, attendance data id
        /// </summary>
        /// <param name="attendanceData"></param>
        /// <returns></returns>
        public AttendanceData GetByCondition(AttendanceData attendanceData)
        {
            return _attendanceDataRepository.Table.FirstOrDefault(x =>
                x.AttendanceDataId == attendanceData.AttendanceDataId &&
                x.AttendanceSessionId == attendanceData.AttendanceSessionId && x.UserId == attendanceData.UserId &&
                x.IsDeleted == false);
        }

        public void InsertOrUpdate(AttendanceData data)
        {
            using (var transaction = _context.BeginTransaction())
            {
                try
                {
                    // get all attendance data by session and user
                    var attendanceRecords = _attendanceDataRepository.TableNoTracking
                        .Where(atd => atd.AttendanceSessionId == data.AttendanceSessionId && atd.UserId == data.UserId && atd.IsDeleted == false)
                        .OrderByDescending(atd => atd.LastUpdatedTime).ThenBy(atd => atd.AttendanceDataId)
                        .ToList();

                    // if no entry, then insert
                    // if have entry, 1 record => update, many record do clean up, then update
                    // clean up => keep latest record, delete other record
                    if (!attendanceRecords.Any())
                    {
                        _attendanceDataRepository.Insert(data);
                    }
                    else
                    {
                        if (attendanceRecords.Count() > 1)
                        {
                            var AttendanceDataIdsToSoftDelete = attendanceRecords.Skip(1).Select(atd => atd.AttendanceDataId).ToList();
                            _attendanceDataDao.SoftDelete(AttendanceDataIdsToSoftDelete);
                        }
                        // update latest record
                        AttendanceData recordToUpdate = attendanceRecords.First();
                        recordToUpdate.Percentage = data.Percentage;
                        recordToUpdate.Remarks = data.Remarks;
                        recordToUpdate.LastUpdatedBy = data.LastUpdatedBy;
                        recordToUpdate.LastUpdatedTime = data.LastUpdatedTime;
                        recordToUpdate.Excused = data.Excused;
                        recordToUpdate.Participation = data.Participation;
                        _attendanceDataRepository.Update(recordToUpdate);
                    }

                    //// confirm
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="list"></param>
        public void Update(List<AttendanceData> list)
        {
            _attendanceDataRepository.Update(list);
        }

        /// <summary>
        ///     Get user attendance data with attachment
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public AttendanceUserData GetAttendanceDataWithAttachment(int id, int userId)
        {
            try
            {
                var data = _attendanceDataRepository.Table.FirstOrDefault(x =>
                    x.AttendanceDataId == id && x.UserId == userId && x.IsDeleted == false);
                if (data == null)
                    return null;
                var attachment = _attendanceAttachmentRepository.Table.FirstOrDefault(x => x.AttendanceDataId == id);
                return new AttendanceUserData
                {
                    UserData = data,
                    Attachment = attachment
                };
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="list"></param>
        public void Insert(List<AttendanceData> list)
        {
            _attendanceDataRepository.Insert(list);
        }

        /// <summary>
        ///     Set all attendance data
        /// </summary>
        /// <param name="attendanceListId"></param>
        /// <param name="attendanceSessionId"></param>
        /// <param name="courseId"></param>
        /// <param name="userUpdate"></param>
        /// <param name="status"></param>
        /// <param name="isUpdateAbsent"></param>
        /// <returns></returns>
        public int SetAllAttendance(int attendanceListId, int attendanceSessionId, int courseId, int userUpdate, bool status, bool? isUpdateAbsent = default(bool?))
        {
            try
            {
                var percentage = status ? 100 : 0;
                if (isUpdateAbsent.HasValue && isUpdateAbsent.Value)
                {
                    percentage = 0;
                }
                var updateTime = DateTime.UtcNow;

                var userIds = GetSelectedUserForAttendanceList(attendanceListId);

                // update existing
                var userIdsWithExistingAttendanceData = isUpdateAbsent.HasValue && isUpdateAbsent.Value
                    ? _attendanceDataRepository.Table
                        .Where(x => x.AttendanceSessionId == attendanceSessionId && x.IsDeleted == false && x.Percentage == null)
                        .Select(x => x.UserId)
                        .ToList()
                    : _attendanceDataRepository.Table
                        .Where(x => x.AttendanceSessionId == attendanceSessionId && x.IsDeleted == false)
                        .Select(x => x.UserId)
                        .ToList();

                if (userIdsWithExistingAttendanceData.Count > 0)
                {
                    _attendanceDataDao.UpdateForSetAll(new AttendanceData
                    {
                        LastUpdatedTime = updateTime,
                        LastUpdatedBy = userUpdate,
                        Percentage = percentage,
                        AttendanceSessionId = attendanceSessionId
                    });
                }

                // insert new one if there is any
                // find new userIds
                var userIdWithoutAttendanceData = userIds.Where(id => !userIdsWithExistingAttendanceData.Contains(id)).ToList();
                if (userIdWithoutAttendanceData.Count > 0)
                {
                    var newAttendanceData = new List<AttendanceData>();
                    foreach (var userId in userIdWithoutAttendanceData)
                        newAttendanceData.Add(new AttendanceData
                        {
                            AttendanceSessionId = attendanceSessionId,
                            UserId = userId,
                            Percentage = percentage,
                            LastUpdatedBy = userUpdate,
                            LastUpdatedTime = updateTime,
                            IsDeleted = false
                        });
                    _attendanceDataDao.Insert(newAttendanceData);
                }
                return attendanceSessionId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<AttendanceData> GetAttendanceDataByIdAndUserId(int id, int userId)
        {
            try
            {
                return await _attendanceDataRepository.Table.FirstOrDefaultAsync(x => x.AttendanceDataId == id && x.UserId == userId && x.IsDeleted == false);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #region SERVICE

        private readonly IRepository<AttendanceData> _attendanceDataRepository;
        private readonly IRepository<AttendanceList> _attendanceListRepository;
        private readonly IRepository<AttendanceSession> _attendanceSessionRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<AttendanceListCategoryOrSection> _attendanceCourseRepository;
        private readonly IRepository<UserEnrollment> _userEnrollmentRepository;
        private readonly IRepository<AttendanceAttachment> _attendanceAttachmentRepository;
        private readonly IUserEnrollmentService _userEnrollmentService;
        private readonly IUserGroupService _userGroupService;
        private readonly IDbContext _context;
        private readonly IRepository<UserGroup> _userGroupRepository;
        private readonly IRepository<Entity.LmsTools.Role> _roleRepo;
        private readonly IAttendanceDataDao _attendanceDataDao;

        #endregion
    }
}