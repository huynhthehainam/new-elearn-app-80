using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.Mapping
{
    public class GradeReleaseMap : IEntityTypeConfiguration<GradeRelease>
    {
        public GradeReleaseMap()
        {

        }

        public void Configure(EntityTypeBuilder<GradeRelease> builder)
        {
            builder.HasKey(a => a.GradeReleaseId);
            builder.Property(c => c.GradeReleaseId).ValueGeneratedOnAdd();
            builder.ToTable("GradeReleases");
        }
    }
}