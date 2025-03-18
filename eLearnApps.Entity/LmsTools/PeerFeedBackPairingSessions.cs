namespace eLearnApps.Entity.LmsTools
{
    public class PeerFeedBackPairingSessions : BaseEntity
    {
        public int Id { get; set; }
        public int PeerFeedBackPairingId { get; set; }
        public int PeerFeedBackSessionId { get; set; }
    }
}