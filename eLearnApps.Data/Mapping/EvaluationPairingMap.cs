using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.Mapping
{
    public class EvaluationPairingMap : IEntityTypeConfiguration<EvaluationPairing>
    {
        public EvaluationPairingMap()
        {

        }

        public void Configure(EntityTypeBuilder<EvaluationPairing> builder)
        {
            builder.HasKey(a => a.EvaluationPairingId);
            builder.Property(c => c.EvaluationPairingId).ValueGeneratedOnAdd();
            builder.ToTable("EvaluationPairings");
        }
    }
}