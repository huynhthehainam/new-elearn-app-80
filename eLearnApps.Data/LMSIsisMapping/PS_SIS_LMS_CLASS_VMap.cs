using eLearnApps.Entity.LmsIsis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace eLearnApps.Data.LMSIsisMapping
{
    public class PS_SIS_LMS_CLASS_VMap : IEntityTypeConfiguration<PS_SIS_LMS_CLASS_V>
    {
        public PS_SIS_LMS_CLASS_VMap()
        {

        }

        public void Configure(EntityTypeBuilder<PS_SIS_LMS_CLASS_V> builder)
        {
            builder.HasKey(c => new { c.STRM, c.CLASS_NBR, c.ACAD_CAREER, c.ACAD_GROUP });
            builder.ToTable("PS_SIS_LMS_CLASS_V");
        }
    }
}
