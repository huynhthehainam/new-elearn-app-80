using eLearnApps.Entity.LmsTools;

namespace eLearnApps.Entity.Security
{
    public class StandardPermissionProvider
    {
        #region System
        public static readonly Permission AccessAdmin = new Permission {Name = "Access admin", SystemName = "AccessAdmin", Category = "System"};
        public static readonly Permission ManageAcl = new Permission { Name = "Admin area. Manage ACL", SystemName = "ManageACL", Category = "Configuration" };
        public static readonly Permission ManageSystemLog = new Permission { Name = "Admin area. Manage System Log", SystemName = "ManageSystemLog", Category = "Configuration" };
        public static readonly Permission ExecuteQuery = new Permission { Name = "Admin area. Execute Query", SystemName = "ExecuteQuery", Category = "Configuration" };
        #endregion

        #region FFTS
        public static readonly Permission AccessFfts = new Permission {Name = "Access Ffts", SystemName = "AccessFfts", Category = "Ffts"};
        public static readonly Permission AccessFftsSearch = new Permission { Name = "Access Ffts Search", SystemName = "AccessFftsSearch", Category = "Ffts" };
        #endregion

        #region ICS
        public static readonly Permission ManageIcs = new Permission { Name = "Manage Ics", SystemName = "ManageIcs", Category = "Ics" };
        public static readonly Permission FeedbackIcs = new Permission { Name = "Feedback Ics", SystemName = "FeedbackIcs", Category = "Ics" };
        #endregion

        #region MYJOURNAL
        public static readonly Permission JournalEntries = new Permission { Name = "Journal Entries", SystemName = "JournalEntries", Category = "Journal" };
        public static readonly Permission MyJournal = new Permission { Name = "My Journal", SystemName = "MyJournal", Category = "Journal" };
        public static readonly Permission ManageJournal = new Permission { Name = "Manage Journal", SystemName = "ManageJournal", Category = "Journal" };
        #endregion

        #region CMT
        public static readonly Permission AttendanceList = new Permission { Name = "Access Cmt Attendance List", SystemName = "AttendanceList", Category = "Cmt" };
        public static readonly Permission ClassPhoto = new Permission { Name = "Access Cmt Class Photo", SystemName = "ClassPhoto", Category = "Cmt" };
        public static readonly Permission MyAttendance = new Permission { Name = "Access Cmt My Attendance", SystemName = "MyAttendance", Category = "Cmt" };
        #endregion

        #region EE
        public static readonly Permission ManageExtraction = new Permission { Name = "Manage Extraction", SystemName = "ManageExtraction", Category = "Ee" };
        #endregion

        #region Rpt
        public static readonly Permission AccessRpt = new Permission { Name = "Access Rpt", SystemName = "AccessRpt", Category = "Rpt" };
        public static readonly Permission AccessRptMyResult = new Permission { Name = "Access Rpt My Result", SystemName = "AccessRptMyResult", Category = "Rpt" };
        #endregion

        #region PET
        public static readonly Permission EvaluationEntries = new Permission { Name = "Evaluation Entries", SystemName = "EvaluationEntries", Category = "Pet" };
        public static readonly Permission ManageEvaluation = new Permission { Name = "Manage Evaluation", SystemName = "ManageEvaluation", Category = "Pet" };
        public static readonly Permission MarkLabel = new Permission { Name = "Mark Label", SystemName = "MarkLabel", Category = "Pet" };
        public static readonly Permission MyEvaluation = new Permission { Name = "My Evaluation", SystemName = "MyEvaluation", Category = "Pet" };
        public static readonly Permission MyResult = new Permission { Name = "My Result", SystemName = "MyResult", Category = "Pet" };
        #endregion

        #region GPT
        public static readonly Permission AccessGpt = new Permission { Name = "Access Gpt", SystemName = "Access Gpt", Category = "Gpt" };
        #endregion
    }
}