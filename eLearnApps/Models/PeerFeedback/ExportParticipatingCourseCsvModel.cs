using CsvHelper.Configuration.Attributes;

namespace eLearnApps.Models.PeerFeedback
{
    public class ExportParticipatingCourseCsvModel
    {
        [Name("Offering School")]
        public string AcadGroup { get; set; }

        [Name("Course Code")]
        public string CourseCode { get; set; }

        [Name("Course Title")]
        public string CourseName { get; set; }

        [Name("Instructor")]
        public string Instructor { get; set; }

        [Name("Total Participants")]
        public int StudentCount { get; set; }

        [Name("Group Names")]
        public string GroupNames { get; set; }

        [Name("Number of Groups")]
        public int GroupCount { get; set; }

        [Name("Unassigned Student Count")]
        public int UnassignedStudentCount { get; set; }

        [Name("Has Multiple Group (Y/N)")]
        public string HasMultipleGroup { get; set; }

        [Name("Duplicate Enrolment (Y/N)")]
        public string Duplicates { get; set; }

        [Name("Created In PSFS")]
        public string CreatedInPSFS { get; set; }
    }
}