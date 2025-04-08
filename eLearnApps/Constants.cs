using System;
using eLearnApps.Core;

namespace eLearnApps
{
    public class Constants
    {
        private readonly IConfiguration _configuration;
        public Constants(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public static string ReleasedColor => "#f90";
        public static string FutureReleasedColor => "#00f";

        public const int PageSize20 = 20;
        public static int MaxICSSessionLength = 4 * 60 * 60; // 4h - unit is in seconds
        public static int MinICSSessionLength = 15 * 60; // 15m - unit is in seconds
        public static string KendoDisplayTimeSpanFormat = @"HH:mm";
        public static string DisplayTimeSpanFormat = @"HH\:mm";
        public static string DisplayDateFormat = "dd MMM yyyy";
        public static string DisplayDateTimeFormat = "dd MMM yyyy hh:mm tt";
        public static string ShortDateTimeFormat = "yyyy/MM/dd";
        public static string DateTimeHourFormat = "yyyy/MM/dd HH:mm";
        public static string DefaultScheduleStartTime = "yyyy/MM/dd 08:00";
        public static string DefaultScheduleEndTime = "yyyy/MM/dd 22:30";
        public static string ApplicationJson = "application/json";
        public static string ColorOwner = "#f8a398";
        public static string ColorTeamMate = "#51a0ed";
        public static int PageSize = 10;
        public static string SingaporeStandardTime = "Singapore Standard Time";
        public static string Gmt8 = "GMT+08:00";
        public static string DefaultOffSet = "+08:00";
        public static int RemainingHour = 0;
        public static string SplitKey = "___";
        public static string HomePageUrl = "https://www.smu.edu.sg/";
        public static string DateTimeFormatUniversal = "yyyyMMddTHHmmssZ";
        public static string SmuDomain = "smu.edu.sg";
        public static string AttachmentFileName = "Attachment";

        public string LinkedServerPrefix =>
            _configuration.GetValue<string>("LinkedServerPrefix") ?? "";

        public static string LocalIpAddress = "127.0.0.1";
        public string LinkInvite => _configuration.GetValue<string>("LinkInvite") ?? "";
        public string MailError => _configuration.GetValue<string>("MailError") ?? "";
        public string SystemMailAddress => _configuration.GetValue<string>("SystemMailAddress") ?? "";
        public string SystemMailName => _configuration.GetValue<string>("SystemMailName") ?? "";
        public string MailErrorSubject => _configuration.GetValue<string>("MailErrorSubject") ?? "";
        public string SubjectInformInvitationAccept => _configuration.GetValue<string>("SubjectInformInvitationAccept") ?? "";
        public string SubjectInformInvitationDecline => _configuration.GetValue<string>("SubjectInformInvitationDecline") ?? "";
        public string SubjectInformInvitationExpire => _configuration.GetValue<string>("SubjectInformInvitationExpire") ?? "";
        public string StaticFilesFolder => _configuration.GetValue<string>("StaticFilesFolder") ?? "";
        public string IcsTemplateFile => _configuration.GetValue<string>("IcsTemplateFile") ?? "";
        public string ClassPhotosBasePath => _configuration.GetValue<string>("ClassPhotosBasePath") ?? "";
        public string ClassPhotosPath => _configuration.GetValue<string>("ClassPhotosPath") ?? "";
        public string DefaultUserImage => _configuration.GetValue<string>("DefaultUserImage") ?? "";
        public string SmuELearnHomePage => _configuration.GetValue<string>("SmuElearnHomePage") ?? "";

        public string EvaluationEntryExportGroupByEvaluatorTemplate =>
            _configuration.GetValue<string>("EvaluationEntryExportGroupByEvaluatorTemplate") ?? "";

        public string EvaluationEntryExportGroupByTargetTemplate =>
            _configuration.GetValue<string>("EvaluationEntryExportGroupByTargetTemplate") ?? "";

        public string KeyUserInfo => _configuration.GetValue<string>("KeyUserInfo") ?? "";
        public string SessionUserKey => "UserInfo";
        // key to get role permission from cache. this value is shared
        public string KeyRolePermission = "RolePermission";

        public string KeyUserClassSchedule = "{0}ClassSchedule";

        public string PostdurationForLionWidget => _configuration.GetValue<string>("PostdurationForLionWidget") ?? "";
        public int NoOfPostForLionWidget => Convert.ToInt32(_configuration.GetValue<string>("NoOfPostForLionWidget"));
        public string LionEnv => _configuration.GetValue<string>("LionEnv") ?? "";
        public string LionWebEnv => _configuration.GetValue<string>("LionWebEnv") ?? "";
        public int DefaultOrgUnitId => Convert.ToInt32(_configuration.GetValue<string>("DefaultOrgUnitId"));
        public int DefaultUserOrgUnitId => Convert.ToInt32(_configuration.GetValue<string>("DefaultUserOrgUnitId"));
        public double Timeout => Convert.ToDouble(_configuration.GetValue<string>("Timeout"));

        public string ShowErrorMessage => _configuration.GetValue<string>("ShowErrorMessage") ?? "";
        public string ErrorSessionObjectName = "errorobject";
        public string ValidateLTI => _configuration.GetValue<string>("ValidateLTI") ?? "";
        public string Signature_Host => _configuration.GetValue<string>("Signature_Host") ?? "";
        public int SessionBeforeEnd => Convert.ToInt32(_configuration.GetValue<string>("SessionBeforeEnd"));
        public int MaxAllowedContentLength => Convert.ToInt32(_configuration.GetValue<string>("MaxAllowedContentLength"));
        public string MaxAllowedContentLengthErrorMessage => _configuration.GetValue<string>("MaxAllowedContentLengthErrorMessage") ?? "";
        public string ImageAllowedExtensions => _configuration.GetValue<string>("ImageAllowedExtensions") ?? "";
        public int ImageMaxFileSize => Convert.ToInt32(_configuration.GetValue<string>("ImageMaxFileSize"));
        public string PhotoKey => "SjCXZBmBgbBfDOo";
        public string PeerFeedbackAdmins => _configuration.GetValue<string>("PeerFeedbackAdmins") ?? "";
        public string ProcessOnlyWhitelistedCourses => _configuration.GetValue<string>("ProcessOnlyWhitelistedCourses") ?? "";
        public string PeerFeedBackRoleNameAdmin = "admin";
        public string PeerFeedBackRoleNameStudent = "student";
        public string PeerFeedBackRoleNameInstructor = "instructor";
        public int PeerFeedbackReportQueryTimeout => Convert.ToInt32(_configuration.GetValue<string>("PeerFeedbackReportQueryTimeout", "300"));
        public int PeerFeedbackGetTermsRecentTermsSize => Convert.ToInt32(_configuration.GetValue<string>("PeerFeedbackGetTermsRecentTermsSize", "25"));
        public AcadCareer PeerFeedbackGetTermsEnableUGPG => _configuration.GetValue<AcadCareer>("PeerFeedbackGetTermsEnableUGPG", AcadCareer.Both);

        public bool ShowRatingConfigPage => Convert.ToBoolean(_configuration.GetValue<string>("ShowRatingConfigPage"));
        public bool ShowQuestionConfigPage => Convert.ToBoolean(_configuration.GetValue<string>("ShowQuestionConfigPage"));
        public string YetToMeetExpectations => _configuration.GetValue<string>("YetToMeetExpectations") ?? "";
        public string MeetsExpectations => _configuration.GetValue<string>("MeetsExpectations") ?? "";
        public string ExceedsExpectations => _configuration.GetValue<string>("ExceedsExpectations") ?? "";

        public string ResponsibilityQuestionText => _configuration.GetValue<string>("ResponsibilityQuestion") ?? "";
        public string MeetsExpectationsQuestionText => _configuration.GetValue<string>("MeetsExpectationsQuestion") ?? "";
        public string ExceedsExpectationsQuestionText => _configuration.GetValue<string>("ExceedsExpectationsQuestion") ?? "";

        public string QuestionTypes => _configuration.GetValue<string>("QuestionTypes") ?? "";
        public string GPTZeroApiKey => _configuration.GetValue<string>("GPTZero:ApiKey") ?? "";
        public int GPTZeroMaxItemPerRequest => Convert.ToInt32(_configuration.GetValue<string>("GPTZero:MaxItemPerRequest"));
        public string GPTZeroUrl => _configuration.GetValue<string>("GPTZero:Url") ?? "";
        public bool EnableGPTZeroOption => _configuration.GetValue<bool>("EnableGPTZeroOption");

        public string ToolIdPeerTutoring => "PeerTutoring";
        public string ToolIdPeerFeedback => "PeerFeedback";

        public string PeerTutoringSystemURL => _configuration.GetValue<string>("PeerTutoringSystemURL") ?? "";
        public string ReportPercentageColumns => _configuration.GetValue<string>("ReportPercentageColumns") ?? "";
        public string ReportHideColumns => _configuration.GetValue<string>("ReportHideColumns") ?? "";
        public bool UseFullDbName => _configuration.GetValue<string>("UseFullDbName") == "true";
        public string SurveyUrl => _configuration.GetValue<string>("SurveyUrl") ?? "";

        public string LTI13Validate => _configuration.GetValue<string>("LTI13Validate", "true") ?? "";
        public string LTI13TokenValidationUrl => _configuration.GetValue<string>("LTI13TokenValidationUrl") ?? "";

        // Custom Tool - GPT - Grade Processing Tool
        public bool GptUseNewUITermSelection => Convert.ToBoolean(_configuration.GetValue<string>("GptUseNewUITermSelection", "true"));
    }

