using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.Entity.Logging
{
    public class GPTAuditLog
    {
        public int Id { get; set; }
        public string Action { get; set; }
        public int ActionByUserId { get; set; }
        public string ActionByName { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public DateTime ActionTime { get; set; }
        public string IpAddress { get; set; }
        public string SessionId { get; set; }
    }

}
