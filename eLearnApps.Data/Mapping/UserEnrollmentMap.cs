using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.Mapping
{
    public class UserEnrollmentMap : IEntityTypeConfiguration<UserEnrollment>
    {
        public UserEnrollmentMap()
        {

        }

        public void Configure(EntityTypeBuilder<UserEnrollment> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.ToTable("UserEnrollments");
        }
    }
}