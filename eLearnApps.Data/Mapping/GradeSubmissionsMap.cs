using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.Mapping
{
    public class GradeSubmissionsMap : IEntityTypeConfiguration<GradeSubmissions>
    {
        public GradeSubmissionsMap()
        {

        }

        public void Configure(EntityTypeBuilder<GradeSubmissions> builder)
        {
            builder.HasKey(a => a.GradeSubmissionId);
            builder.Property(c => c.GradeSubmissionId).ValueGeneratedOnAdd();
            builder.ToTable("GradeSubmissions");
        }
    }
}
