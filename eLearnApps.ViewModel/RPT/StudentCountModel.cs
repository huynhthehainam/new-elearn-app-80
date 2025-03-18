using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.ViewModel.RPT
{
   public class StudentCountModel
    {
        public int OrgUnitId { get; set; }
        public string SectionName { set; get; }
        public int StudentCount { set; get; }
        public string Instructors { get; set; }
    }
}
