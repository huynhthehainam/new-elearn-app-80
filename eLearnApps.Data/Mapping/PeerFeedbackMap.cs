using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.Mapping
{
    public class PeerFeedbackMap : IEntityTypeConfiguration<PeerFeedback>
    {
        public PeerFeedbackMap()
        {

        }

        public void Configure(EntityTypeBuilder<PeerFeedback> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.ToTable("PeerFeedback");
        }
    }

    public class PeerFeedbackSessionsMap : IEntityTypeConfiguration<PeerFeedbackSessions>
    {
        public PeerFeedbackSessionsMap()
        {

        }

        public void Configure(EntityTypeBuilder<PeerFeedbackSessions> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.ToTable("PeerFeedbackSessions");
        }
    }

    public class PeerFeedbackEvaluatorsMap : IEntityTypeConfiguration<PeerFeedbackEvaluators>
    {
        public PeerFeedbackEvaluatorsMap()
        {

        }

        public void Configure(EntityTypeBuilder<PeerFeedbackEvaluators> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.ToTable("PeerFeedbackEvaluators");
        }
    }

    public class PeerFeedbackPairingsMap : IEntityTypeConfiguration<PeerFeedbackPairings>
    {
        public PeerFeedbackPairingsMap()
        {

        }

        public void Configure(EntityTypeBuilder<PeerFeedbackPairings> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.ToTable("PeerFeedbackPairings");
        }
    }

    public class PeerFeedbackTargetsMap : IEntityTypeConfiguration<PeerFeedbackTargets>
    {
        public PeerFeedbackTargetsMap()
        {

        }

        public void Configure(EntityTypeBuilder<PeerFeedbackTargets> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.ToTable("PeerFeedbackTargets");
        }
    }
    public class PeerFeedbackQuestionEntityMap : IEntityTypeConfiguration<Entity.LmsTools.PeerFeedbackQuestionMap>
    {
        public PeerFeedbackQuestionEntityMap()
        {

        }

        public void Configure(EntityTypeBuilder<Entity.LmsTools.PeerFeedbackQuestionMap> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.ToTable("PeerFeedbackQuestionMap");
        }
    }
    public class PeerFeedBackPairingSessionsMap : IEntityTypeConfiguration<Entity.LmsTools.PeerFeedBackPairingSessions>
    {
        public PeerFeedBackPairingSessionsMap()
        {

        }

        public void Configure(EntityTypeBuilder<PeerFeedBackPairingSessions> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.ToTable("PeerFeedBackPairingSessions");
        }
    }
    public class PeerFeedBackResponsesMap : IEntityTypeConfiguration<Entity.LmsTools.PeerFeedBackResponses>
    {
        public PeerFeedBackResponsesMap()
        {

        }

        public void Configure(EntityTypeBuilder<PeerFeedBackResponses> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.ToTable("PeerFeedBackResponses");
        }
    }
    public class PeerFeedBackResponseRemarksMap : IEntityTypeConfiguration<Entity.LmsTools.PeerFeedBackResponseRemarks>
    {
        public PeerFeedBackResponseRemarksMap()
        {

        }

        public void Configure(EntityTypeBuilder<PeerFeedBackResponseRemarks> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.ToTable("PeerFeedBackResponseRemarks");
        }
    }
}