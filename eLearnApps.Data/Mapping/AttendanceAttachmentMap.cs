using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.Mapping
{
    internal class AttendanceAttachmentMap : IEntityTypeConfiguration<AttendanceAttachment>
    {

        public void Configure(EntityTypeBuilder<AttendanceAttachment> builder)
        {
            builder.HasKey(a => a.AttendanceAttachmentId);
            builder.Property(c => c.AttendanceAttachmentId).ValueGeneratedOnAdd();
            builder.ToTable("AttendanceAttachments");
        }
    }
}