using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.Mapping
{
    public class AttendanceListMap : IEntityTypeConfiguration<AttendanceList>
    {
        public void Configure(EntityTypeBuilder<AttendanceList> builder)
        {
            builder.HasKey(a => a.AttendanceListId);
            builder.Property(c => c.AttendanceListId).ValueGeneratedOnAdd();
            builder.ToTable("AttendanceLists"); 
        }
    }
}