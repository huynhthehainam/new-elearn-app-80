using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.Entity.Valence
{
    public class GradeValue
    {
        public int UserId { get; set; }
        public int OrgUnitId { get; set; }
        public string DisplayedGrade { get; set; }
        public int GradeObjectIdentifier { get; set; }
        public string GradeObjectname { get; set; }
        public GRADEOBJ_T GradeObjectType { get; set; }
        public string GradeObjectTypeName { get; set; }
        public RichTextInput Comments { get; set; }
        public RichTextInput PrivateComments { get; set; }
        public double PointsNumerator { get; set; }
        public double PointsDenominator { get; set; }
        public double WeightedDenominator { get; set; }
        public double WeightedNumerator { get; set; }
        public int LastModifiedBy { get; set; }

        /// <summary>
        /// UTC Date and Time of record last modified
        /// </summary>
        public DateTime LastModifiedAt { get; set; }

        /// <summary>
        /// UTC Date and Time of result released
        /// </summary>
        public DateTime ReleasedDate { get; set; }

        private string GetMark()
        {
            var numerator = Math.Round(PointsNumerator, 2, MidpointRounding.AwayFromZero);
            var denumerator = Math.Round(PointsDenominator, 2, MidpointRounding.AwayFromZero);

            string mark;
            if (GradeObjectIdentifier == (int)Core.GradeObjectType.RoundedFinal)
                mark = $"{numerator}";
            else
                mark = $"{numerator} / {denumerator}";

            return mark;
        }

        public double Points
        {
            get
            {
                return PointsNumerator / PointsDenominator * 100;
            }
        }

        public string MyResultDisplayText
        {
            get
            {
                if (double.IsNaN(Points)) return string.Empty;
                else
                {
                    return GetMark();
                }
            }
        }
    }

    public enum GRADEOBJ_T 
    {
        Numeric = 1,
        PassFail = 2,
        SelectBox = 3,
        Text = 4,
        Calculated = 5,
        Formula = 6,
        FinalCalculated = 7,
        FinalAdjusted = 8,
        Category = 9
    }
}
