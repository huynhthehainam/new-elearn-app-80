using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace eLearnApps.Data.Mapping
{
    public class GradeSubmissionGradesMap : IEntityTypeConfiguration<GradeSubmissionGrades>
    {
        public GradeSubmissionGradesMap()
        {

        }

        public void Configure(EntityTypeBuilder<GradeSubmissionGrades> builder)
        {
            builder.HasKey(a => new { a.GradeSubmissionId, a.StudentId });
            builder.ToTable("GradeSubmissionGrades");
        }
    }
}
