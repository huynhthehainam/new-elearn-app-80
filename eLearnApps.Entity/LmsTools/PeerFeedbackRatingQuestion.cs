namespace eLearnApps.Entity.LmsTools
{
    public class PeerFeedbackRatingQuestion : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? DisplayOrder { get; set; }
        public bool Deleted { get; set; }
    }

    public class PeerFeedbackRatingOption : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool Deleted { get; set; }
    }

    public class PeerFeedbackRatingMap : BaseEntity
    {
        public int Id { get; set; }
        public int RatingOptionId { get; set; }
        public int RatingQuestionId { get; set; }
    }

    public class PeerFeedbackQuestion : BaseEntity
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool Deleted { get; set; }
    }

    public class PeerFeedbackQuestionRatingMap : BaseEntity
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public int RatingOptionId { get; set; }
        public int RatingQuestionId { get; set; }
    }
}