using System.Collections.Generic;

namespace eLearnApps.Entity.LmsTools.Dto
{
    public class ItemDto
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public string Description { set; get; }
        public int Type { set; get; }

        /// <summary>
        ///     To be used only when student evaluate own group member, where both UserId and OrgUnitId are both required to be
        ///     filled
        ///     But it is not a group evaluation
        ///     TODO: user may belong to more than 1 group. this reference is incorrect. need to remove.
        /// </summary>
        public int GroupId { get; set; }

        public string Group { get; set; } // Group Id
        public string Section { get; set; } // Section name
        public string OrgDefinedId { get; set; } // campus id
        public int OrgUnitId { get; set; }
        public int UserId { get; set; }
        public string DisplayName { get; set; }
        public List<User> UserList { get; set; }
        public List<int> CategoryGroupId { get; set; }

        public bool IsInstructor { get; set; } = false;
        public int EvaluationPairingId { get; set; }
        public string MemberGroup { get; set; }
    }
}