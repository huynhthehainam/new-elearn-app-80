using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.ViewModel.WidgetLTI
{
    public class WCFQuiz
    {
        public Int32 Id { get; set; }

        public String Header { get; set; }

        public String CreateorName { get; set; }

        public List<string> Taggedtags { get; set; }

        public DateTime LastUpdated { get; set; }

        public int VotesCount { get; set; }


        public int AnswersCount { get; set; }

        public int? ViewCount { get; set; }

        public string UserType { get; set; }
    }
}
