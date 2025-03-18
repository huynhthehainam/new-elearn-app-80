using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.Mapping
{
    public class GradeReleaseSettingMap : IEntityTypeConfiguration<GradeReleaseSetting>
    {
        public GradeReleaseSettingMap()
        {

        }

        public void Configure(EntityTypeBuilder<GradeReleaseSetting> builder)
        {
            builder.HasKey(a => a.GradeReleaseSettingId);
            builder.Property(c => c.GradeReleaseSettingId).ValueGeneratedOnAdd();
            builder.ToTable("GradeReleaseSettings");
        }
    }
}