using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace eLearnApps.ViewModel.ICS
{
    public class QuestionViewModel
    {
        public int Id { get; set; }

        public int SessionId { get; set; }

        public int UserId { get; set; }

        [DisplayName("Question")] public string Content { get; set; }

        public string SessionKey { get; set; }
        public int Index { get; set; }
        public bool? Addressed { get; set; }
        public string QuestionKey { get; set; }
    }

    public class QuestionModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string QuestionKey { get; set; }
        [Required]
        public int SessionId { get; set; }
        [Required]
        public string SessionKey { get; set; }
        public bool? Addressed { get; set; }
    }
}