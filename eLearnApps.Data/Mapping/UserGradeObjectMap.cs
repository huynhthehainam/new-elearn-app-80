using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.Mapping
{
    internal class UserGradeObjectMap : IEntityTypeConfiguration<UserGradeObject>
    {
        public UserGradeObjectMap()
        {

        }

        public void Configure(EntityTypeBuilder<UserGradeObject> builder)
        {
            builder.HasKey(a => new { a.UserId, a.GradeObjectId });
            builder.ToTable("UserGradeObject");
        }
    }
}
