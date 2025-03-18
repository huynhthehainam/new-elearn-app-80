using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.Mapping
{
    public class GradeReportViewSettingDataAccess : IEntityTypeConfiguration<GradeReportViewSetting>
    {
        public GradeReportViewSettingDataAccess()
        {

        }

        public void Configure(EntityTypeBuilder<GradeReportViewSetting> builder)
        {
            builder.HasKey(a => new { a.OrgUnitId, a.UserId, a.GradeObjectId });
            builder.ToTable("GradeReportViewSettings");
        }
    }
}