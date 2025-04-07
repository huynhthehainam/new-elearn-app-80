using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.Entity.LmsTools.Dto
{
    /// <summary>
    /// Used in Line Chart in ICS
    /// </summary>
    public class SessionLineChart
    {
        public ICSSession Session { get; set; }
        public List<User> Users { get; set; }
        public List<ICSSessionUserSense> Senses { get; set; }
    }
}
