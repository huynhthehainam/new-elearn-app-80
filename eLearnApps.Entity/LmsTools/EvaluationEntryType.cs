namespace eLearnApps.Entity.LmsTools
{
    public class EvaluationEntryType : BaseEntity
    {
        public int EvaluationEntryTypeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}