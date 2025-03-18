using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.Entity.LmsTools.Dto
{
    public class UserEnrollmentDto
    {
        public int UserId { get; set; }
        public int SemesterId { get; set; }
        public int CourseId { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public bool IsClassList { get; set; }
    }
}
