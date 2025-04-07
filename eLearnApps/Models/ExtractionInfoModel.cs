using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eLearnApps.Models
{
    public class ExtractionInfoModel
    {
        public string CourseCode { get; set; }
        public int CourseId { get; set; }
        public int? SectionId { get; set; }
        public string SectionName { get; set; }
        public int QuizId { get; set; }
        public string QuizName { get; set; }
        public bool IsStudentNameShown { get; set; }
        public bool IsQuestionShown { get; set; }
        public string FontSize { get; set; }
        public string LineSpacing { get; set; }
        public string ClientTimezone { get; set; }
        public List<ViewModel.Valence.SectionData> Sections { get; set; }
        public bool IsOutputZipped { get; set; } = false;
        public List<eLearnApps.Entity.Valence.User> EnrolledUsers { get; set; }
        public int SortBy { get; set; }
        public int GroupBy { get; set; }
        public int? ExportOption { get; set; }
        public string GptZeroTitle { get; set; }
        public List<int> GtpZeroTitleCondition { get; set; } = new List<int>();
        public List<string> QuestionTypes { get; set; }
    }
}