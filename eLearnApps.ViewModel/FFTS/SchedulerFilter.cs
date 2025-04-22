using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.ViewModel.FFTS
{
   public class SchedulerFilter
    {
        public List<int> Teammates { set; get; }
        public DateTime Start { set; get; }
        public DateTime End { set; get; }
    }
}
