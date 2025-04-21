
using System;

namespace eLearnApps.Entity.LmsTools.Dto
{
    public class PeerFeedBackDashboardExportCsvDto
    {
        public string PeerSelfValue { get; set; }
        public string CourseName { get; set; }
        public int CourseId { get; set; }
        public string GroupName { get; set; }
        public string EvaluatorName { get; set; }
        public string EvaluatorCampusId { get; set; }
        public string TargetName { get; set; }
        public string TargetCampusId { get; set; }
        public string Question { get; set; }
        public string Rating { get; set; }
        public string SelectedOption { get; set; }
        public DateTime UpdatedDateTime { get; set; }
        public int TargetUserId { get; set; }
        public int EvaluatorUserId { get; set; }
        public int? PeerFeedBackRatingId { get; set; }
        public int? PeerFeedBackOptionId { get; set; }
        public int PeerFeedbackQuestionId { get; set; }
    }
}
