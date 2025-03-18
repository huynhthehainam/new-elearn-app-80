using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.Entity.LmsTools
{
    public class PeerTutoringRecord
    {
        /// <summary>
        /// Complex Key 1
        /// </summary>
        public int OrgUnitId { get; set; }
        /// <summary>
        /// Complex key 2
        /// </summary>
        public int StudentUserId { get; set; }
        /// <summary>
        /// Core.Constants.PT_TUTOR_ROLE  || Core.Constants.PT_TUTEE_ROLE
        /// </summary>
        public string? TargetRole { get; set; }
        public DateTime? SubmittedOn { get; set; }
        public int RecommendedBy { get; set; }
        public DateTime? RecommendedOn { get; set; }
    }
}
