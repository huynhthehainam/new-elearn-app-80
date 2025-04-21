using System;

namespace eLearnApps.Entity.LmsTools.Dto
{
    public class PeerFeedbackSessionsDto
    {
        public int Id { get; set; }
        public DateTime EntryStartTime { get; set; }
        public DateTime EntryCloseTime { get; set; }
        public int LastUpdatedBy { get; set; }
        public DateTime LastUpdatedTime { get; set; }
        public string Strm { get; set; }
        public string Label { get; set; }
        public int PeerFeedbackId { get; set; }
        public int? PeerFeedBackParingId { get; set; }
        public string PeerFeedBackName { get; set; }
        public string CourseOfferingCode { get; set; }
    }

    public class PeerFeedBackEvaluationDto
    {
        public int PeerFeedBackId { get; set; }
        public string Name { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public double Progress { get; set; }
        public string CourseName { get; set; }
        public int PeerFeedBackPairingId { get; set; }
        public int PeerFeedBackSessionId { get; set; }
        public int PeerFeedBackGroupId { get; set; }
        public string SessionName { get; set; }
        public int TargetUserId { get; set; }
        public string PeerFeedBackName { get; set; }
        public int CourseId { get; set; }
    }

    public class PeerFeedBackTargetsOrgUnitPairingDto
    {
        public int OrgUnitId { get; set; }
        public int PeerFeedbackPairingId { get; set; }
    }
    public class PeerFeedBackGroupDto
    {
        public int CourseId { get; set; }
        public string OfferingSchool { get; set; }
        public string CourseCode { get; set; }
        public string CourseTitle { get; set; }
        public string InstructorName { get; set; }
        public string InstructorEmail { get; set; }
        public int TotalParticipants { get; set; }
        public string GroupNames { get; set; }
        public int? NoOfGroup { get; set; }
        public int? UnassignedStudentCount { get; set; }
        public string HasMultipleGroup { get; set; }
        public string DuplicateEnrollment { get; set; }
        public string CreatedInPSFS { get; set; }
        public string Instructors { get; set; }
    }
    public class PeerFeedBackGroupReadinessReportDto
    {
        public string AcadGroup { get; set; }
        public int OrgUnitId { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string Instructor { get; set; }
        public int StudentCount { get; set; }
        public string GroupNames { get; set; }
        public int GroupCount { get; set; }
        public int UnassignedStudentCount { get; set; }
        public string Duplicates { get; set; }
        public string CreatedInPSFS { get; set; }
        public string HasMultipleGroup { get; set; }
        public string MultipleCategoryGroups { get; set; }
    }
}