using eLearnApps.Core.Cryptography;
using System;

namespace eLearnApps.Entity.LmsTools
{
    public class GPTApprovalHistory : BaseEntity
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
        public double Distribution_I { get; set; }
        public double? Distribution_PR { get; set; }
        public double Distribution_Pass { get; set; } = 0;
        public double Distribution_Fail { get; set; } = 0;
        public int Status { get; set; }
        public DateTime ApprovedOn { get; set; }
        public string ApprovedBy { get; set; } = string.Empty;
        public string? RejectReason { get; set; }
        public string AcadCareer { get; set; } = string.Empty;
        public string AcadGroup { get; set; } = string.Empty;
        public string? AcadOrg { get; set; }
        public string Strm { get; set; } = string.Empty;
        public int ApprovedById { get; set; }
        public int InstructorId { get; set; }
        public int GradeSubmissionId { get; set; }
        public bool IsPassFail { get; set; } = false;

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