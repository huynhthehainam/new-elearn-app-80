using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.Entity.Valence
{
    public class UserGradeObject
    {
        public int GradeObjectId { get; set; }
        public int GradeObjectTypeId { get; set; }
        public double PointsNumerator { get; set; }
        public double PointsDenominator { get; set; }
        public double WeightedDenominator { get; set; }
        public double WeightedNumerator { get; set; }
        public int UserId { get; set; }
        public int OrgUnitId { get; set; }
        public string DisplayedGrade { get; set; }
        public string OrgDefinedId { get; set; }
    }
}
