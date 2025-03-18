using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace eLearnApps.Data.Mapping
{
    public class PeerFeedbackRatingQuestionMap : IEntityTypeConfiguration<PeerFeedbackRatingQuestion>
    {
        public PeerFeedbackRatingQuestionMap()
        {

        }

        public void Configure(EntityTypeBuilder<PeerFeedbackRatingQuestion> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.ToTable("PeerFeedbackRatingQuestion");
        }
    }
    public class PeerFeedbackRatingOptionMap : IEntityTypeConfiguration<PeerFeedbackRatingOption>
    {
        public PeerFeedbackRatingOptionMap()
        {

        }

        public void Configure(EntityTypeBuilder<PeerFeedbackRatingOption> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.ToTable("PeerFeedbackRatingOption");
        }
    }
    public class PeerFeedbackRatingQuestionEntityMap : IEntityTypeConfiguration<PeerFeedbackRatingMap>
    {
        public PeerFeedbackRatingQuestionEntityMap()
        {

        }

        public void Configure(EntityTypeBuilder<PeerFeedbackRatingMap> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.ToTable("PeerFeedbackRatingMap");
        }
    }

    public class PeerFeedbackQuestionMap : IEntityTypeConfiguration<PeerFeedbackQuestion>
    {
        public PeerFeedbackQuestionMap()
        {

        }

        public void Configure(EntityTypeBuilder<PeerFeedbackQuestion> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.ToTable("PeerFeedbackQuestion");
        }
    }

    public class PeerFeedbackQuestionRatingEntityMap : IEntityTypeConfiguration<PeerFeedbackQuestionRatingMap>
    {
        public PeerFeedbackQuestionRatingEntityMap()
        {

        }

        public void Configure(EntityTypeBuilder<PeerFeedbackQuestionRatingMap> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.ToTable("PeerFeedbackQuestionRatingMap");
        }
    }
}