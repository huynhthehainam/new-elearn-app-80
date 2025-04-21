using System;
using System.Collections.Generic;

namespace eLearnApps.ViewModel.PET
{
    public class EvaluableItemModel
    {
        public long? Id { get; set; }
        public int? EvaluationPairingId { get; set; }
        public int UserId { get; set; }
        public int OrgUnitId { get; set; }
        public bool IsOrgUnit { get; set; }
        public bool IsDeleted { get; set; }
        public int? LastUpdatedBy { get; set; }
        public DateTime LastUpdatedTime { get; set; }
        public string Name { get; set; }
        public string OrgDefinedId { get; set; }
        public int? RoleId { get; set; }
        public int Type { get; set; }
        public string Group { get; set; }
        public string Section { get; set; }
        public List<int> GroupIds { get; set; }
        public decimal Overall { get; set; }
    }
}