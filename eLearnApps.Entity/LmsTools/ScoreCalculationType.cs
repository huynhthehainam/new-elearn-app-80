namespace eLearnApps.Entity.LmsTools
{
    public class ScoreCalculationType : BaseEntity
    {
        public int ScoreCalculationTypeId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}