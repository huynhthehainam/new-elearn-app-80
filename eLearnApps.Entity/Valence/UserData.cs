using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.Entity.Valence
{
    public class UserData
    {
        public long OrgId { get; set; }
        public long UserId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string ExternalEmail { get; set; }
        public string OrgDefinedId { get; set; }
        public string UniqueIdentifier { get; set; }
        public UserActivationData Activation { get; set; }
        public DateTime LastAccessedDate { get; set; }
    }

    public class UserActivationData
    {
        public bool IsActive { get; set; }
    }
}
