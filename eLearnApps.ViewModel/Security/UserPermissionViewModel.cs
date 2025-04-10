using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.ViewModel.Security
{
    public class UserEnrollmentViewModel
    {
        public int UserId { get; set; }
        public int CourseId { get; set; }
        public int RoleId { get; set; }
    }

    public class RolePermissionViewModel
    {
        public int RoleId { get; set; }
        public int Id { get; set; }
        public string PermissionName { get; set; }
        public string PermissionSystemName { get; set; }
        public string PermissionUrl { get; set; }
        public string PermissionCategory { get; set; }
        public int PermissionOrder { get; set; }
    }
}

