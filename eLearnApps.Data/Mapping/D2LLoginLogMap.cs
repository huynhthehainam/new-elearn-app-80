using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.Mapping
{
    public class D2LLoginLogMap : IEntityTypeConfiguration<D2LLoginLog>
    {
        public D2LLoginLogMap()
        {

        }

        public void Configure(EntityTypeBuilder<D2LLoginLog> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.ToTable("D2LLoginLog");
        }
    }
}
