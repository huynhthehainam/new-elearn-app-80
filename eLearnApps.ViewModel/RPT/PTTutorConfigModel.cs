using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.ViewModel.RPT
{
    public class PTTutorConfigModel
    {
        public string STRM { get; set; }
        public string COURSE_ID { get; set; }
        public DateTime TutorRegStartDate { get; set; }
        public DateTime TutorRegEndDate { get; set; }
        public float TutorMinMark { get; set; }
        public string PGUG { get; set; }
        public List<string> TutorExcludedRole { get; set; }

        public bool MarkPassThreshold(int mark)
        {
            bool passThreshold = (mark >= TutorMinMark);
            return passThreshold;
        }
        public List<string> PGUGList
        {
            get
            {
                return string.IsNullOrEmpty(PGUG) ? new List<string> { } : PGUG.Split(',').ToList();
            }
        }
    }
}
