namespace eLearnApps.Entity.LmsTools
{
    public class Permission : BaseEntity
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? SystemName { get; set; }
        public string? Category { get; set; }
        public string? Url { get; set; }
        public int Order { get; set; }
        public int? Parent { get; set; }
    }
}