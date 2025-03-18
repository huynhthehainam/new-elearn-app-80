using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.Mapping
{
    public class JournalEntryMap : IEntityTypeConfiguration<JournalEntry>
    {
        public JournalEntryMap()
        {

        }

        public void Configure(EntityTypeBuilder<JournalEntry> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.ToTable("JournalEntries");
        }
    }
}