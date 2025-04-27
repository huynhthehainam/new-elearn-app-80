using System;

namespace eLearnApps.ViewModel.PET
{
    public class EvaluationEntrySessionModel
    {
        public int? EvaluationSessionId { get; set; }
        public DateTime EntryStartTime { get; set; }
        public DateTime EntryCloseTime { get; set; }
    }
}