using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.Mapping
{
    public class EvaluationMap : IEntityTypeConfiguration<Evaluation>
    {
        public EvaluationMap()
        {

        }

        public void Configure(EntityTypeBuilder<Evaluation> builder)
        {
            builder.HasKey(a => a.EvaluationId);
            builder.Property(c => c.EvaluationId).ValueGeneratedOnAdd();
            builder.ToTable("Evaluations");
        }
    }
}