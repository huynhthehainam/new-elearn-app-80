using eLearnApps.Entity.LmsIsis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.LMSIsisMapping
{
    public class MergeSectionMap : IEntityTypeConfiguration<MergeSection>
    {
        public MergeSectionMap()
        {
        }

        public void Configure(EntityTypeBuilder<MergeSection> builder)
        {
            builder.HasKey(c => c.IndvCourseOfferingCode);
            builder.ToTable("MergeSections");
        }
    }
}