    public static class CustomClaimTypes
    {
        public const string CourseId = "CourseId";
    }
    public enum FunctionName
    {
        Ffts = 1,
        InClassSensing = 2,
        Security = 3,
        Cmt = 4,
        Journal = 5,
        Pet = 6,
        RPT = 7,
        Ee = 8,
        Gpt = 9,
        PeerFeedback = 10
    }

    public enum PartialMenuViewName
    {
        _LeftMenu = 1,
        _LeftMenuIcs = 2,
        _LeftMenuAdmin = 3,
        _LeftMenuCMT = 4,
        _LeftMenuJournal = 5,
        _LeftMenuPet = 6,
        _LeftMenuRPT = 7,
        _LeftMenuEE = 8,
        _LeftMenuGPT = 9,
        _LeftMenuPF = 10
    }

    public enum Category
    {
        System = 1,
        Ffts = 2,
        Ics = 3,
        Configuration = 4,
        Cmt = 5,
        Journal = 6,
        Rpt = 7,
        Ee = 8,
        Pet = 9,
        Gpt = 10,
        PeerFeedback = 11,
    }

    public enum InviteUserStatus
    {
        New = 0,
        Modified = 1,
        Remove = 2
    }

    public enum ImageExtension
    {
        Jpeg,
        Jpg
    }

