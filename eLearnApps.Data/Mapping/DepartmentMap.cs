using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.Mapping
{
    public class DepartmentMap : IEntityTypeConfiguration<Department>
    {
        public DepartmentMap()
        {

        }

        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.ToTable("Departments");
        }
    }
}