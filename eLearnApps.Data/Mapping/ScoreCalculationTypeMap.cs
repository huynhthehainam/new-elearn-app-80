using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.Mapping
{
    public class ScoreCalculationTypeMap : IEntityTypeConfiguration<ScoreCalculationType>
    {
        public ScoreCalculationTypeMap()
        {

        }

        public void Configure(EntityTypeBuilder<ScoreCalculationType> builder)
        {
            builder.HasKey(a => a.ScoreCalculationTypeId);
            builder.Property(c => c.ScoreCalculationTypeId).ValueGeneratedNever();

            builder.ToTable("ScoreCalculationTypes");
        }
    }
}