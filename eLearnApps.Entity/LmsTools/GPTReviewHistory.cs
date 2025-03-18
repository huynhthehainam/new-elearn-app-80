using eLearnApps.Core.Cryptography;
using System;

namespace eLearnApps.Entity.LmsTools
{
    public class GPTReviewHistory : BaseEntity
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string? InstructorName { get; set; }
        public double Distribution_APlus { get; set; }
        public double Distribution_A { get; set; }
        public double Distribution_AMinus { get; set; }
        public double Distribution_Others { get; set; }
        public double Distribution_Pass { get; set; }
        public double Distribution_Fail { get; set; }
        public double Distribution_I { get; set; }
        public double? Distribution_PR { get; set; }
        public int Status { get; set; }
        public DateTime ReviewedOn { get; set; }
        public string ReviewedBy { get; set; } = string.Empty;
        public int ReviewedById { get; set; }
        public string? FlagReason { get; set; }
        public string AcadCareer { get; set; } = string.Empty;
        public string AcadGroup { get; set; } = string.Empty;
        public string? AcadOrg { get; set; }
        public string Strm { get; set; } = string.Empty;
        public int InstructorId { get; set; }
        public int GradeSubmissionId { get; set; }
        public bool IsPassFail { get; set; }

        /// <summary>
        /// AES Enrypted course id to be used for parameter in query string
        /// </summary>
        public string CourseKey
        {
            get
            {
                return AesEncrypt.Encrypt(CourseId.ToString());
            }
        }
    }
}