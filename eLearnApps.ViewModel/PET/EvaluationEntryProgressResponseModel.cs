using System.Collections.Generic;
using eLearnApps.Core.Cryptography;

namespace eLearnApps.ViewModel.PET
{
    public class EvaluationEntryProgressResponseModel
    {
        public int UserId { get; set; }
        public EvaluationModel Evaluation { get; set; }
        public List<EvaluationSessionModel> EvaluationSession { get; set; }
        public List<EvaluationQuestionModel> EvaluationQuestion { get; set; }
        public int? EvaluationParringId { get; set; }
        public int OrgUnitId { get; set; }
        public string UserKey { get; set; }
    }
}