using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.ViewModel.FFTS
{
   public class SearchResult
    {
        public SearchResult()
        {
            UnavailableList = new List<TeamMate>();
            AvailableList = new List<TeamMate>();
        }

        public int Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public List<TeamMate> UnavailableList { get; set; }
        public List<TeamMate> AvailableList { get; set; }
        public bool IsOverlap { get; set; }
        public double StartDateMilliseconds { set; get; }
        public double EndDateMilliseconds { set; get; }
        public string Title { get; set; }
        public bool StartOfDay { get; set; }
        public bool EndOfDay { get; set; }
    }

    public class SearchResultWStatus
    {
        public bool CanSearch { get; set; }
        public string ErrorMessage { get; set; }
        public List<SearchResult> Results { get; set; }

    }
}
