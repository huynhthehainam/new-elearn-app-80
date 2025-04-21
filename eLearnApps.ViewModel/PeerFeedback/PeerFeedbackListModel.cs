using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace eLearnApps.ViewModel.PeerFeedback
{
    public class PeerFeedbackListModel
    {
        public List<PeerFeedbackViewModel> PeerFeedbacks { get; set; }
    }

    public class PeerFeedbackViewModel
    {
        public DateTime EntryStartTime { get; set; }
        public DateTime EntryCloseTime { get; set; }
        public decimal Weight { get; set; }
        public string Label { get; set; }
        public string Name { get; set; }
        public Double Progress { get; set; }
        public int GroupCountComplete { get; set; }
        public int TotalUserCountInGroup { get; set; }
        public int PeerFeedBackPairingId { get; set; }
        public int PeerFeedBackSessionId { get; set; }
        public int PeerFeedBackGroupId { get; set; }
        public int PeerFeedBackId { get; set; }
        public string SessionName { get; set; }
        public string PeerFeedBackKey { get; set; }
    }

    public class PeerFeedbackDetailList
    {
        public int PeerFeedBackPairingId { get; set; }
        public int PeerFeedBackSessionId { get; set; }
        public int PeerFeedBackGroupId { get; set; }
        public int PeerFeedBackId { get; set; }
        public double Progress { get; set; }
        public List<PeerFeedbackDetail> PeerFeedbackDetails { get; set; }
    }
    public class PeerFeedbackDetail
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

    public class PeerFeedbackResponseViewModel
    {
        public int PeerFeedBackId { get; set; }
        public int PeerFeedBackSessionId { get; set; }
        public int PeerFeedBackPairingId { get; set; }
        public int PeerFeedBackGroupId { get; set; }
        public bool Closed { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public double Progress { get; set; }
        public string PeerFeedBackKey { get; set; }
        public List<PeerFeedbackUserModel> Users { get; set; }
        public List<PeerFeedbackQuestionModel> Questions { get; set; }
        public List<PeerFeedBackResponseViewModel> PeerFeedBackResponses { get; set; }
        public List<PeerFeedBackResponseRemarkViewModel> PeerFeedBackResponseRemarks { get; set; }
        public int CourseId { get; set; }
    }
    public class PeerFeedbackUserModel
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
    public class PeerFeedbackQuestionModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<PeerFeedbackRatingQuestionModel> RatingQuestion { get; set; }
        public TextColorValue MedianRating { get; set; }
        public TextColorValue MeanScore { get; set; }
    }

    public class PeerFeedbackQuestionResponseModel
    {
        public List<PeerFeedbackRatingQuestionModel> RatingQuestion { get; set; }
    }

    public class PeerFeedbackRatingQuestionModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Selected { get; set; }
        public List<PeerFeedbackRatingOptionModel> RatingOptions { get; set; }
    }
    public class PeerFeedbackRatingOptionModel
    {
        public int QuestionId { get; set; }
        public int RatingQuestionId { get; set; }
        public int RatingOptionId { get; set; }
        public string OptionName { get; set; }
        public bool Display { get; set; }
        public bool Checked { get; set; }
        public string ColorCode { get; set; }
        public double Progress { get; set; }
        public int ResponseCount { get; set; }
        public bool IsMySelf { get; set; }
        public int StatisticDataType { get; set; }
        public string RatingQuestionName { get; set; }
        public int? DisplayOrder { get; set; }
    }
    public class PeerFeedbackResultModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int PeerFeedBackPairingId { get; set; }
        public int PeerFeedBackSessionId { get; set; }
        public int PeerFeedBackGroupId { get; set; }
        public int PeerFeedBackId { get; set; }
        public bool IsMySelf { get; set; }
        public string CourseName { get; set; }
        public int CourseId { get; set; }
        public string SessionName { get; set; }
        public string PeerFeedBackName { get; set; }
        public string Key { get; set; }
        public string WarningMessage { get; set; }
        public bool DefaultActive { get; set; }
        [Required]
        public PeerFeedbackSessionViewModel Session { get; set; }
    }
    public class PeerFeedbackResultDetailModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Progress { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<PeerFeedbackQuestionModel> Questions { get; set; }
        public List<PeerFeedbackQuestionRatingResultModel> Result { get; set; }
        public List<PeerFeedbackRatingQuestionModel> RatingQuestion { get; set; }
        public List<PeerFeedbackRatingOptionModel> RatingResponse { get; set; }
        
    }

    public class PeerFeedbackQuestionRatingResultModel
    {
        public int Id { get; set; }
        public int RatingOptionId { get; set; }
        public int RatingQuestionId { get; set; }
        public int QuestionId { get; set; }
        public int UserId { get; set; }
        public int CourseId { get; set; }
    }

    public class PeerFeedbackDashboardDetail
    {
        public string ChartId { get; set; }
        public string Label { get; set; }
        public SelectList School { get; set; }
        public SelectList Cohort { get; set; }
        public List<PeerFeedbackRatingQuestionModel> RatingQuestion { get; set; }
        public List<PeerFeedbackRatingOptionModel> RatingResponse { get; set; }
    }
    public class PeerFeedbackDashboard
    {
        public int CourseId { get; set; }
        public string SchoolTermJson { get; set; }
        public IEnumerable<DashBoardTermItem> School { get; set; }
        public IEnumerable<DashBoardTermItem> Cohort { get; set; }
        public List<DashboardDetailParams> PeerFeedBackParamsInResponses { get; set; }
        public List<PeerFeedbackQuestionModel> Questions { get; set; } = new List<PeerFeedbackQuestionModel>();
    }

    public class PeerFeedBackResponsesModel
    {
        public List<PeerFeedBackResponseUserModel> Response { get; set; }
    }
    public class PeerFeedBackResponseUserModel
    {
        public int PeerFeedBackId { get; set; }
        public int PeerFeedBackSessionId { get; set; }
        public int PeerFeedBackPairingId { get; set; }
        public int PeerFeedBackGroupId { get; set; }
        public string PeerFeedBackKey { get; set; }
        public List<PeerFeedBackResponseUserItemModel> Users { get; set; }
    }
    public class PeerFeedBackResponseUserItemModel
    {
        public int UserId { get; set; }
        public List<PeerFeedBackResponseUserQuestionModel> Questions { get; set; }
        public PeerFeedBackResponseRemarkViewModel Remark { get; set; }

    }
    public class PeerFeedBackResponseUserQuestionModel
    {
        public int QuestionId { get; set; }
        public List<PeerFeedBackResponseUserQuestionRatingModel> Ratings { get; set; }
    }

    public class PeerFeedBackResponseUserQuestionRatingModel
    {
        public int RatingId { get; set; }
        public List<PeerFeedBackResponseUserQuestionOptionModel> Options { get; set; }
    }
    public class PeerFeedBackResponseUserQuestionOptionModel
    {
        public int OptionId { get; set; }
        public bool Checked { get; set; }
    }

    public class PeerFeedBackResponseViewModel
    {
        public int Id { get; set; }
        public int PeerFeedbackId { get; set; }
        public int PeerFeedbackQuestionId { get; set; }
        public int PeerFeedbackSessionId { get; set; }
        public int TargetUserId { get; set; }
        public int EvaluatorUserId { get; set; }
        public DateTime LastUpdateTime { get; set; }
        public int? PeerFeedBackRatingId { get; set; }
        public int? PeerFeedBackOptionId { get; set; }
        
    }
    public class PeerFeedBackResponseRemarkViewModel
    {
        public int Id { get; set; }
        public int PeerFeedbackId { get; set; }
        public int PeerFeedbackSessionId { get; set; }
        public int TargetUserId { get; set; }
        public int EvaluatorUserId { get; set; }
        public DateTime LastUpdateTime { get; set; }
        public bool IsDeleted { get; set; }
        public int PeerFeedBackGroupId { get; set; }
        public string Remarks { get; set; }

    }

    public class PeerFeedBackResultDetailViewModel
    {
        public string Key { get; set; }
        public int GroupBy { get; set; }
        public int PeerFeedBackId { get; set; }
        public int PeerFeedBackGroupId { get; set; }
        public int PeerFeedBackSessionId { get; set; }
        public int PeerFeedBackQuestionId { get; set; }
        public int PeerFeedBackPairingId { get; set; }
        public double Progress { get; set; }
        [Required]
        public PeerFeedbackSessionViewModel Session { get; set; }
        public List<PeerFeedbackQuestionModel> Questions { get; set; }
        public List<PeerFeedbackResultModel> PeerFeedbackResultModelList { get; set; }
    }
    public class PeerFeedBackResultDetailQuestionStatisticViewModel
    {
        public string ChartId { get; set; }
        public string Label { get; set; }
        public string ChartData { get; set; }
        public string QuestionTitle { get; set; }
        public string ResourceId { get; set; }
        public int CountUserComplete { get; set; }
        public int TotalUserInGroup { get; set; }
        public int GroupBy { get; set; }
        public List<PeerFeedbackRatingQuestionModel> RatingQuestion { get; set; }
        public List<PeerFeedbackRatingOptionModel> RatingResponse { get; set; }
        public TextColorValue SelfValue { get; set; }
        public List<PeerFeedbackQuestionModel> Questions { get; set; } = new List<PeerFeedbackQuestionModel>();
    }

    public class PeerFeedBackChartItem
    {
        public List<string> Labels { get; set; }
        public List<int> Data { get; set; }
        public List<string> BackgroundColor { get; set; }
    }
    public class DashboardDetailParams
    {
        public int PeerFeedBackId { get; set; }
        public int PeerFeedBackSessionId { get; set; }
        public int? PeerFeedBackPairingId { get; set; }
        public int PeerFeedBackGroupId { get; set; }
        public int PeerFeedBackQuestion { get; set; }
    }
    public class SelfDirectedLearningResourcesModel
    {
        public string ResourceId { get; set; }
        public string QuestionName { get; set; }
        public int ItemType { get; set; }
        public List<SelfDirectedLearningResourcesItemTemplate> ItemTemplates { get; set; }
    }
    public class SelfDirectedLearningResourcesItemTemplate
    {
        public string Title { get; set; }
        public string Label { get; set; }
        public List<SelfDirectedLearningResourcesItemTemplateLink> SelfDirectedLearningResourceLink { get; set; }
    }
    public class SelfDirectedLearningResourcesItemTemplateLink
    {
        public string Text { get; set; }
        public string Href { get; set; }
    }
    public class TextColorValue
    {
        public string Text { get; set; }
        public string ColorCode { get; set; }
    }
    public class DashBoardSchoolItem
    {
        public string Text { get; set; }
        public string Value { get; set; }
        public List<DashBoardTermItem> Terms { get; set; }

    }
    public class DashBoardTermItem
    {
        public string Text { get; set; }
        public string Value { get; set; }
        public List<string> Courses { get; set; } = new List<string>();
    }
    public class PeerFeedBackResponseGroupByModel
    {
        public int PeerFeedbackId { get; set; }
        public int PeerFeedbackSessionId { get; set; }
        public int PeerFeedBackGroupId { get; set; }
    }
}
