using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.ViewModel.Valence
{
    public class UserGradeObject
    {
        public int GradeObjectId { get; set; }
        public string GradeObjectName { get; set; }
        public int GradeObjectTypeId { get; set; }
        public string GradeObjectTypeName { get; set; }
        public int PointsNumerator { get; set; }
        public int PointsDenominator { get; set; }
        public int WeightedDenominator { get; set; }
        public int WeightedNumerator { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public DateTime? GradeObjectStartDate { get; set; }
        public DateTime? GradeObjectEndDate { get; set; }
        public string CommentsForUser { get; set; }
        public int GraderUserId { get; set; }
        public string PrivateGradeComments { get; set; }
        public int UserId { get; set; }
        public int OrgUnitId { get; set; }
        public string OrgUnitCode { get; set; }
        public string DisplayedGrade { get; set; }
        public string OrgDefinedId { get; set; }
    }
}
