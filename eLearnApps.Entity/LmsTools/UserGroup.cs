using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.Entity.LmsTools
{
    public class UserGroup : BaseEntity
    {
        public int Id { get; set; }
        public int? CategoryGroupId { get; set; }
        public int? UserId { get; set; }
    }
}
