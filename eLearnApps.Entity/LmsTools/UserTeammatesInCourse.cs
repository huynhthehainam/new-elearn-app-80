using System;

namespace eLearnApps.Entity.LmsTools
{
    public class UserTeammatesInCourse : BaseEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CourseId { get; set; }
        public int TeammateId { get; set; }
    }
}

