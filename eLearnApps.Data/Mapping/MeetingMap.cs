using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.Mapping
{
    public class MeetingsMap : IEntityTypeConfiguration<Meeting>
    {
        public MeetingsMap()
        {

        }

        public void Configure(EntityTypeBuilder<Meeting> builder)
        {
            builder.HasKey(a => a.MeetingId);
            builder.Property(c => c.MeetingId).ValueGeneratedOnAdd().HasColumnName("MeetingID");
            builder.ToTable("Meetings");
        }
    }
}