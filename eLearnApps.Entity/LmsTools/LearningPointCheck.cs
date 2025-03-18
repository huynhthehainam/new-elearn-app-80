using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.Entity.LmsTools
{
    public class LearningPointCheck: BaseEntity
    {
        public int Id { get; set; }
        public int LearningPointId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
