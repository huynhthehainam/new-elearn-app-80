using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.Mapping
{
    public class EvaluationQuestionsLabelSchemeMap : IEntityTypeConfiguration<EvaluationQuestionsLabelScheme>
    {
        public EvaluationQuestionsLabelSchemeMap()
        {

        }

        public void Configure(EntityTypeBuilder<EvaluationQuestionsLabelScheme> builder)
        {
            builder.HasKey(a => a.EvaluationQuestionsLabelSchemesId);
            builder.Property(c => c.EvaluationQuestionsLabelSchemesId).ValueGeneratedOnAdd();
            builder.ToTable("EvaluationQuestionsLabelSchemes");
        }
    }
}