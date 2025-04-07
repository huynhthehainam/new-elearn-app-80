namespace eLearnApps.ViewModel.ICS
{
    public class CreateSessionViewModel
    {
        public int CourseId { get; set; }

        public CreateSingleSession SingleSession { get; set; }

        public CreateRecurringSession RecurringSession { get; set; }

        public CreateByImportViewModel ImportedSession { get; set; }
    }
}