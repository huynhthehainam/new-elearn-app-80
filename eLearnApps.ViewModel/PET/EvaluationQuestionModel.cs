using System;
using System.Collections.Generic;

namespace eLearnApps.ViewModel.PET
{
    public class EvaluationQuestionModel
    {
        public long EvaluationQuestionId { get; set; }
        public int EvaluationId { get; set; }
        public string Name { get; set; }
        public string Question { get; set; }
        public decimal Weight { get; set; }
        public int EntryTypeId { get; set; }
        public string EntryType { get; set; }
        public decimal MinMarks { get; set; }
        public decimal MaxMarks { get; set; }
        public decimal MarksInterval { get; set; }
        public int MinSelection { get; set; }
        public int MaxSelection { get; set; }
        public bool AllowComments { get; set; }
        public bool AllowDecimalMarks { get; set; }
        public bool IsDeleted { get; set; }
        public int RankMarkLabelId { get; set; }
        public decimal? RankMarkLabelInterval { get; set; }
        public string RankMarkLabelName { get; set; }
        public int LastUpdatedBy { get; set; }
        public int QuestionScoreCalculation { get; set; }
        public DateTime LastUpdatedTime { get; set; }
        public decimal? AveragedMark { get; set; }
        public decimal TotalAvg { get; set; }
        public List<MarkRankLabelDetailModel> ListMarkRankLabelDetailModel { get; set; }
    }
}