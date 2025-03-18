using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.ViewModel.Common
{
    /// <summary>
    /// a user can have different role in different course.
    /// e.g. a student is enrolled in 8 courses, but in 2 courses, he is doing exceptionally well, he is promoted to TA role.
    /// </summary>
    public class RoleViewModel
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public int CourseId { get; set; }
    }
}
