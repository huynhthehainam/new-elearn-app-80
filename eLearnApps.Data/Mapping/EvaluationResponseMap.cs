using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.Mapping
{
    public class EvaluationResponseMap : IEntityTypeConfiguration<EvaluationResponse>
    {
        public EvaluationResponseMap()
        {

        }

        public void Configure(EntityTypeBuilder<EvaluationResponse> builder)
        {
            builder.HasKey(a => a.EvaluationResponseId);
            builder.Property(c => c.EvaluationResponseId).ValueGeneratedOnAdd();
            builder.ToTable("EvaluationResponses");
        }
    }
}