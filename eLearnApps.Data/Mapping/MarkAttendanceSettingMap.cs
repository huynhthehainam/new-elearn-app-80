using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.Mapping
{
    public class MarkAttendanceSettingMap : IEntityTypeConfiguration<MarkAttendanceSetting>
    {
        public MarkAttendanceSettingMap()
        {

        }

        public void Configure(EntityTypeBuilder<MarkAttendanceSetting> builder)
        {
            builder.HasKey(a => a.MarkAttendanceSettingId);
            builder.Property(c => c.MarkAttendanceSettingId).ValueGeneratedOnAdd().HasColumnName("MarkAttendanceSettingId");
            builder.ToTable("MarkAttendanceSetting");
        }
    }
}
