using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.Mapping
{
    internal class MarkRankLabelDetailMap : IEntityTypeConfiguration<MarkRankLabelDetail>
    {
        public MarkRankLabelDetailMap()
        {

        }

        public void Configure(EntityTypeBuilder<MarkRankLabelDetail> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.ToTable("MarkRankLabelDetail");
        }
    }
}