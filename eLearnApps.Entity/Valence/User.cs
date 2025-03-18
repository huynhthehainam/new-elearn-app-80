using System;

namespace eLearnApps.Entity.Valence
{
    public class User
    {
        public string Identifier { get; set; }
        public string DisplayName { get; set; }
        public string EmailAddress { get; set; }
        public string OrgDefinedId { get; set; }
        public string ProfileBadgeUrl { get; set; }
        public string ProfileIdentifier { get; set; }
        public bool IsClasslist { get; set; }
        private string sectionName = "unknown";
        public int sectionId = 0;
        public void SetSection(string name, int id)
        {
            sectionName = name;
            sectionId = id;
        }
        public string GetSectionName() => sectionName;
        public int GetSectionId() => sectionId;
        public int StudentSectionNameNumber
        {
            get
            {
                try
                {
                    var numStr = sectionName.Substring(1);
                    var number = int.Parse(numStr);
                    return number;
                }
                catch (Exception)
                {
                    return int.MaxValue;
                }
            }
        }
    }
}

