using System.Collections.Generic;

namespace eLearnApps.ViewModel.Valence
{
    public class PagedResultSetDynamic<T>
    {
        public PagingInfo PagingInfo { get; set; }
        public List<T> Items { get; set; }
    }
}
