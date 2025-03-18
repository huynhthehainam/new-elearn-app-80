using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.Mapping
{
    public class GPTReviewHistoryMap : IEntityTypeConfiguration<GPTReviewHistory>
    {
        public GPTReviewHistoryMap()
        {

        }

        public void Configure(EntityTypeBuilder<GPTReviewHistory> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.ToTable("GPTReviewHistory");
        }
    }
}