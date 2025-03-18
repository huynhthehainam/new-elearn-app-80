using System.Collections.Generic;

namespace eLearnApps.ViewModel.KendoUI
{
    public class TreeViewItem
    {
        public int id { get; set; }
        public string text { get; set; }
        public bool expanded { get; set; }
        public bool hasChildren { get; set; }
        public List<NodeItem> items { get; set; }
    }

    public class NodeItem
    {
        public int id { get; set; }
        public string text { get; set; }
        public int? NumOfStudent { get; set; }
        public List<int> UserEnroll { get; set; }
        public bool hasChildren { get; set; }
    }
}