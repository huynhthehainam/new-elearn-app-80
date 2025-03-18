using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsIsis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.LMSIsisMapping
{
    public class PS_SIS_LMS_SCHED_VMap : IEntityTypeConfiguration<PS_SIS_LMS_SCHED_V>
    {
        public PS_SIS_LMS_SCHED_VMap()
        {

        }

        public void Configure(EntityTypeBuilder<PS_SIS_LMS_SCHED_V> builder)
        {
            builder.HasKey(c => new { c.STRM, c.CLASS_NBR, c.CLASS_MTG_NBR });
            builder.ToTable("PS_SIS_LMS_SCHED_V");
        }
    }
}
