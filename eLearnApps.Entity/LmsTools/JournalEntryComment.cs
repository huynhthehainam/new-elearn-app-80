

using eLearnApps.Core;
using System;

namespace eLearnApps.Entity.LmsTools
{
    public class JournalEntryComment : BaseEntity
    {
        public int Id { get; set; }
        public int JournalEntryId { get; set; }
        public int UserId { get; set; }
        public string Comment { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public RecordStatus Status { get; set; }
        public DateTime LastUpdatedTime { get; set; }
        public int LastUpdatedBy { get; set; }
    }
}