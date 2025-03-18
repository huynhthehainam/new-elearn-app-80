using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsIsis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.LMSIsisMapping
{
    public class TL_CourseOfferingsMap : IEntityTypeConfiguration<TL_CourseOfferings>
    {
        public TL_CourseOfferingsMap()
        {

        }

        public void Configure(EntityTypeBuilder<TL_CourseOfferings> builder)
        {
            builder.HasKey(c => c.CourseOfferingCode);
            builder.ToTable("TL_CourseOfferings");
        }
    }
}
