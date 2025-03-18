using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsIsis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.LMSIsisMapping
{
    public class TL_MergeSectionssMap : IEntityTypeConfiguration<TL_MergeSections>
    {
        public TL_MergeSectionssMap()
        {

        }

        public void Configure(EntityTypeBuilder<TL_MergeSections> builder)
        {
            builder.HasKey(c => c.CourseOfferingCode);
            builder.ToTable("TL_MergeSections");
        }
    }
}
