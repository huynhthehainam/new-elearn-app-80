using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.Entity.Valence
{
    public class ClasslistUser
    {
        public string Identifier { get; set; }
        public string ProfileIdentifier { get; set; }
        public string DisplayName { get; set; }
        public string UserName { get; set; }
        public string OrgDefinedId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int RoleId { get; set; }
    }
}
