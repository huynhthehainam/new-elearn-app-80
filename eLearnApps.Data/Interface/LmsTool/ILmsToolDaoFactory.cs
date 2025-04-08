
namespace eLearnApps.Data.Interface.LmsTool
{
    public interface ILmsToolDaoFactory
    {
        //IGradeObjectDao GradeObjectDao { get; }
        //IGradeObjectCategoryDao GradeObjectCategoryDao { get; }
        //IUserGradeObjectDao UserGradeObjectDao { get; }
        //IGradeModerationDao GradeModerationDao { get; }
        //IGradeModerationMarkDao GradeModerationMarkDao { get; }
        IUserDao UserDao { get; }
        //ISubmissionAcknowledgementnDao SubmissionAcknowledgementDao { get; }
        //IGradeSubmissionDao GradeSubmissionDao { get; }
        //IGradeSubmissionGradeDao GradeSubmissionGradeDao { get; }
        //IGradeResetStatusDao GradeResetStatusDao { get; }
        //IGradeReleaseDao GradeReleaseDao { get; }
        //IIGradesDao IGradesDao { get; }
        //IPRGradeDao PRGradeDao { get; }
        //IPRGradeHistoryDao PRGradeHistoryDao { get; }
        //IGradeReportViewSettingDataAccessDao GradeReportViewSettingDataAccessDao { get; }
        //IJournalDao JournalDao { get; }
        //IJournalEntryDao JournalEntryDao { get; }
        //IJournalEntryCommentDao JournalEntryCommentDao { get; }
        IIcsSessionDao IcsSessionDao { get; }
        IUserEnrollmentDao UserEnrollmentDao { get; }
        IIcsSessionUserSenseDao IcsSessionUserSenseDao { get; }
        IQuestionDao QuestionDao { get; }
        ILearningPointDao LearningPointDao { get; }
        ILearningPointCheckDao LearningPointCheckDao { get; }
        IAttendanceDataDao AttendanceDataDao { get; }
        ICategoryGroupDao CategoryGroupDao { get; }
        //IGptDao GptDao { get; }
        //IAttendanceAttachmentDao AttendanceAttachmentDao { get; }
        //IUserUploadFileDao UserImagesDao { get; }

        //ITAEnrollAckDao TAEnrollAckDao { get; }
        //ISubmittedGradeStatisticDao SubmittedGradeStatisticDao { get; }
        //IPeerFeedBackResponsesDao PeerFeedBackResponsesDao { get; }
        //IPeerFeedBackResponseRemarksDao PeerFeedBackResponseRemarksDao { get; }
        //IPeerTutoringRecordDao PeerTutoringRecordDao { get; }
        //IPeerFeedBackDao PeerFeedBackDao { get; }
    }
}