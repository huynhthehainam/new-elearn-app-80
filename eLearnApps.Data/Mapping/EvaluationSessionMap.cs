using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.Mapping
{
    public class EvaluationSessionMap : IEntityTypeConfiguration<EvaluationSession>
    {
        public EvaluationSessionMap()
        {

        }

        public void Configure(EntityTypeBuilder<EvaluationSession> builder)
        {
            builder.HasKey(a => a.EvaluationSessionId);
            builder.Property(c => c.EvaluationSessionId).ValueGeneratedOnAdd();
            builder.ToTable("EvaluationSessions");
        }
    }
}