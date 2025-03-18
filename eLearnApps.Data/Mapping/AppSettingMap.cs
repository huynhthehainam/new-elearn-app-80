using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.Mapping
{
    public class AppSettingMap : IEntityTypeConfiguration<AppSetting>
    {

        public void Configure(EntityTypeBuilder<AppSetting> builder)
        {
            builder.HasKey(a => a.AppSettingId);
            builder.Property(c => c.AppSettingId).ValueGeneratedOnAdd();
            builder.ToTable("AppSettings");
        }
    }
}