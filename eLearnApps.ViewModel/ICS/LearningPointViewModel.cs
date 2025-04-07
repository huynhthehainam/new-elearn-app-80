namespace eLearnApps.ViewModel.ICS
{
    public class LearningPointViewModel
    {
        public int Id { get; set; }
        public int SessionId { get; set; }

        public string Description { get; set; }

        public int Progress { get; set; }
        public int Index { get; set; }
        public int CountPointCheck { get; set; }
        public string Statistic { get; set; }
        public bool IsEditable { get; set; }
        public string LearningPointKey { get; set; }
        public bool HasLearningPointCheck { get; set; }

    }
}