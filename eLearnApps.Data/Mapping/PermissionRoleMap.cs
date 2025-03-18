using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.Mapping
{
    public class PermissionRoleMap : IEntityTypeConfiguration<PermissionRole>
    {
        public PermissionRoleMap()
        {

        }

        public void Configure(EntityTypeBuilder<PermissionRole> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.ToTable("PermissionRole");
        }
    }
}