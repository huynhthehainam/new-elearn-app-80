using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.Mapping
{
    public class CourseTemplateMap : IEntityTypeConfiguration<CourseTemplate>
    {
        public CourseTemplateMap()
        {

        }

        public void Configure(EntityTypeBuilder<CourseTemplate> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.ToTable("CourseTemplates");
        }
    }
}