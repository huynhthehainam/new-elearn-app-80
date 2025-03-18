using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.Mapping
{
    public class LearningPointMap : IEntityTypeConfiguration<LearningPoint>
    {
        public LearningPointMap()
        {

        }

        public void Configure(EntityTypeBuilder<LearningPoint> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.ToTable("LearningPoints");
        }
    }
}
