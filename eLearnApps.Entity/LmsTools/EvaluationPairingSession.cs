namespace eLearnApps.Entity.LmsTools
{
    public class EvaluationPairingSession : BaseEntity
    {
        public int EvaluationPairingSessionId { get; set; }
        public int EvaluationPairingId { get; set; }
        public int EvaluationSessionId { get; set; }
    }
}