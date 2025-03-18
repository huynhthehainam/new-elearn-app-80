using System;

namespace eLearnApps.Entity.Valence
{
    public class Access
    {
        public bool IsActive { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool CanAccess { get; set; }
    }
}
