using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.Mapping
{
    public class GPTPermissionRolesMap : IEntityTypeConfiguration<GPTPermissionRoles>
    {
        public GPTPermissionRolesMap()
        {

        }

        public void Configure(EntityTypeBuilder<GPTPermissionRoles> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.ToTable("GPTPermissionRoles");
        }
    }
}