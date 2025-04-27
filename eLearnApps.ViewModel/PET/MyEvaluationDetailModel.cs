using System.Collections.Generic;

namespace eLearnApps.ViewModel.PET
{
    public class MyEvaluationDetailModel
    {
        public EvaluationModel Evaluation { get; set; }
        public EvaluationEntrySessionModel EvaluationSession { get; set; }
        public List<MyEvaluationSessionViewModel> ListUser { get; set; }
        public List<MyEvaluationSessionViewModel> ListUserGroupSection { get; set; }
        public string EvaluationKey { get; set; }
        public string EvaluationSessionKey { get; set; }
        public int EvaluationPairingId { get; set; }
        public string EvaluationPairingKey { get; set; }
    }

    public class MyEvaluationSessionViewModel
    {
        public int UserId { get; set; }
        public string DisplayName { get; set; }
        public string EmailAddress { get; set; }
        public string OrgDefinedId { get; set; }
        public string GroupName { get; set; }
        public string SectionName { get; set; }
        public string Avatar { get; set; }
        public int GroupId { get; set; }
    }
}