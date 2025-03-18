using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.Mapping
{
    public class MeetingAttendeMap : IEntityTypeConfiguration<MeetingAttendee>
    {
        public MeetingAttendeMap()
        {

        }

        public void Configure(EntityTypeBuilder<MeetingAttendee> builder)
        {
            builder.HasKey(a => a.MeetingAttendeeId);
            builder.Ignore(c => c.Email);
            builder.Property(c => c.MeetingAttendeeId).ValueGeneratedOnAdd();
            builder.ToTable("MeetingAttendees");
        }
    }
}