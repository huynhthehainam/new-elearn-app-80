using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using eLearnApps.Entity.LmsTools;

namespace eLearnApps.Data.Interface.LmsTool
{
    public interface IIcsSessionDao
    {
        /// <summary>
        ///     get Ics session by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ICSSession GetById(int id);

        /// <summary>
        ///     Get list session by course
        /// </summary>
        /// <param name="courseId"></param>
        /// <returns></returns>
        List<ICSSession> GetByCourse(int courseId);

        /// <summary>
        ///     Check user can delete session
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        int CanDeleteSession(int sessionId);

        /// <summary>
        ///     Check exists ics session between start time - end time
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="sessionDate"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        List<ICSSession> CheckRangeStartTimeEndTime(int courseId, DateTime sessionDate, TimeSpan startTime, TimeSpan endTime);
    }
}