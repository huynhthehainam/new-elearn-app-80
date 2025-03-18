using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.Entity.LmsIsis
{
    public class StudentPhoto : BaseEntity
    {
        public string PersonId { get; set; }
        public string IdCardNo { get; set; }
        public byte[] Photo { get; set; }
    }
}
