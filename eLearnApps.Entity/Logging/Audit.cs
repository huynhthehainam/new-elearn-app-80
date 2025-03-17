using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.Entity.Logging
{
    public class Audit : BaseEntity
    {
        public int Id { get; set; }
        public string? ToolId { get; set; }
        public string? UserId { get; set; }
        public int OrgUnitId { get; set; }
        public string? Type { get; set; }
        public int ResourceId { get; set; }
        public string? TableName { get; set; }
        public DateTime? DateTime { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public string? AffectedColumns { get; set; }
        public string? PrimaryKeys { get; set; }
    }
}
