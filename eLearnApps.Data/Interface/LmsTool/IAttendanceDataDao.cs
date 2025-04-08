using System.Collections.Generic;
using System.Data;
using eLearnApps.Entity.LmsTools;

namespace eLearnApps.Data.Interface.LmsTool
{
    public interface IAttendanceDataDao
    {
        /// <summary>
        ///     insert list attendance data
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        int Insert(List<AttendanceData> list);
        /// <summary>
        /// update percent list attendance data
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        int Update(List<AttendanceData> list);
        int UpdateForSetAll(AttendanceData attendanceData);
        /// <summary>
        /// Insert or update
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        int Save(List<AttendanceData> list);

        /// <summary>
        /// Bulk insert attendance data
        /// </summary>
        /// <param name="dtSource"></param>
        void InsertAttendanceData(DataTable dtSource);
        /// <summary>
        /// get schema attendance data
        /// </summary>
        /// <returns></returns>
        DataTable GetAttendanceDataSchema();
        /// <summary>
        /// Update attendance data
        /// </summary>
        /// <param name="listAttendanceData"></param>
        void UpdateAttendanceData(List<AttendanceData> listAttendanceData);

        /// <summary>
        /// Soft delete attendancedata whose PK is listed in attendancedataids
        /// </summary>
        /// <param name="AttendanceDataIds">PK of attendance data record to be soft deleted</param>
        /// <returns></returns>
        int SoftDelete(List<int> AttendanceDataIds);
       /// <summary>
       /// 
       /// </summary>
       /// <param name="data"></param>
       /// <returns></returns>
        void Insert(AttendanceData data);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        void Update(AttendanceData data);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        List<AttendanceData> GetBySession(int sessionId);
    }
}