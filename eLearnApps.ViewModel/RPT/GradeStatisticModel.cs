using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.ViewModel.RPT
{
   public class GradeStatisticModel
    {
        public string Final { set; get; }
        public string Section { get; set; }
        public string Marks { set; get; }
        public string Grade { set; get; }
        public string DisplayFinal => SectionRowNumber > 0 ? string.Empty : Final;
        public int SectionRowNumber { get; set; }
        public bool IsLastRowOfSection { get; set; }
    }
}
