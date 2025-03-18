using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.Mapping
{
    public class EvaluationQuestionMap : IEntityTypeConfiguration<EvaluationQuestion>
    {
        public EvaluationQuestionMap()
        {

        }

        public void Configure(EntityTypeBuilder<EvaluationQuestion> builder)
        {
            builder.HasKey(a => a.EvaluationQuestionId);
            builder.Property(c => c.EvaluationQuestionId).ValueGeneratedOnAdd();
            builder.ToTable("EvaluationQuestions");
        }
    }
}