using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace eLearnApps.ViewModel.PET
{
    public class MarkRankLabelModel
    {
        public int? Id { get; set; }
        [StringLength(200)]
        public string Name { get; set; }
        public decimal Minimum { get; set; }
        public decimal Maximum { get; set; }
        public decimal? Interval { get; set; }
        public int? RankMin { get; set; }
        public int? RankMax { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public int? LastUpdateBy { get; set; }
        public string Key { get; set; }
        public List<MarkRankLabelDetailModel> DetailModel { get; set; }
    }

    public class MarkRankLabelDetailModel
    {
        public int? Id { get; set; }
        public int? MarkRankLabelId { get; set; }
        public Decimal? MarkNumber { get; set; }
        [StringLength(20)]
        public string Label { get; set; }
    }
}