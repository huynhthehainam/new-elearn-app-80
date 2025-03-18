using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.Mapping
{
    public class AttendanceSessionMap : IEntityTypeConfiguration<AttendanceSession>
    {
        public void Configure(EntityTypeBuilder<AttendanceSession> builder)
        {
            builder.HasKey(a => a.AttendanceSessionId);
            builder.Property(c => c.AttendanceSessionId).ValueGeneratedOnAdd();
            builder.ToTable("AttendanceSessions");
        }
    }
}