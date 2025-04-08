using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using eLearnApps.Entity.LmsTools;
using eLearnApps.Entity.LmsTools.Dto;

namespace eLearnApps.Business.Interface
{
    public interface IAttendanceDataService
    {
        /// <summary>
        ///     get attendance data by attendance list id
        /// </summary>
        /// <param name="attendanceListId"></param>
        /// <param name="listInsert"></param>
        /// <param name="listUpdate"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<AttendanceUserWeekData> GetAttendanceWeeklyByAttendanceId(int attendanceListId, ref List<AttendanceData> listInsert, ref List<AttendanceData> listUpdate, int userId = 0);
        /// <summary>
        /// get attendance data
        /// </summary>
        /// <param name="attendanceListId"></param>
        /// <param name="attendanceSessionId"></param>
        /// <returns></returns>
        List<AttendanceUserWeekData> GetAttendanceData(int attendanceListId, int attendanceSessionId);
        /// <summary>
        /// Get attendance data by userid, attendance session id, attendance data id
        /// </summary>
        /// <param name="attendanceData"></param>
        /// <returns></returns>
        AttendanceData GetByCondition(AttendanceData attendanceData);

        void InsertOrUpdate(AttendanceData data);

        /// <summary>
        /// Insert list attendance data
        /// </summary>
        /// <param name="list"></param>
        void Insert(List<AttendanceData> list);
        /// <summary>
        /// Update list attendance data
        /// </summary>
        /// <param name="list"></param>
        void Update(List<AttendanceData> list);
        /// <summary>
        /// Get user attendance data with attachment
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        AttendanceUserData GetAttendanceDataWithAttachment(int id, int userId);
        /// <summary>
        /// Set all attendance data
        /// </summary>
        /// <param name="attendanceListId"></param>
        /// <param name="attendanceSessionId"></param>
        /// <param name="courseId"></param>
        /// <param name="userUpdate"></param>
        /// <param name="status"></param>
        /// <param name="isUpdateAbsent"></param>
        /// <returns></returns>
        int SetAllAttendance(int attendanceListId, int attendanceSessionId, int courseId, int userUpdate, bool status, bool? isUpdateAbsent = default(bool?));

        List<int> GetSelectedUserForAttendanceList(int attendanceListId);
        /// <summary>
        /// Get data for My Attendance
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="studentId"></param>
        /// <returns></returns>
        List<MyAttendanceDto> GetAllAttendanceByStudentId(int courseId, int studentId);
        bool CheckStudentInThisAttendanceList(int attendanceListId, int studentId);
        int UpdateAbsent(int courseId, int userId);
        /// <summary>
        /// Get data for class photo summary
        /// </summary>
        /// <param name="attendanceListId"></param>
        /// <param name="userId"></param>
        /// <param name="listInsert"></param>
        /// <param name="listUpdate"></param>
        /// <returns></returns>
        List<AttendanceUserWeekData> GetAttendanceWeeklyByAttendanceIdForClassPhoto(int attendanceListId, int userId, ref List<AttendanceData> listInsert, ref List<AttendanceData> listUpdate);
        /// <summary>
        /// Save silent
        /// </summary>
        /// <param name="listInsert"></param>
        /// <param name="listUpdate"></param>
        /// <returns></returns>
        void UpdateAttendanceDataSilent(List<AttendanceData> listInsert, List<AttendanceData> listUpdate);
        /// <summary>
        /// Insert or update
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        int Save(List<AttendanceData> list);
        /// <summary>
        /// Get attendance data by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        AttendanceData GetById(int id);
        /// <summary>
        ///  Bulk insert attendance data
        /// </summary>
        /// <param name="attendanceList"></param>
        /// <param name="listSessionId"></param>
        /// <param name="userId"></param>
        void InsertAttendanceData(int attendanceList, List<int> listSessionId, int userId);
        /// <summary>
        /// update attendance data
        /// </summary>
        /// <param name="attendanceData"></param>
        void SetAllAttendance(AttendanceData attendanceData);
        /// <summary>
        /// Update attendance list data
        /// </summary>
        /// <param name="listAttendance"></param>
        void UpdateAttendanceData(List<AttendanceData> listAttendance);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<AttendanceData> GetAttendanceDataByIdAndUserId(int id, int userId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="listAttendance"></param>
        /// <returns></returns>
        void SaveAttendanceData(List<AttendanceData> listAttendance);
    }
}