using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.Mapping
{
    public class EvaluatorMap : IEntityTypeConfiguration<Evaluator>
    {
        public EvaluatorMap()
        {

        }

        public void Configure(EntityTypeBuilder<Evaluator> builder)
        {
            builder.HasKey(a => a.EvaluatorId);
            builder.Property(c => c.EvaluatorId).ValueGeneratedOnAdd();
            builder.ToTable("Evaluators");
        }
    }
}