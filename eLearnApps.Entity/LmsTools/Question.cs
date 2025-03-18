using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.Entity.LmsTools
{
    public class Question: BaseEntity
    {
        public int Id { get; set; }
        public int ICSSessionId { get; set; }
        public int UserId { get; set; }
        public string? Content { get; set; }
        public int RecordStatus { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? LastUpdatedOn { get; set; }
        public bool? Addressed { get; set; }

    }
}
