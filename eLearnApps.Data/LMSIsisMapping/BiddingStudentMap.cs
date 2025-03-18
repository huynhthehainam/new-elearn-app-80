using eLearnApps.Entity.LmsIsis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.LMSIsisMapping
{
    internal class BiddingStudentMap : IEntityTypeConfiguration<BiddingStudent>
    {
        public BiddingStudentMap()
        {

        }

        public void Configure(EntityTypeBuilder<BiddingStudent> builder)
        {
            builder.HasKey(c => new { c.CourseOfferingCode, c.Nricfin });
            builder.ToTable("BiddingStudents");
        }
    }
}