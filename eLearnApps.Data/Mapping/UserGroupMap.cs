using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.Mapping
{
    public class UserGroupMap : IEntityTypeConfiguration<UserGroup>
    {
        public UserGroupMap()
        {

        }

        public void Configure(EntityTypeBuilder<UserGroup> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.ToTable("UserGroups");
        }
    }
}
