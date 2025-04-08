namespace eLearnApps.Entity.LmsTools.Dto
{
    public class AttendanceDataDto
    {
        public int AttendanceSessionId { get; set; }
        public decimal? Percentage { get; set; }
        public int AttendanceDataId { get; set; }
        public int AttendanceListId { get; set; }
        public int UserId { get; set; }
        public int? Participation { get; set; }
        public bool? Excused { get; set; }
    }
}