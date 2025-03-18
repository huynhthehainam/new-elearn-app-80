

using eLearnApps.Core;
using System;

namespace eLearnApps.Entity.LmsTools
{
    public class Journal : BaseEntity
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public RecordStatus Status { get; set; }
        public DateTime LastUpdatedTime { get; set; }
        public int LastUpdatedBy { get; set; }
    }
}
