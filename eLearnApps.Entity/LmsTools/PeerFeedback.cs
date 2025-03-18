using System;

namespace eLearnApps.Entity.LmsTools
{
    public class PeerFeedback : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int TypeId { get; set; }
        public int LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedTime { get; set; }
        public DateTime? CreatedTime { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class PeerFeedbackSessions : BaseEntity
    {
        public int Id { get; set; }
        public DateTime EntryStartTime { get; set; }
        public DateTime EntryCloseTime { get; set; }
        public int LastUpdatedBy { get; set; }
        public DateTime LastUpdatedTime { get; set; }
        public bool IsDeleted { get; set; }
        public string Strm { get; set; } = string.Empty;
        public string? Label { get; set; }
        public int PeerFeedbackId { get; set; }
        public DateTime? DueDate { get; set; }
        public string? CourseOfferingCode { get; set; }
    }

    public class PeerFeedbackEvaluators : BaseEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int OrgUnitId { get; set; }
        public bool IsOrgUnit { get; set; }
        public bool IsDeleted { get; set; }
        public int LastUpdatedBy { get; set; }
        public DateTime LastUpdatedTime { get; set; }
        public int PeerFeedbackPairingId { get; set; }
    }

    public class PeerFeedbackPairings : BaseEntity
    {
        public int Id { get; set; }
        public int PeerFeedbackId { get; set; }
        public int LastUpdatedBy { get; set; }
        public DateTime LastUpdatedTime { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class PeerFeedbackTargets : BaseEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int OrgUnitId { get; set; }
        public bool IsOrgUnit { get; set; }
        public bool IsDeleted { get; set; }
        public int LastUpdatedBy { get; set; }
        public DateTime LastUpdatedTime { get; set; }
        public int PeerFeedbackPairingId { get; set; }

    }

    public class PeerFeedbackQuestionMap : BaseEntity
    {
        public int Id { get; set; }
        public int PeerFeedbackId { get; set; }
        public int PeerFeedbackQuestionId { get; set; }
    }

    public class PeerFeedBackResponses : BaseEntity
    {
        public int Id { get; set; }
        public int PeerFeedbackId { get; set; }
        public int PeerFeedbackQuestionId { get; set; }
        public int PeerFeedbackSessionId { get; set; }
        public int TargetUserId { get; set; }
        public int EvaluatorUserId { get; set; }
        public DateTime LastUpdateTime { get; set; }
        public bool IsDeleted { get; set; }
        public int? PeerFeedBackRatingId { get; set; }
        public int? PeerFeedBackOptionId { get; set; }
        public int PeerFeedBackGroupId { get; set; }
    }
    public class PeerFeedBackResponseRemarks : BaseEntity
    {
        public int Id { get; set; }
        public int PeerFeedbackId { get; set; }
        public int PeerFeedbackSessionId { get; set; }
        public int TargetUserId { get; set; }
        public int EvaluatorUserId { get; set; }
        public DateTime LastUpdateTime { get; set; }
        public bool IsDeleted { get; set; }
        public int PeerFeedBackGroupId { get; set; }
        public string? Remarks { get; set; }
    }
}