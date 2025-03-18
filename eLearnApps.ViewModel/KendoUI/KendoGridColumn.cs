using System.Collections.Generic;

namespace eLearnApps.ViewModel.KendoUI
{
    public class KendoGridColumn
    {
        public string SelectName { get; set; }
        public string field { get; set; }
        public string title { get; set; }
        public int? width { get; set; }
    }

    public class KendoGridGroupColumn
    {
        public string SelectName { get; set; }
        public int Id { get; set; }
        public string field { get; set; }
        public string title { get; set; }
        public bool? hidden { get; set; }
        public bool? locked { get; set; }
        public bool? lockable { get; set; }
        public int? width { get; set; }
        public int? minScreenWidth { get; set; }
        public string template { get; set; }
        public KendoGridGroupColumnAttributes attributes { get; set; }
        public List<KendoGridColumn> columns { get; set; }
    }

    public class KendoGridGroupColumnAttributes
    {
        public string Class { get; set; }
        public string style { get; set; }
    }
}