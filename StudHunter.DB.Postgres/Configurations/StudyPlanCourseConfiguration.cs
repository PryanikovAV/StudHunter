using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.DB.Postgres.Configurations;

public class StudyPlanCourseConfiguration : IEntityTypeConfiguration<StudyPlanCourse>
{
    public void Configure(EntityTypeBuilder<StudyPlanCourse> builder)
    {
        builder.HasKey(spc => new { spc.StudyPlanId, spc.CourseId });

        builder.Property(spc => spc.StudyPlanId)
               .HasColumnType("UUID")
               .IsRequired();

        builder.Property(spc => spc.CourseId)
               .HasColumnType("UUID")
               .IsRequired();

        builder.HasOne(spc => spc.StudyPlan)
               .WithMany(sp => sp.StudyPlanCourses)
               .HasForeignKey(spc => spc.StudyPlanId)
               .IsRequired();

        builder.HasOne(spc => spc.Course)
               .WithMany(c => c.StudyPlanCourses)
               .HasForeignKey(spc => spc.CourseId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired();
    }
}
