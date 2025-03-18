namespace eLearnApps.Entity.LmsTools
{
    public class User : BaseEntity, System.IEquatable<User>
    {
        public int Id { get; set; }
        public string? DisplayName { get; set; }
        public string? EmailAddress { get; set; }
        public string? OrgDefinedId { get; set; }
        public string? ProfileBadgeUrl { get; set; }
        public string? ProfileIdentifier { get; set; }

        public bool Equals(User? other)
        {
            if (this is null && other is null) return true;
            if (other is null) return false;
            if (Id != other.Id || DisplayName != other.DisplayName ||
                EmailAddress != other.EmailAddress || OrgDefinedId != other.OrgDefinedId ||
                ProfileBadgeUrl != other.ProfileBadgeUrl || ProfileIdentifier != other.ProfileIdentifier) return false;

            return true;
        }

        public override int GetHashCode()
        {
            int hashId = Id.GetHashCode();

            return hashId;
        }
    }
}