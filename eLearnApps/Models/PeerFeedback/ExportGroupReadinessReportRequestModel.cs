using System.Collections.Generic;

namespace eLearnApps.Models.PeerFeedback
{
    public class ExportGroupReadinessReportRequestModel
    {
        public string TimeZone { get; set; } = "Asia/Singapore";
        public List<string> Terms { get; set; } = new List<string>();
    }
}