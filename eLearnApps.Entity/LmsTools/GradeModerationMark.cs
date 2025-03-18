namespace eLearnApps.Entity.LmsTools
{
    public class GradeModerationMark
    {
        public long ModerationMarkId { get; set; }
        public int ModerationId { get; set; }
        public int UserId { get; set; }
        public int GradeObjectId { get; set; }
        public double? MarksBefore { get; set; }
        public double? MarksAfter { get; set; }
        public double? MaxMarks { get; set; }
        public double? AdjustedMarksBefore { get; set; }
        public double? AdjustedMaxMarks { get; set; }
        public double? CalculatedMarksBefore { get; set; }
        public double? CalculatedMaxMarks { get; set; }
    }
}