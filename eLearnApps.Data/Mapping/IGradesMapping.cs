using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.Mapping
{
    public class IGradesMap : IEntityTypeConfiguration<IGrades>
    {
        public IGradesMap()
        {

        }

        public void Configure(EntityTypeBuilder<IGrades> builder)
        {
            builder.HasKey(a => a.IGradeId);
            builder.Property(c => c.IGradeId).ValueGeneratedOnAdd();
            builder.ToTable("IGrades");
        }
    }
}
