namespace eLearnApps.Models.PeerFeedback
{
    public class StudentPeerFeedbackViewModel
    {
        public UserModel UserInfo { get; set; }
        public bool HasClosedSession { get; set; }
        public bool IsInitialLanding { get; set; }
    }
    public class PeerFeedBackResponseEvaluatorTargetModel
    {
        public int EvaluatorUserId { get; set; }
        public int TargetUserId { get; set; }
    }
    public class PeerFeedBackSessionGroupModel
    {
        public int PeerFeedBackId { get; set; }
        public int PeerFeedBackSessionId { get; set; }
        public int PeerFeedBackGroupId { get; set; }
        public int PeerFeedBackQuestionId { get; set; }
        public int EvaluatorUserId { get; set; }
    }
}