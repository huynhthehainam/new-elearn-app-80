using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.ViewModel.RPT
{
   public class GradeSchemeRangeViewModel
    {
        public string Label { get; set; }
        public double MinPoint { get; set; }
        public double MaxPoint { get; set; }
        public string Grade { get; set; }
        public string Colour { get; set; }
        public int Order { get; set; }
    }

    public class GradeSchemeViewModel
    {
        public int GradeSchemeId { get; set; }
        public string Name { get; set; }
        public string GradeSchemeUrl { get; set; }
        public double MinMark { get; set; }
        public double MaxMark { get; set; }
        public bool IsMixedGradeType { get; set; }
        public List<GradeSchemeRangeViewModel> Schemes { get; set; }

        public string FindGradeFromMark(double mark)
        {
            if (Schemes == null)
                return string.Empty;
            else
            {
                // only loop through Ranges if Ranges have 2 elements or more. Else simply return last item or null
                foreach(var scheme in Schemes)
                {
                    if (scheme.MinPoint <= mark && mark < scheme.MaxPoint)
                        return scheme.Grade;
                }

                // mark is not within range
                return string.Empty;
            }
        }

        public GradeSchemeRangeViewModel FindGradeSchemeFromMark(double mark)
        {
            if (Schemes == null)
                return null;
            else
            {
                // only loop through Ranges if Ranges have 2 elements or more. Else simply return last item or null
                foreach (var scheme in Schemes)
                {
                    if (scheme.MinPoint <= mark && mark < scheme.MaxPoint)
                        return scheme;
                }

                // mark is not within range
                return null;
            }
        }

        public string FindColor(string grade)
        {
            if (string.IsNullOrEmpty(grade)) return string.Empty;

            var scheme = Schemes.Where(s => string.Equals(grade.Trim(), s.Grade.Trim(), StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            if (scheme != null) return scheme.Colour;
            else return string.Empty;
        }
    }
}
