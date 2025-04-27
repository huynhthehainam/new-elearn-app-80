namespace eLearnApps.ViewModel.PeerFeedback
{
    public class AuditActionModel
    {
        public string Question { get; set; }
        public AuditResourceId ResourceId { get; set; }
        public int CourseId { get; set; }
    }
}