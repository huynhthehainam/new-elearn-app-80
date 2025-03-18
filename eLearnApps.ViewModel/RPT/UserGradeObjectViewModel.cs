using eLearnApps.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.ViewModel.RPT
{
    public class UserGradeObjectViewModel
    {
        public double Points
        {
            get
            {
                return PointsNumerator / PointsDenominator * 100;
            }
        }

        public string GradeBookDisplayText
        {
            get
            {
                if (GradeObjectTypeId == (int)Core.GradeObjectType.Text)
                {
                    return DisplayedGrade;
                }
                else
                {
                    if (double.IsNaN(Points))
                    {
                        if (!string.IsNullOrWhiteSpace(DisplayedGrade) && DisplayedGrade == "I") return "I";
                        return string.Empty;
                    }
                    else
                    {
                        var mark = GetMark();

                        var grade = string.Empty;
                        if (!string.IsNullOrWhiteSpace(DisplayedGrade))
                            grade = $", {DisplayedGrade}";

                        return $"{mark}{grade}";
                    }
                }
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


        private string GetMark()
        {
            var numerator = Math.Round(PointsNumerator, 2, MidpointRounding.AwayFromZero);
            var denumerator = Math.Round(PointsDenominator, 2, MidpointRounding.AwayFromZero);

            string mark;
            if (GradeObjectTypeId == (int)Core.GradeObjectType.RoundedFinal || GradeObjectId == (int)Core.GradeObjectType.Final)
                mark = $"{numerator}";
            else
                mark = $"{numerator} / {denumerator}";

            return mark;
        }

        public bool IsInvalid {
            get
            {
                if (GradeObjectTypeId == (int)GradeObjectType.Final || GradeObjectId == (int)GradeObjectType.RoundedFinal)
                {
                    if (PointsNumerator < 0 || PointsNumerator > PointsDenominator) return true;
                }

                return false;
            }
        }

        public string KendoGridColumnId 
        { 
            get 
            {
                switch (GradeObjectId)
                {
                    case (int)GradeObjectType.Final:
                        return "GradeObject_Final";
                    case (int)GradeObjectType.Rank:
                        return "GradeObject_Rank";
                    default:
                        return $"GradeObject_{GradeObjectId}";
                }
            } 
        }

        public int UserId { get; set; }
        public string OrgDefinedId { get; set; }
        public int GradeObjectId { get; set; }
        public double PointsNumerator { get; set; }
        public double PointsDenominator { get; set; }
        public double WeightedDenominator { get; set; }
        public double WeightedNumerator { get; set; }
        public string DisplayedGrade { get; set; }
        public int GradeObjectTypeId { get; set; }
        public int CourseId { get; set; }
    }
}
