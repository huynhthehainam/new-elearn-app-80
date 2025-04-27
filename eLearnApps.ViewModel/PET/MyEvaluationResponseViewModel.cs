using System.Collections.Generic;

namespace eLearnApps.ViewModel.PET
{
    public class MyEvaluationResponseViewModel
    {
        public int CourseId { get; set; }
        public string EvaluationKey { get; set; }
        public string EvaluationSessionKey { get; set; }
        public bool IsTargetGroup { get; set; }
        public List<int> GroupId { get; set; }
        public List<int> UserIds { get; set; }
        public List<EvaluationQuestionModel> EvaluationQuestion { get; set; }
        public EvaluationModel Evaluation { get; set; }
        public EvaluationEntrySessionModel EvaluationSession { get; set; }
        public List<MyEvaluationSessionViewModel> ListUser { get; set; }
        public List<EvaluationResponseUserModel> ListResponse { get; set; }
        public List<DefaultViewModel> ListGroup { get; set; }
        public bool IsClosed { get; set; }
        public string EvaluationPairingKey { get; set; }
        public bool? IsReadOnly { get; set; }
    }

    public class DefaultViewModel
    {
        public string Text { get; set; }
        public int Value { get; set; }
    }
}