namespace eLearnApps.Entity.LmsTools.Dto
{
    public class MyAttendanceDto
    {
        public int AttendanceId { get; set; }
        public string Name { get; set; }
        public int Total { get; set; }
        public int Present { get; set; }
        public int Absent { get; set; }
        public int Partial { get; set; }
        public decimal? Score { get; set; }
        public int AttendanceSessionId { get; set; }
        public string AttendanceKey { get; set; }
        public bool IsUpdateSummary { get; set; }
        public int UserId { get; set; }
        public int? Excused { get; set; }
    }
}