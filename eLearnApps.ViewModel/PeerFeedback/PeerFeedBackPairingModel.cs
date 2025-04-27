using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace eLearnApps.ViewModel.PeerFeedback
{
    public class PeerFeedBackPairingModel
    {
        public int PeerFeedBackPairingId { get; set; }
        public int PeerFeedBackId { get; set; }
        public int PeerFeedBackSessionId { get; set; }
        public List<Item> Evaluators { get; set; }
        public List<Item> Targets { get; set; }
        public List<PeerFeedbackSessionViewModel> Sessions { get; set; }
        public ICollection<string> Evaluator { get; set; }
        public ICollection<string> Target { get; set; }
        public List<int> TargetsId { get; set; }
    }

    public class Item
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public string Description { set; get; }
        public int Type { set; get; }
        public int GroupId { get; set; }
    }

    public class PeerFeedBackSessionsPairingListModel
    {
        public int PeerFeedBackId { get; set; }
        public List<PeerFeedBackSessionsPairingModel> SessionPairings { get; set; }
    }

    public class PeerFeedBackSessionsPairingModel
    {
        public List<Item> Evaluators { get; set; }
        public List<Item> Targets { get; set; }
        [Required]
        public PeerFeedbackSessionViewModel Session { get; set; }
        public ICollection<string> Evaluator { get; set; }
        public ICollection<string> Target { get; set; }
    }

    public class EvaluatorTargetModel
    {
        public string Evaluator { get; set; }
        public string Target { get; set; }
        public int PeerFeedBackId { get; set; }
        public int PeerFeedBackPairingId { get; set; }
        public int PeerFeedBackSessionId { get; set; }
        public int TypeId { get; set; }
        public bool IsCourseGroup { get; set; }
        public int CurrentCourseGroup { get; set; }
    }
    public class DataSourceEvaluatorTargetModel
    {
        public string DataSourceEvaluator { get; set; }
        public string DataSourceTarget { get; set; }
    }
}