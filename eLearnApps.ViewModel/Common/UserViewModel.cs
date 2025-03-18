using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.ViewModel.Common
{
    public class UserViewModel
    {
        public UserViewModel(int orgUnitId, string displayName, int roleId, string roleName, int courseOrgUnitId, string orgDefinedId)
        {
            UserOrgUnitId = orgUnitId;
            DisplayName = displayName;
            UserRole = new RoleViewModel()
            {
                RoleId = roleId,
                RoleName = roleName,
                CourseId = courseOrgUnitId
            };
            OrgDefinedId = orgDefinedId;
            SectionName = string.Empty;
        }

        public int UserOrgUnitId { get; set; }

        public string DisplayName { get; set; }

        public RoleViewModel UserRole { get; set; }
        public string SectionName { get; set; }
        public string OrgDefinedId { get; set; }

        public string UserKey 
        { 
            get 
            {
                return Core.Cryptography.AesEncrypt.Encrypt(UserOrgUnitId.ToString());
            }
        }
    }
}
