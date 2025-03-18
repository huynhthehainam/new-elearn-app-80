namespace eLearnApps.Entity.LmsTools
{
    public class MarkRankLabelDetail : BaseEntity
    {
        public int Id { get; set; }
        public int MarkRankLabelId { get; set; }
        public decimal MarkNumber { get; set; }
        public string Label { get; set; } = string.Empty;
    }
}