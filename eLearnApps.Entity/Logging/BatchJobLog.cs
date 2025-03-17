using System;

namespace eLearnApps.Entity.Logging
{
    public class BatchJobLog
    {
        public long Id { get; set; }
        public string ExecutableName { get; set; }
        public string Parameters { get; set; }
        public DateTime JobStart { get; set; }
        public DateTime? JobEnd { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastUpdatedOn { get; set; }
    }
}