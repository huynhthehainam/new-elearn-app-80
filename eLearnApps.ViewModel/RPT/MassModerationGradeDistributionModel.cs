using eLearnApps.Core;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace eLearnApps.ViewModel.RPT
{
    public class MassModerationRequestViewModel
    {
        [Range(0, 6)]
        public int ModerationTypeId { get; set; }

        [Range(0, 100)]
        public double? From { get; set; }

        [Range(0, 100)]
        public double? To { get; set; }

        [Range(-100, 100, ErrorMessage = "Add/Subtract value between from -100 to 100.")]
        public double AdjustMarks { get; set; }

        public double? Moderate(UserGradeObjectViewModel grade, GradeSchemeViewModel gradeScheme)
        {
            if (grade == null)
            {
                return null;
            }
            else
            {

                bool needToModerte = false;
                switch (ModerationTypeId)
                {
                    case (int)Core.GradeModerationType.MarkBetween:
                        needToModerte = (grade.PointsNumerator >= From && grade.PointsNumerator <= To);
                        break;

                    case (int)Core.GradeModerationType.MarkEqualTo:
                        needToModerte = grade.PointsNumerator == From;
                        break;

                    case (int)Core.GradeModerationType.MarkGreaterThan:
                        needToModerte = grade.PointsNumerator > From;
                        break;

                    case (int)Core.GradeModerationType.MarkGreaterThanOrEqualTo:
                        needToModerte = grade.PointsNumerator >= From;
                        break;

                    case (int)Core.GradeModerationType.MarkLessThan:
                        needToModerte = grade.PointsNumerator < From;
                        break;

                    case (int)Core.GradeModerationType.MarkLessThanOrEqualTo:
                        needToModerte = grade.PointsNumerator <= From;
                        break;

                    default:
                        needToModerte = true;
                        break;

                }

                if (needToModerte)
                {
                    var moderatedMark = grade.PointsNumerator + AdjustMarks;

                    // moderated cannot goes beyond gradeschme min/max
                    if (moderatedMark < gradeScheme.MinMark) moderatedMark = gradeScheme.MinMark;
                    else if (moderatedMark > gradeScheme.MaxMark) moderatedMark = gradeScheme.MaxMark;

                    return moderatedMark;
                }
                else return grade.PointsNumerator;
            }
        }
    }

    public class MassModerationGradeDistributionViewModel
    {
        public string Grade { set; get; }
        public string Marks { set; get; }
        public string Color { set; get; }

        [JsonIgnore]
        public int OriginalCountValue { get; set; }
        [JsonIgnore]
        public double OriginalPercentageValue { get; set; }
        [JsonIgnore]
        public double OriginalAccumValue { set; get; }

        [JsonIgnore]
        public int ModeratedCountValue { get; set; }
        [JsonIgnore]
        public double ModeratedPercentageValue { get; set; }
        [JsonIgnore]
        public double ModeratedAccumValue { set; get; }

        public string OriginalCount { get { return OriginalCountValue.ToString(); } }
        public string OriginalPercentage { get { return OriginalPercentageValue.ToString("0.#") + "%"; } }
        public string OriginalAccum { get { return OriginalAccumValue.ToString("0.#") + "%"; } }

        public string ModeratedCount { get { return ModeratedCountValue.ToString(); } }
        public string ModeratedPercentage { get { return ModeratedPercentageValue.ToString("0.#") + "%"; } }
        public string ModeratedAccum { get { return ModeratedAccumValue.ToString("0.#") + "%"; } }

    }


    public class MassModerationPreviewViewModel
    {
        public string Description { get; set; }

        public List<MassModerationGradeDistributionViewModel> GradeDistribution { get; set; }

        public List<MassModerationRoundedFinal> RoundedFinal { get; set; }
    }
}