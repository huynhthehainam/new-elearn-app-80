using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.Mapping
{
    internal class GradeObjectCategoryMap : IEntityTypeConfiguration<GradeObjectCategory>
    {
        public GradeObjectCategoryMap()
        {

        }

        public void Configure(EntityTypeBuilder<GradeObjectCategory> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.ToTable("GradeObjectCategory");
        }
    }
}
