using eLearnApps.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.ViewModel.ICS
{
    public class UserSenseViewModel
    {
        public int Id { get; set; }
        public int Sessionid { get; set; }
        public int UserId { get; set; }
        public Senses Sense { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
