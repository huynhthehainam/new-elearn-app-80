namespace eLearnApps.ViewModel.PeerFeedback
{
    
    public class RatingQuestionModel
    {
        public int? Id { get; set; }
        
        public string Name { get; set; }
        public int? DisplayOrder { get; set; }
    }

    
    public class RatingAnswerModel
    {
        public int? Id { get; set; }
        
        public string Name { get; set; }
    }
}