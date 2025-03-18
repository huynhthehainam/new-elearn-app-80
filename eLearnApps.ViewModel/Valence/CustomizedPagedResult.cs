using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.ViewModel.Valence
{
    public class CustomizedPagedResult<T>
    {
        public string Bookmark { get; set; }
        public bool HasMoreItems { get; set; }
        public List<T> Items { get; set; }
    }
}
