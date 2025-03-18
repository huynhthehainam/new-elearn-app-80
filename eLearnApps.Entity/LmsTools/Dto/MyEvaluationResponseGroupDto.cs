namespace eLearnApps.Entity.LmsTools.Dto
{
    public class MyEvaluationResponseGroupDto : BaseEntity
    {
        public string Text { get; set; }
        public int Value { get; set; }
        public int EvaluationPairingId { get; set; }
        public int UserId { get; set; }
    }
}