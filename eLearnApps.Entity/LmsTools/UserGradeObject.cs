using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.Entity.LmsTools
{
    /// <summary>
    /// Each user may only have 1 grade value per GradeObjectId,
    /// Primarykey = CompositeKey (UserId, GradeObjectId)
    /// </summary>
    public class UserGradeObject : BaseEntity
    {
        /// <summary>
        /// Composite Key 1
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// Composite Key 2
        /// </summary>
        public int GradeObjectId { get; set; }

        public string? GradeObjectName { get; set; }
        public int GradeObjectTypeId { get; set; }
        public string? GradeObjectTypeName { get; set; }
        public double PointsNumerator { get; set; }
        public double PointsDenominator { get; set; }
        public double WeightedDenominator { get; set; }
        public double WeightedNumerator { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public DateTime? GradeObjectStartDate { get; set; }
        public DateTime? GradeObjectEndDate { get; set; }

        [MaxLength(500)]
        public string? CommentsForUser { get; set; }
        public int GraderUserId { get; set; }
        public string? PrivateGradeComments { get; set; }
        /// <summary>
        /// Equivalent of CourseId
        /// </summary>
        public int OrgUnitId { get; set; }
        public string? OrgUnitCode { get; set; }
        public string? DisplayedGrade { get; set; }
        public string? OrgDefinedId { get; set; }
    }
}
