using Microsoft.AspNetCore.Mvc.Rendering;

namespace eLearnApps.ViewModel.EE
{
    public class CreateExtractionViewModel
    {
        public int CourseId { get; set; }
        public string? QuizId { get; set; }
        public int? SectionId { get; set; }
        public string? SectionName { get; set; }
        public string? FontSize { get; set; }
        public string? LineSpace { get; set; }

        public string? ClientTimezone { get; set; }

        public bool IsStudentNameShown { get; set; }

        public bool IsQuestionShown { get; set; }

        public List<SelectListItem> Quizzes { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Sections { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> FontSizes { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> LineSpaces { get; set; } = new List<SelectListItem>();
        public int SortBy { get; set; }
        public int GroupBy { get; set; }
        public int ExportOption { get; set; }
        public int AdditionalExportOptions { get; set; }
        public bool EnableGPTZeroOption { get; set; }
        public List<QuestionTypeViewModel> QuestionTypes { get; set; }
    }
}