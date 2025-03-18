using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.Mapping
{
    public class AttendanceDataMap : IEntityTypeConfiguration<AttendanceData>
    {
        public void Configure(EntityTypeBuilder<AttendanceData> builder)
        {
            builder.HasKey(a => a.AttendanceDataId);
            builder.Property(c => c.AttendanceDataId).ValueGeneratedOnAdd();
            builder.ToTable("AttendanceDatas");
        }
    }
}