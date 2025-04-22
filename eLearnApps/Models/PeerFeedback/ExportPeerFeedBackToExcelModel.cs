
namespace eLearnApps.Models.PeerFeedback
{
    public class ExportPeerFeedBackToExcelModel
    {
        
        public List<int> Sessions { get; set; }

        /// <summary>
        /// 0 - Group by Course/Section, 1 - Group by Student School
        /// </summary>
        
        public PeerFeedBackReportGroupBy GroupBy { get; set; }

        /// <summary>
        /// 0 - Overall Response Rate
        /// 1 - Overall Descriptor Result
        /// 2 - Overall MEAN Score Result
        /// 3 - Individual Student Result
        /// </summary>
        
        public PeerFeedBackReportType ReportType { get; set; }
        public string TimeZone { get; set; } = "Asia/Singapore";
        public string SessionNames { get; set; }
    }
}