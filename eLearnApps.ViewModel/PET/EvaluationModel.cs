using Microsoft.AspNetCore.Mvc.Rendering;


namespace eLearnApps.ViewModel.PET
{
    public class EvaluationModel
    {
        public int EvaluationId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int EvaluationTypeId { get; set; }
        public int SourceEvaluationId { get; set; }
        public bool AllowSelfEvaluate { get; set; }
        public int TargetTypeId { get; set; }
        public int EvaluatorTypeId { get; set; }
        public bool IsInstructorEvaluator { get; set; }
        public int? InstructorWeight { get; set; }
        public decimal QuestionScoreCalculation { get; set; }
        public int SessionScoreCalculation { get; set; }
        public bool AllowEvaluation { get; set; }
        public bool IsDeleted { get; set; }
        public string EvaluationKey { get; set; }
        public List<EvaluableItemModel> EvaluableItem { get; set; }
        public string UserData { get; set; }
        public List<EvaluationEntryProgressResponseModel> ProgressResponseData { get; set; }
        public List<EvaluationQuestionModel> EvaluationQuestion { get; set; }
        public int TotalUserResponse { get; set; }
        public string OverallProgress { get; set; }
        public bool AllowDisplayGroupByQuestion { get; set; }
        public List<Tuple<int, string, List<Tuple<EvaluationSessionModel, List<EvaluationSessionUserPercentModel>>>, List<Tuple<EvaluationSessionModel, List<EvaluationQuestionModel>>>>> Response { get; set; }

    }
    public class EvaluationViewModel
    {
        public EvaluationViewModel()
        {
            AvailableTypes = new List<EvaluationTypeViewModel>();
            AvailableInstructorWeights = new List<SelectListItem>();
            AvailableScoreCalculations = new List<ScoreCalculationTypeViewModel>();
        }
        public string Name { get; set; }
        public string Description { get; set; }
        public int EvaluationTypeId { get; set; }
        public int SourceEvaluationId { get; set; }
        public bool AllowSelfEvaluate { get; set; }
        public int TargetTypeId { get; set; }
        public int EvaluatorTypeId { get; set; }
        public bool IsInstructorEvaluator { get; set; }
        public int? InstructorWeight { get; set; }
        public decimal QuestionScoreCalculation { get; set; }
        public int SessionScoreCalculation { get; set; }
        public bool AllowEvaluation { get; set; }
        public bool IsDeleted { get; set; }
        public string EvaluationKey { get; set; }
        public IList<EvaluationTypeViewModel> AvailableTypes { get; set; }
        public IList<SelectListItem> AvailableInstructorWeights { get; set; }
        public IList<ScoreCalculationTypeViewModel> AvailableScoreCalculations { get; set; }
    }
    public class EvaluationTypeViewModel
    {
        public int EvaluationTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
    public class ScoreCalculationTypeViewModel
    {
        public int ScoreCalculationTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}