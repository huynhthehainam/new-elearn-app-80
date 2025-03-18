using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.Mapping
{
    public class GPTApprovalHistoryMap : IEntityTypeConfiguration<GPTApprovalHistory>
    {
        public GPTApprovalHistoryMap()
        {

        }

        public void Configure(EntityTypeBuilder<GPTApprovalHistory> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.ToTable("GPTApprovalHistory");
        }
    }
}