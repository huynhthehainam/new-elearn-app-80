using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.ViewModel.Valence
{
    public class GradeValue
    {
        public int GradeObjectIdentifier { get; set; }
        public string GradeObjectName { get; set; }
        public int GradeObjectType { get; set; }
        public string GradeObjectTypeName { get; set; }
        public double PointsNumerator { get; set; }
        public double PointsDenominator { get; set; }
        public double WeightedDenominator { get; set; }
        public double WeightedNumerator { get; set; }
        public string DisplayedGrade { get; set; }
        public RichTextInput Comments { get; set; }
        public RichTextInput PrivateComments { get; set; }
        public DateTime? LastModified { get; set; }
        public int LastModifiedBy { get; set; }
        public DateTime? ReleasedDate { get; set; }
    }
}


	