    public enum ImageFolderSize
    {
        Small = 1,
        Medium = 2,
        Large = 3
    }

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

    public enum RoleName
    {
        Instructor = 1,
        Student = 2
    }

    public enum TeamMateFilterType
    {
        None = 1,
        Category = 2,
        CategoryGroup = 3,
        Section = 4,
        Role = 5,
        AllSection = 6
    }

    public enum EvaluationEntryExportType
    {
        GroupByTarget = 1,
        GroupByEvaluator = 2,
        GradeBook = 3
    }

    public enum EvaluationEntryDataExport
    {
        MarkAndComment = 1,
        MarkOnly = 2,
        CommentOnly = 3,
        MarksOnlyinTableFormat = 4
    }

    public enum EvaluationResponseMarks
    {
        Unranked = -2
    }

    public enum MassModerationStatus
    {
        Allowed = 1,
        DisallowedBcReleaseMethod = 2,
        DisallowedBcSubmitted = 3
    }

    public enum ExamExtractionSortBy
    {
        Name = 1,
        OrgDefinedId = 2
    }

    public enum ExamExtractionGroupBy
    {
        Question = 1,
        Student = 2
    }

    public enum ExportOption
    {
        AllToOneDocument = 1,
        OneDocumentPerStudent = 2
    }

    public enum AttendanceUpdateTo
    {
        Attendance = 1,
        Remarks = 2,
        Participation = 3
    }

    public enum IcsNotifyUpdate
    {
        FeedBack = 1,
        LearningPointQuestion = 2,
        LearningPoint = 3,
        Question = 4,
        ResetFeedBack = 5
    }
    public enum PeerFeedBackStatisticDataType
    {
        RateMe = 1,
        RateEachOther = 2,
        MySelf = 3,
    }
    public enum PeerFeedBackResultGroupBy
    {
        AssignedGroup = 1,
        AllCourses = 2
    }

    public enum AuditResourceId
    {
        //User click View Commended Resource in Responsibility and Commitment
        ClickResponsibilityAndCommitment = 1,
        //User click View Commended Resource in Contribution Towards Team Effectiveness
        ClickContributionTowardsTeamEffectiveness = 2,
        //User click View Commended Resource in Towards Team Deliverables
        ClickContributionTowardsTeamDeliverables = 3,
        //User click Confirm in Evaluation page with incomplete evaluation
        ClickConfirmIncompleteEvaluation = 4,
        //User click Confirm in Evaluation page with incomplete evaluation
        ClickSubmitCompleteEvaluation = 5,
        //User click Agree on Term and Condition in Peer Tutoring Register as Tutee
        ClickAgreeTnCForPeerTutoringTutee = 6,
        //User click Agree on Term and Condition in Peer Tutoring Register as Tutor
        ClickAgreeTnCForPeerTutoringTutor = 7,
        //User click Peer Feedback Result Detail
        ViewedEvaluation = 8,
        //To track GPTZero usage
        ExportEEWithAIContentDetector = 9,
    }
    public enum PeerFeedBackReportType
    {
        OverallResponseRate = 0,
        OverallDescriptorResult = 1,
        OverallMeanScoreResult = 2,
        IndividualStudentResult = 3
    }
    public enum PeerFeedBackReportGroupBy
    {
        CourseSection = 0,
        StudentSchool = 1,
    }
}