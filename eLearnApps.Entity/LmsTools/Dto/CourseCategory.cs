using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.Entity.LmsTools.Dto
{
   public class CourseCategory
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public int CategoryGroupId { set; get; }
        public string CategoryGroupName { set; get; }
    }
}
