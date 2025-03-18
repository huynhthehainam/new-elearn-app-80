using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.ViewModel.Valence
{
    public class ObjectListPage<T>
    {
        public string Next { get; set; }
        public List<T> Objects { get; set; }
    }
}
