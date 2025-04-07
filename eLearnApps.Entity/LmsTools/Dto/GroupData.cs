using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.Entity.LmsTools.Dto
{
    public class GroupData
    {
        public int GroupId { get; set; }
        public string Name { get; set; }
        public string CourseCategoryGroupName { get; set; }
        public Description Description { get; set; }
        public List<int> Enrollments { get; set; }
    }
}
