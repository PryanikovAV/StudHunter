﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudHunter.DB.Postgres.Models;
namespace StudHunter.DB.Postgres.Configurations;

public class VacancyCourseConfiguration : IEntityTypeConfiguration<VacancyCourse>
{
    public void Configure(EntityTypeBuilder<VacancyCourse> builder)
    {
        builder.HasKey(vc => new
        {
            vc.CourseId,
            vc.VacancyId
        });

        builder.Property(vc => vc.VacancyId)
               .HasColumnType("UUID")
               .HasColumnName("VacancyId")
               .IsRequired();

        builder.Property(vc => vc.CourseId)
               .HasColumnType("UUID")
               .IsRequired();

        builder.HasOne(vc => vc.Course)
               .WithMany(c => c.VacancyCourses)
               .HasForeignKey(vc => vc.CourseId)
               .IsRequired();

        builder.HasOne(vc => vc.Vacancy)
               .WithMany(v => v.Courses)
               .HasForeignKey(vc => vc.VacancyId)
               .IsRequired();
    }
}
