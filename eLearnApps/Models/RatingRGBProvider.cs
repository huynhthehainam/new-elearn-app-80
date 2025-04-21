using eLearnApps.Entity.LmsTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eLearnApps.Models
{
    public class RatingRGBProvider
    {
        private Dictionary<int, string>  _dicColorCode;

        public RatingRGBProvider(List<PeerFeedbackRatingQuestion> ratingQuestions)
        {
            _dicColorCode = new Dictionary<int, string>();

            if (ratingQuestions != null)
            {
                string colorCode = string.Empty;
                foreach (var ratingQuestion in ratingQuestions)
                {
                    switch (ratingQuestion.Id)
                    {
                        case 1:
                            colorCode = "rgb(128, 0, 0)"; // RED
                            break;
                        case 2:
                            colorCode = "rgb(14,38,105)"; // BLUE
                            break;
                        case 3:
                            colorCode = "rgb(0, 128, 0)"; // GREEN
                            break;
                        default:
                            var rnd = new Random();
                            colorCode = $"rgb({rnd.Next(0, 255)}, {rnd.Next(0, 255)}, {rnd.Next(0, 255)})"; // random color
                            break;
                    }

                    _dicColorCode.Add(ratingQuestion.Id, colorCode);
                }
            }
        }

        /// <summary>
        /// Get predefined color assigned for each ratingId.
        /// If ratingId cannot be found in setting, random color is returned
        /// </summary>
        /// <param name="ratingId">RatingId to get color codes</param>
        /// <returns>RGB in String represetation</returns>
        public string GetRatingColorCodes(int ratingId)
        {
            var colorCode = string.Empty;
            if (_dicColorCode.ContainsKey(ratingId))
            {
                colorCode = _dicColorCode[ratingId];
            } 
            else
            {
                var rnd = new Random();
                colorCode = $"rgb({rnd.Next(0, 255)}, {rnd.Next(0, 255)}, {rnd.Next(0, 255)})"; // random color
            }
            return colorCode;
        }
    }
}