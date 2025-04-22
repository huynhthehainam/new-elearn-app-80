using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.ViewModel.FFTS
{
   public class TeamMate: IEquatable<TeamMate>
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public bool Equals(TeamMate other)
        {
            return other != null && Id == other.Id;
        }
    }
}
