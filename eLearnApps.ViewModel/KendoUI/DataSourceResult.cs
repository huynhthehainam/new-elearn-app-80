using System.Collections;
using System.Collections.Generic;

namespace eLearnApps.ViewModel.KendoUI
{
    public class DataSourceResult
    {
        public object ExtraData { get; set; }

        public IEnumerable Data { get; set; }

        public object Errors { get; set; }

        public int Total { get; set; }
    }


    public class DataSourceResultObject
    {
        public List<ResultItem> results { get; set; }
        public Pagination pagination { get; set; }
    }

    public class Pagination
    {
        public bool more { get; set; }
    }

    public class ResultItem: DataItem
    {
        public List<DataItem> subItem { get; set; }
    }

    public class DataItem
    {
        public string id { get; set; }
        public string text { get; set; }
    }


}