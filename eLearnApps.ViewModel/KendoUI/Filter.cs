using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace eLearnApps.ViewModel.KendoUI
{
    public class Filter
    {
        [JsonPropertyName("logic")]
        public string Logic { get; set; }

        [JsonPropertyName("field")]
        public string Field { get; set; }

        [JsonPropertyName("operator")]
        public string Operator { get; set; }

        [JsonPropertyName("value")]
        public object Value { get; set; }

        [JsonPropertyName("filters")]
        public List<Filter> Filters { get; set; }

        public bool IsDescriptor
        {
            get
            {
                return Field != null && Operator != null;
            }
        }
    }
    public class KendoUIRequestModel
    {
        public int Skip { get; set; }
        public int Take { get; set; }
        public bool GroupPaging { get; set; }
        public Filter Filter { get; set; }
        public int Page {  get; set; }
        public int PageSize { get; set; }

    }
}
