using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.Mapping
{
    public class EvaluationTargetMap : IEntityTypeConfiguration<EvaluationTarget>
    {
        public EvaluationTargetMap()
        {

        }

        public void Configure(EntityTypeBuilder<EvaluationTarget> builder)
        {
            builder.HasKey(a => a.EvaluationTargetId);
            builder.Property(c => c.EvaluationTargetId).ValueGeneratedOnAdd();
            builder.ToTable("EvaluationTargets");
        }
    }
}