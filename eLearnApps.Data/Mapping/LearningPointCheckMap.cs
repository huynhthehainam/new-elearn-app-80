using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.Mapping
{
    public class LearningPointCheckMap : IEntityTypeConfiguration<LearningPointCheck>
    {
        public LearningPointCheckMap()
        {

        }

        public void Configure(EntityTypeBuilder<LearningPointCheck> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.ToTable("LearningPointChecks");
        }
    }
}
