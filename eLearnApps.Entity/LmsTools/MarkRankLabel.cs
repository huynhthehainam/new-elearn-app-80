using System;

namespace eLearnApps.Entity.LmsTools
{
    public class MarkRankLabel : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Minimum { get; set; }
        public decimal Maximum { get; set; }
        public decimal? Interval { get; set; }
        public int? RankMin { get; set; }
        public int? RankMax { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public int LastUpdateBy { get; set; }
        public int CourseId { get; set; }
        public bool IsDeleted { get; set; }
    }
}