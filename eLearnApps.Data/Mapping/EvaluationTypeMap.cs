using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.Mapping
{
    public class EvaluationTypeMap : IEntityTypeConfiguration<EvaluationType>
    {
        public EvaluationTypeMap()
        {

        }

        public void Configure(EntityTypeBuilder<EvaluationType> builder)
        {
            builder.HasKey(a => a.EvaluationTypeId);
            builder.Property(c => c.EvaluationTypeId).ValueGeneratedOnAdd();
            builder.ToTable("EvaluationTypes");
        }
    }
}