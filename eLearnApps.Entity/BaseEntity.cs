namespace eLearnApps.Entity
{
    public class BaseEntity
    {
        //public int Id { get; set; }

        public override bool Equals(object? obj)
        {
            // when both is null 
            if (this is null && obj is null) return true;
            else
            {
                // when either one is null, return false
                if (this is null || obj is null) return false;

                // when none is null then proceed as normal
                return Equals(obj as BaseEntity);
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(BaseEntity x, BaseEntity y)
        {
            return Equals(x, y);
        }

        public static bool operator !=(BaseEntity x, BaseEntity y)
        {
            return !(x == y);
        }
    }
}