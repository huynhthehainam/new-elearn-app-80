using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.Entity.LmsTools
{
    public class GradeRelease : BaseEntity
    {
        public int Id { get; set; }

        /// <summary>
        /// This is legacy PK which does not have identity in production. on 20190618 replaced with Id
        /// </summary>
        public int GradeReleaseId { get; set; }
        public int OrgUnitId { get; set; }
        public int MergeOrgUnitId { get; set; }
        public int LastUpdatedBy { get; set; }
        public DateTime LastUpdatedTime { get; set; }
        public bool IsLatest { get; set; }
    }
}