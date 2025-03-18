using eLearnApps.Entity.LmsIsis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.LMSIsisMapping
{
    public class CoursesMapping : IEntityTypeConfiguration<Courses>
    {

        public void Configure(EntityTypeBuilder<Courses> builder)
        {
            builder.HasKey(c => c.Id);
            builder.ToTable("Courses");
        }
    }
}
