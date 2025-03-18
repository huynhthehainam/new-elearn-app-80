using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.Mapping
{
    public class GPTSpecialAccessMap : IEntityTypeConfiguration<GPTSpecialAccess>
    {
        public GPTSpecialAccessMap()
        {

        }

        public void Configure(EntityTypeBuilder<GPTSpecialAccess> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.ToTable("GPTSpecialAccess");
        }
    }
}