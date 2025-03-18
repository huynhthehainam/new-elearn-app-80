using System;

namespace eLearnApps.Entity.LmsTools
{
    public class TAEnrollAck
    {
        public int TAEnrollAckId { get; set; }
        public int CourseId { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public int AckBy { get; set; }
        public DateTime AckDateTime { get; set; }

        public TAEnrollAck (int courseId, int userId, int roleId, int ackBy, DateTime ackDateTime)
        {
            TAEnrollAckId = -1;
            CourseId = courseId;
            UserId = userId;
            RoleId = roleId;
            AckBy = ackBy;
            AckDateTime = ackDateTime;
        }

    }
}