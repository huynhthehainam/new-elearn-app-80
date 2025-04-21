using System.Collections.Generic;

namespace eLearnApps.Entity.LmsTools.Dto
{
    public class SeedData
    {
        public List<PeerFeedbackRatingQuestion> PeerFeedbackRatingQuestions { get; set; }
        public List<PeerFeedbackRatingOption> PeerFeedbackRatingOptions { get; set; }
        public List<PeerFeedbackQuestion> PeerFeedbackQuestions { get; set; }
        public List<PeerFeedbackQuestionRatingMap> PeerFeedbackQuestionRatingMaps { get; set; }
    }
}
