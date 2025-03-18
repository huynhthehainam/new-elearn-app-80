using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.Mapping
{
    public class MarkRankLabelMap : IEntityTypeConfiguration<MarkRankLabel>
    {
        public MarkRankLabelMap()
        {

        }

        public void Configure(EntityTypeBuilder<MarkRankLabel> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.ToTable("MarkRankLabel");
        }
    }
}