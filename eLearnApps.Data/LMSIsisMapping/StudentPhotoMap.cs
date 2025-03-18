using eLearnApps.Entity.LmsIsis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.Data.LMSIsisMapping
{
    public class StudentPhotoMap : IEntityTypeConfiguration<StudentPhoto>
    {
        public StudentPhotoMap()
        {

        }

        public void Configure(EntityTypeBuilder<StudentPhoto> builder)
        {
            builder.HasKey(c => c.PersonId);
            builder.ToTable("StudentPhoto");
        }
    }
}
