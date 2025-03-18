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
    public class ClassPhotoSettingMap : IEntityTypeConfiguration<ClassPhotoSetting>
    {

        public void Configure(EntityTypeBuilder<ClassPhotoSetting> builder)
        {
            builder.HasKey(a => a.ClassPhotoSettingId);
            builder.Property(c => c.ClassPhotoSettingId).ValueGeneratedOnAdd().HasColumnName("ClassPhotoSettingID");
            builder.ToTable("ClassPhotoSetting");
        }
    }
}
