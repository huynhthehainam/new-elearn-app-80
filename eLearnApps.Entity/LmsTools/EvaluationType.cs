namespace eLearnApps.Entity.LmsTools
{
    public class EvaluationType : BaseEntity
    {
        public int EvaluationTypeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}