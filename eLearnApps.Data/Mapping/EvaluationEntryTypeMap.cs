using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.Mapping
{
    public class EvaluationEntryTypeMap : IEntityTypeConfiguration<EvaluationEntryType>
    {
        public EvaluationEntryTypeMap()
        {

        }

        public void Configure(EntityTypeBuilder<EvaluationEntryType> builder)
        {
            builder.HasKey(a => a.EvaluationEntryTypeId);
            builder.Property(c => c.EvaluationEntryTypeId).ValueGeneratedOnAdd();
            builder.ToTable("EvaluationEntryTypes");
        }
    }
}