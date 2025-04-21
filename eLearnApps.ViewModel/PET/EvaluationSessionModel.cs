using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.ViewModel.PET
{
   public class EvaluationSessionModel
    {
        public int EvaluationSessionId { get; set; }
        public int EvaluationId { get; set; }
        public string Label { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public double StartTotalMilliseconds { get; set; }
        public double EndTotalMilliseconds { get; set; }
        public bool IsSelected { get; set; }
        public decimal Percent { get; set; }
        public decimal Weight { get; set; }
        public int EvaluationPairingId { get; set; }
        public List<EvaluationQuestionModel> ListEvaluationQuestion { get; set; }
        public string EvaluationSessionKey { get; set; }
        public string StartUtc { get; set; }
        public string EndUtc { get; set; }
        public string EvaluationKey { get; set; }
        public bool IsCloseDisplay { get; set; }
    }
}
