using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.Mapping
{
    public class EvaluationReleaseSettingMap : IEntityTypeConfiguration<EvaluationReleaseSetting>
    {
        public EvaluationReleaseSettingMap()
        {

        }

        public void Configure(EntityTypeBuilder<EvaluationReleaseSetting> builder)
        {
            builder.HasKey(a => a.EvaluationReleaseSettingId);
            builder.Property(c => c.EvaluationReleaseSettingId).ValueGeneratedOnAdd();
            builder.ToTable("EvaluationReleaseSettings");
        }
    }
}