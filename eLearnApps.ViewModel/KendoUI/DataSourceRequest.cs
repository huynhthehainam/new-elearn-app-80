using System;
using System.Collections.Generic;

namespace eLearnApps.ViewModel.KendoUI
{
    public class DataSourceRequest
    {
        public DataSourceRequest()
        {
            Page = 1;
            PageSize = 10;
        }

        public int Page { get; set; }
        public int PageSize { get; set; }
        public int? Id { get; set; }
        public int? Id2 { get; set; }
        public string Key1 { get; set; }
        public string Key2 { get; set; }
        public int? ImageSize { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public List<GridSort> Sort { get; set; }
    }

    public class GridSort
    {
        public string field { get; set; }
        public string dir { get; set; }
    }
}