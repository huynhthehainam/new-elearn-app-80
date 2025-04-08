using eLearnApps.Data.Interface;
using eLearnApps.Data.Interface.Lmsisis;
using eLearnApps.Data.Interface.LmsTool;
using eLearnApps.Data.Interface.Logging;
using eLearnApps.Data.Lmsisis;
using eLearnApps.Data.LmsTool;
using eLearnApps.Data.Logging;
using Microsoft.Extensions.Configuration;

namespace eLearnApps.Data
{
    public class DaoFactory : IDaoFactory
    {
        private readonly IConfiguration _configuration;
        private readonly DataContext _dataContext;
        public DaoFactory(IConfiguration configuration, DataContext dataContext)
        {
            _configuration = configuration;
            _dataContext = dataContext;
        }
        //public IGradeObjectDao GradeObjectDao => new GradeObjectDao(_configuration);
        //public IGradeObjectCategoryDao GradeObjectCategoryDao => new GradeObjectCategoryDao(_configuration);
        //public IUserGradeObjectDao UserGradeObjectDao => new UserGradeObjectDao(_configuration);
        //public IGradeModerationDao GradeModerationDao => new GradeModerationDao(_configuration);
        //public IGradeModerationMarkDao GradeModerationMarkDao => new GradeModerationMarkDao(_configuration);
        public IAuditLogDao AuditLogDao => new AuditLogDao(_configuration);
        public IGPTAuditLogDao GPTAuditLogDao => new GPTAuditLogDao(_configuration);
        public IGPTDebugLogDao GPTDebugLogDao => new GPTDebugLogDao(_configuration);
        //public IGptDao GptDao => new GptDao(_configuration);
        public IBatchJobLogDao BatchJobLogDao => new BatchJobLogDao(_configuration);
        public IBatchJobLogDetailDao BatchJobLogDetailDao => new BatchJobLogDetailDao(_configuration);
        public IErrorLogDao ErrorLogDao => new ErrorLogDao(_configuration);
        public IToolAccessLogDao ToolAccessLogDao => new ToolAccessLogDao(_configuration);
        //public ITLMergeSectionDao TLMergeSectionDao => new TLMergeSectionDao(_configuration);
        //public IPsSisLmsGradeTypeDao PsSisLmsGradeTypeDao => new PsSisLmsGradeTypeDao(_configuration);
        public IPS_SIS_LMS_PHOTO_VDao PS_SIS_LMS_PHOTO_VDao => new PS_SIS_LMS_PHOTO_VDao(_configuration);
        //public ITLCourseOfferingDao TLCourseOfferingDao => new TLCourseOfferingDao(_configuration);
        public IUserDao UserDao => new UserDao(_configuration);
        //public ITAEnrollAckDao TAEnrollAckDao => new TAEnrollAckDao(_configuration);

        //public IGradeSubmissionDao GradeSubmissionDao => new GradeSubmissionDao(_configuration);
        //public IGradeSubmissionGradeDao GradeSubmissionGradeDao => new GradeSubmissionGradeDao(_configuration);
        //public IGradeResetStatusDao GradeResetStatusDao => new GradeResetStatusDao(_configuration);
        //public IGradeReleaseDao GradeReleaseDao => new GradeReleaseDao(_configuration);
        //public IIGradesDao IGradesDao => new IGradesDao(_configuration);
        //public IPRGradeDao PRGradeDao => new PRGradeDao(_configuration);
        //public IPRGradeHistoryDao PRGradeHistoryDao => new PRGradeHistoryDao(_configuration);
        //public IGradeReportViewSettingDataAccessDao GradeReportViewSettingDataAccessDao => new GradeReportViewSettingDataAccessDao(_configuration, _dataContext);
        //public ISubmissionAcknowledgementnDao SubmissionAcknowledgementDao => new SubmissionAcknowledgementDao(_configuration);
        //public ISubmittedGradeStatisticDao SubmittedGradeStatisticDao => new SubmittedGradeStatisticDao(_configuration);

        //public IJournalDao JournalDao => new JournalDao(_configuration);
        //public IJournalEntryDao JournalEntryDao => new JournalEntryDao(_configuration);
        //public IJournalEntryCommentDao JournalEntryCommentDao => new JournalEntryCommentDao(_configuration);
        public IIcsSessionDao IcsSessionDao => new IcsSessionDao(_configuration);
        public IUserEnrollmentDao UserEnrollmentDao => new UserEnrollmentDao(_configuration);
        public IIcsSessionUserSenseDao IcsSessionUserSenseDao => new IcsSessionUserSenseDao(_configuration);
        public IQuestionDao QuestionDao => new QuestionDao(_configuration);
        public ILearningPointDao LearningPointDao => new LearningPointDao(_configuration);
        public ILearningPointCheckDao LearningPointCheckDao => new LearningPointCheckDao(_configuration);
        public IAttendanceDataDao AttendanceDataDao => new AttendanceDataDao(_configuration);
        public ICategoryGroupDao CategoryGroupDao => new CategoryGroupDao(_configuration);
        //public IAttendanceAttachmentDao AttendanceAttachmentDao => new AttendanceAttachmentDao(_configuration);
        //public IUserUploadFileDao UserImagesDao => new UserUploadFileDao(_configuration);
        public IDebugLogDao DebugLogDao => new DebugLogDao(_configuration);
        //public IPeerFeedBackResponsesDao PeerFeedBackResponsesDao => new PeerFeedBackResponsesDao(_configuration);
        //public IPeerFeedBackResponseRemarksDao PeerFeedBackResponseRemarksDao => new PeerFeedBackResponseRemarksDao(_configuration);
        public IAuditDao AuditDao => new AuditDao(_configuration);
        //public IPeerTutoringRecordDao PeerTutoringRecordDao => new PeerTutoringRecordDao(_configuration);
        //public IPeerFeedBackDao PeerFeedBackDao => new PeerFeedBackDao(_configuration);
    }
}