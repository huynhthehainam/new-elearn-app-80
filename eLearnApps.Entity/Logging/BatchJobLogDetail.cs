using System;

namespace eLearnApps.Entity.Logging
{
    public class BatchJobLogDetail
    {
        public long Id { get; set; }
        public long BatchJobLogId { get; set; }
        public string? LogLevel { get; set; }
        public string? LogContent { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}