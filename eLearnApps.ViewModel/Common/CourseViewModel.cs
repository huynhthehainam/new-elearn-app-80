using eLearnApps.Core.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.ViewModel.Common
{
    public class CourseViewModel
    {
        public int SectionNumber
        {
            get
            {
                if (string.IsNullOrEmpty(SectionName))
                {
                    return string.Empty.GetHashCode();
                }
                else
                {
                    if (SectionName.Length > 1)
                    {
                        if (int.TryParse(SectionName.Substring(1), out int tmp))
                            return tmp;
                    }
                    return SectionName.GetHashCode();
                }
            }
        }
        /// <summary>
        /// AES Enrypted course id to be used for parameter in query string
        /// </summary>
        public string CourseKey
        {
            get
            {
                return AesEncrypt.Encrypt(Id.ToString());
            }
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string SectionName { get; set; }
        public string COURSE_ID { get; set; }
        public string ACAD_CAREER { get; set; }
        public decimal? CLASS_NBR { get; set; }
        public string STRM { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? SemesterId { get; set; }
        public int? DepartmentId { get; set; }
        public bool IsMerge { get; set; }
        public bool IsIsis { get; set; }
        public string CourseDisplayName { get; set; }
    }
}
