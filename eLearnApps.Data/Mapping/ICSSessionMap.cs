using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.Mapping
{
    public class ICSSessionMap : IEntityTypeConfiguration<ICSSession>
    {
        public ICSSessionMap()
        {

        }

        public void Configure(EntityTypeBuilder<ICSSession> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.ToTable("ICSSessions");
        }
    }
}
