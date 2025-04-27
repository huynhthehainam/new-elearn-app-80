using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace eLearnApps.ViewModel.FFTS
{
    public class SearchQuery
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Duration { get; set; }
        public List<TeamMate> Participants { get; set; }
    }
}