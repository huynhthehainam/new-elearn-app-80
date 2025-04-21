using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.Entity.LmsTools.Dto
{
    public class PeerFeedbackRatingQuestionMapOptionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<PeerFeedbackRatingOptionDto> Options { get; set; }

    }

    public class PeerFeedbackRatingOptionDto
    {
        public int RatingOptionId { get; set; }
        public int RatingQuestionId { get; set; }
        public int QuestionId { get; set; }
        public string OptionName { get; set; }
    }
}
