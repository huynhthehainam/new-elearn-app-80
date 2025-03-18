using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.ViewModel.Common
{
    public class RPTUserViewModel : UserViewModel
    {
        public bool IsIGrade { get; set; }
        public bool IsPRGrade { get; set; }

        public RPTUserViewModel(int orgUnitId, string displayName, string orgDefinedId, int roleId, string roleName, int courseOrgUnitId, bool isIGrade, bool isPRGrade)
            : base(orgUnitId, displayName, roleId, roleName, courseOrgUnitId, orgDefinedId)
        {
            IsIGrade = isIGrade;
            IsPRGrade = isPRGrade;
        }
    }
}
