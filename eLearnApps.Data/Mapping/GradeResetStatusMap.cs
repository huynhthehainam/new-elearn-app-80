using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.Mapping
{
    public class GradeResetStatusMap : IEntityTypeConfiguration<GradeResetStatus>
    {
        public GradeResetStatusMap()
        {

        }

        public void Configure(EntityTypeBuilder<GradeResetStatus> builder)
        {
            builder.HasKey(a => a.GradeSubmissionId);
            builder.Property(c => c.GradeSubmissionId).ValueGeneratedNever();
            builder.ToTable("GradeResetStatus");
        }
    }
}
