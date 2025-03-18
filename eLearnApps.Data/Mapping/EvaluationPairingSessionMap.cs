using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.Mapping
{
    public class EvaluationPairingSessionMap : IEntityTypeConfiguration<EvaluationPairingSession>
    {
        public EvaluationPairingSessionMap()
        {

        }

        public void Configure(EntityTypeBuilder<EvaluationPairingSession> builder)
        {
            builder.HasKey(a => a.EvaluationPairingSessionId);
            builder.Property(c => c.EvaluationPairingSessionId).ValueGeneratedOnAdd();
            builder.ToTable("EvaluationPairingSessions");
        }
    }
}