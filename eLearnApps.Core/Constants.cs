using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace eLearnApps.Core
{
    public enum InviteStatus
    {
        Accept = 1,
        Pending = 2,
        Reject = 0
    }

    public enum MeetingType
    {
        Me = 1,
        TeamMate = 2
    }

    public static class Constants
    {
        public enum GradeSystem
        {
            [Description("Points")] Points,
            [Description("Weighted")] Weighted,
            [Description("Formula")] Formula
        }

        public static string PT_TUTOR_ROLE = "Tutor";
        public static string PT_TUTEE_ROLE = "Tutee";

        public static string CipherKey = "asdfastqwertwrty34";
        public static string SplitKey = "_SplitKey_";

        public static string SUBMITTED_RESET_STATUS = "N";
        public static int MaxIcsSessionLength = 4 * 60 * 60; // 4h - unit is in seconds
        public static int MinIcsSessionLength = 15 * 60; // 15m - unit is in seconds
        public static string EightZeroPadding = "00000000";
        public static int DefaultImageQuality = 80;
        public static int MaxNumOfParamForDB = 2000;
        public static string DoingWellColor = "rgb(255, 99, 132, 1)";
        public static string ConfusedColor = "rgb(251,220,93, 1)";
        public static string SlowDownColor = "rgba(211,223,150,1)";
        public static string GoFasterColor = "rgba(75, 192, 192, 1)";
        public static string ElaborateMoreColor = "rgba(153, 102, 255, 1)";
        public static string PleaseRepeatColor = "rgba(255, 159, 64, 1)";

        public static string DisplayDateTimeFormat = "dd MMM yyyy hh:mm tt";
        public static string CourseIdClaim = "CourseId";
        public static string ToolNameClaim = "ToolName";
        public static string RoleClaim = "Role";

        public static string AcadCareerUGRD = "UGRD";
        /// <summary>
        ///     this is only for fallback scenario (this is the value for production only)
        /// </summary>
        public static int StudentRoleId = 104;

        public class GradeScheme
        {
            public const int PercentageGradeSchemeId = 0;
            public const string PercentageGradeSchemeUrlSuffix = "grades/schemes/0";
            public const int SMUDefaultGradeSchemeId = -1;
        }

        public static List<string> AttendanceAllowedSupportingDocs => new List<string> { ".pdf", ".jpg", ".jpeg", ".png", ".gif", ".doc", ".docx" };

        public enum PhotoSize
        {
            Small = 1,
            Medium = 2,
            Large = 3
        }

        // TODO: need to move all these constant to COORE
        public enum PhotoPosition
        {
            Left = 1,
            Top = 2,
            Right = 3,
            Bottom = 4
        }

        public enum PageOrientation
        {
            Portrait = 1,
            Landscape = 2
        }

        public enum GroupBy
        {
            Course = -1,
            Section = -2,
            Group = 0
        }
        public static HybridDictionary IcsChartColor = new HybridDictionary
        {
            {Senses.DoingWell, "rgba(255, 99, 132, 1)"},
            {Senses.Confused, "rgba(251,220,93, 1)"},
            {Senses.SlowDown, "rgba(211,223,150,1)"},
            {Senses.GoFaster, "rgba(75, 192, 192, 1)"},
            {Senses.ElaborateMore, "rgba(153, 102, 255, 1)"},
            {Senses.PleaseRepeat, "rgba(255, 159, 64, 1)"}
        };
    }

    public enum SubmissionStatus
    {
        None,
        Partial,
        All
    }

    public enum GradeSystem
    {
        Points,
        Weighted,
        Formula
    }

    public class RoleName
    {
        /// <summary>
        ///     Use case insensitive string comparison
        /// </summary>
        public const string INSTRUCTOR = "instructor";

        /// <summary>
        ///     Use case insensitive string comparison
        /// </summary>
        public const string STUDENT = "student";

        /// <summary>
        ///     searching for TA role must use startswith or LIKE 'TA%'
        /// </summary>
        public const string TA = "ta";

        /// <summary>
        ///     searching for TA role must use startswith or LIKE 'TA%'
        /// </summary>
        public const string AUDITSTUDENT = "audit student";
    }


    public enum LogLevel
    {
        Debug = 0,
        Information = 1,
        Error = 2,
        Warning = 3,
        Fatal = 4
    }

    public enum LogType
    {
        TrackLog = 2,
        BatchJob = 3,
        Error = 4,
        GPTAudit = 5,
        GPTDebug = 6
    }

    public enum RecordStatus
    {
        Deleted = 0,
        Active = 1,
        Locked = 2,
        Unlock = 3,
        Private = 5
    }

    public enum JournalStatus
    {
        Open = 1,
        Locked = 2,
        Future = 3,
        Closed = 4,
        Private = 5
    }

    public enum Senses
    {
        [Description("Doing Well")]
        DoingWell = 1,
        [Description("Confused")]
        Confused = 2,
        [Description("Slow Down")]
        SlowDown = 3,
        [Description("Go Faster")]
        GoFaster = 4,
        [Description("Elaborate More")]
        ElaborateMore = 5,
        [Description("Please Repeat")]
        PleaseRepeat = 6
    }
    
    public enum SchemeSymbol
    {
        Present = 1,
        Absent = 2,
        Partial = 3
    }

    public enum AttendanceCourseType
    {
        Section = 1,
        Category = 2
    }

    public enum EvaluationType
    {
        StudentsEvaluateOneAnother = 1,
        StudentsEvaluateOwnGroupMembers = 2,
        StudentsEvaluateGroups = 3,
        StudentsSelfEvaluate = 4,
        StudentsEvaluateTAs = 5,
        InstructorsEvaluateStudents = 6,
        InstructorsEvaluateGroups = 7
    }

    public enum EvaluationEntryType
    {
        [Description("Comment Only")] CommentOnly = 0,
        [Description("Select Mark")] SelectMark = 1,
        [Description("Input Mark")] InputMark = 2,
        [Description("Rank Target")] RankTarget = 3,
        [Description("Select Target")] SelectTarget = 4
    }

    public enum QueryType
    {
        Target = 1,
        Evaluator = 2
    }

    public enum ScoreCalculationType
    {
        [Description("Average")] Average = 1,
        [Description("Summation")] Summation = 2,
        [Description("Weighted")] Weighted = 3,
        [Description("Normalized")] Normalized = 4
    }
    public enum ScoreCalculationDescription
    {
        [Description("Overall score is calculated by averaging the marks of all items")] Average = 1,
        [Description("Overall score is calculated by summing up the marks of all items")] Summation = 2,
        [Description("Overall score is calculated by summing up the weighted marks of all items")] Weighted = 3,
        [Description("Overall score is calculated by averaging the normalized marks of all items (normalized to max. 100 marks)")] Normalized = 4
    }

    public enum EvaluatorUserTypeEnum
    {
        Student = 1,
        Instructor = 2
    }

    public enum InviteResponseType
    {
        Accept,
        Reject,
        Expire
    }

    public enum DbContext
    {
        LMSTools = 1,
        LMSISIS = 2,
        LOGGING = 3
    }

    public enum ToolName
    {
        Ffts = 1,
        Cmt = 2,
        Pet = 3,
        Ics = 4,
        Journal = 5,
        Rpt = 6,
        SecurityAdministrator = 6,
        Psfs = 7,
        Ee = 8,
        Gpt = 9
    }

    public enum ConnectionStringType
    {
        LmsTool = 1,
        Logging = 2,
        Lmsisis = 3,
        Sqlite = 4,
        DataHub = 5
    }

    public enum GradeModerationType
    {
        [Description("All Marks")] AllMark = 0,
        [Description("Between")] MarkBetween = 1,
        [Description("Greater Than or Equal To")] MarkGreaterThanOrEqualTo = 2,
        [Description("Greater Than")] MarkGreaterThan = 3,
        [Description("Equal To")] MarkEqualTo = 4,
        [Description("Less Than or Equal To")] MarkLessThanOrEqualTo = 5,
        [Description("Less Than")] MarkLessThan = 6
    }

    public enum SectionType
    {
        Combined = -1,
        NoSection = -2
    }

    public enum GradeObjectType
    {
        [Description("NA")] NA = -99,
        [Description("Rank")] Rank = -2,
        [Description("Final")] Final = -1,
        [Description("Rounded Final")] RoundedFinal = 0,
        [Description("Numeric")] Numeric = 1,
        [Description("PassFail")] PassFail = 2,
        [Description("SelectBox")] SelectBox = 3,
        [Description("Text")] Text = 4,
        [Description("Calculated")] Calculated = 5,
        [Description("Formula")] Formula = 6,
        [Description("Final Calculated")] FinalCalculated = 7,
        [Description("Final Adjusted")] FinalAdjusted = 8,
        [Description("Category")] Category = 9
    }

    public enum StatusCode
    {
        Fail = -1,
        DoNotAllow = 0,
        Success = 1,
        DoesNotExists = -2,
        NotAuthorized = -3,
        AlreadyExists = -4
    }

    public enum CacheDataKey
    {
        UpcomingClass = 1,
        BiddingStudent = 2,
        MergeCourse = 3
    }
    public enum JournalType
    {
        OneTime = 1,
        Weekly = 2,
        Fortnightly = 3,
        Monthly = 4
    }
    public enum ResizeType
    {
        LongestSide,
        Width,
        Height
    }

    public enum PictureType
    {
        Image = 1,
        Avatar = 2
    }

    public enum GPTRoles
    {
        Admin = 101,
        Approver = 102,
        Reviewer = 103
    }

    public enum ApprovalStatus
    {
        Reset = -2,
        Rejected = -1,
        Resubmitted = 0,
        Approved = 1
    }
    public enum ReviewStatus
    {
        Rejected = -3,
        Reset = -2,
        Flagged = -1,
        Resubmitted = 0,
        Endorsed = 1,
        Approved = 2 // Triggered when Approver approves a course
    }

    public enum StudentPhotoSizeLarge
    {
        Width = 150,
        Height = 200
    }
    public enum StudentPhotoSizeMedium
    {
        Width = 75,
        Height = 100
    }
    public enum StudentPhotoSizeSmall
    {
        Width = 38,
        Height = 50
    }
    public enum ReviewStatusFilter
    {
        All = -999,
        Endorsed = ReviewStatus.Endorsed,
        Flagged = ReviewStatus.Flagged
    }
    public enum PeerFeedbackType
    {
        StudentsEvaluateOwnGroupMembers = 2,
    }
    public enum AuditType
    {
        None = 0,
        Create = 1,
        Update = 2,
        Delete = 3,
        Click = 4
    }

    public enum AcadCareer
    {        
        PG = 0,
        UG = 1,
        Both = 2
    }
    public enum DatabaseName
    {
        LMSTools = 0,
        LMSISIS = 1
    }
}