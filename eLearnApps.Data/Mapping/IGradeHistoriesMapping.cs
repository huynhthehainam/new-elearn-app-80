using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.Mapping
{
    public class IGradeHistoriesMap : IEntityTypeConfiguration<IGradeHistories>
    {
        public IGradeHistoriesMap()
        {

        }

        public void Configure(EntityTypeBuilder<IGradeHistories> builder)
        {
            builder.HasKey(a => a.IGradeHistoryId);
            builder.Property(c => c.IGradeHistoryId).ValueGeneratedOnAdd();
            builder.ToTable("IGradeHistories");
        }
    }
}
