using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.Mapping
{
    public class PermissionMap : IEntityTypeConfiguration<Permission>
    {
        public PermissionMap()
        {

        }

        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.ToTable("Permission");
        }
    }
}