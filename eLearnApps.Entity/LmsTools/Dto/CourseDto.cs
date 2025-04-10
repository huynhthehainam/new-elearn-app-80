using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.Entity.LmsTools.Dto
{
   public class CourseDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public int StudentCount { get; set; }
        public bool HasSection { get; set; }
    }
}
