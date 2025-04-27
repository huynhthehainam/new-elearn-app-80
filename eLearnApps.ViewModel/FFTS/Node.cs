using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.ViewModel.FFTS
{
   public class Node
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public List<Node> Nodes { set; get; }
        public int Type { set; get; }
    }
}
