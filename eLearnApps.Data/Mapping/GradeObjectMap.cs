﻿using System.ComponentModel.DataAnnotations.Schema;
using eLearnApps.Entity.LmsTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eLearnApps.Data.Mapping
{
    internal class GradeObjectMap : IEntityTypeConfiguration<GradeObject>
    {
        public GradeObjectMap()
        {

        }

        public void Configure(EntityTypeBuilder<GradeObject> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.ToTable("GradeObject");
        }
    }
}
