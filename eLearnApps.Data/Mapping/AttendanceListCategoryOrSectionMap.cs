using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.Mapping
{
    public class AttendanceListCategoryOrSectionMap : IEntityTypeConfiguration<AttendanceListCategoryOrSection>
    {
        public void Configure(EntityTypeBuilder<AttendanceListCategoryOrSection> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(c => c.Id)
                   .ValueGeneratedOnAdd();
            builder.ToTable("AttendanceListCategoryOrSections");
        }
    }
}
