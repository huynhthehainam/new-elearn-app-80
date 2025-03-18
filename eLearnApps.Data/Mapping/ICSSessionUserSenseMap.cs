using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.Mapping
{
    public class ICSSessionUserSenseMap : IEntityTypeConfiguration<ICSSessionUserSense>
    {
        public ICSSessionUserSenseMap()
        {

        }

        public void Configure(EntityTypeBuilder<ICSSessionUserSense> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.ToTable("ICSSessionUserSenses");
        }
    }
}
