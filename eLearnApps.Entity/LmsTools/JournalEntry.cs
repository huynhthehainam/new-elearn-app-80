using System;
using eLearnApps.Core;

namespace eLearnApps.Entity.LmsTools
{
    public class JournalEntry : BaseEntity
    {
        public int Id { get; set; }
        public int JournalId { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public bool IsDraft { get; set; }
        public bool IsRead { get; set; }
        public RecordStatus Status { get; set; }
        public DateTime LastUpdatedTime { get; set; }
        public int LastUpdatedBy { get; set; }
    }
}