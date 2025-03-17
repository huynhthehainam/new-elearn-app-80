using eLearnApps.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace eLearnApps.Entity.Logging
{
    public class AuditEntry
    {
        public AuditEntry()
        {
        }
        public string? ToolId { get; set; }
        public string? UserId { get; set; }
        public int OrgUnitId { get; set; }
        public int ResourceId { get; set; }
        public string? TableName { get; set; }
        public Dictionary<string, object> KeyValues { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> OldValues { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> NewValues { get; } = new Dictionary<string, object>();
        public AuditType AuditType { get; set; }
        public List<string> ChangedColumns { get; } = new List<string>();
        public Audit ToAudit()
        {
            var audit = new Audit();
            audit.ToolId = ToolId;
            audit.UserId = UserId;
            audit.OrgUnitId = OrgUnitId;
            audit.Type = AuditType.ToString();
            audit.ResourceId = ResourceId;
            audit.TableName = TableName;
            audit.DateTime = DateTime.Now;
            audit.PrimaryKeys = JsonSerializer.Serialize(KeyValues);
            audit.OldValues = OldValues.Count == 0 ? null : JsonSerializer.Serialize(OldValues);
            audit.NewValues = NewValues.Count == 0 ? null : JsonSerializer.Serialize(NewValues);
            audit.AffectedColumns = ChangedColumns.Count == 0 ? null : JsonSerializer.Serialize(ChangedColumns);
            return audit;
        }
    }
}
