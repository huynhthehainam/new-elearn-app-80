using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsIsis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.LMSIsisMapping
{
    public class PS_SIS_LMS_GRADE_TYPEMap : IEntityTypeConfiguration<PS_SIS_LMS_GRADE_TYPE>
    {
        public PS_SIS_LMS_GRADE_TYPEMap()
        {

        }

        public void Configure(EntityTypeBuilder<PS_SIS_LMS_GRADE_TYPE> builder)
        {
            builder.HasKey(c => c.CourseOfferingCode);
            builder.ToTable("PS_SIS_LMS_GRADE_TYPE");
        }
    }
}
