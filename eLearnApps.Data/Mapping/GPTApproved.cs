using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.Mapping
{
    public class GPTApprovedMap : IEntityTypeConfiguration<GPTApproved>
    {
        public GPTApprovedMap()
        {

        }

        public void Configure(EntityTypeBuilder<GPTApproved> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.ToTable("GPTApproved");
        }
    }
}