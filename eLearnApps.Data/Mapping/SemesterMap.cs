using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.Mapping
{
    public class SemesterMap : IEntityTypeConfiguration<Semester>
    {
        public SemesterMap()
        {

        }

        public void Configure(EntityTypeBuilder<Semester> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(c => c.Id).ValueGeneratedNever();
            builder.ToTable("Semesters");
        }
    }
}