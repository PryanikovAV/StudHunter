using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.DB.Postgres.Configurations;

public class StudyPlanConfiguration : IEntityTypeConfiguration<StudyPlan>
{
    public void Configure(EntityTypeBuilder<StudyPlan> builder)
    {
        builder.HasKey(sp => sp.Id);

        builder.Property(sp => sp.Id)
               .HasColumnType("UUID")
               .HasDefaultValueSql("gen_random_uuid()")
               .IsRequired();

        builder.Property(sp => sp.StudentId)
               .HasColumnType("UUID")
               .IsRequired();

        builder.Property(sp => sp.CourseNumber)
               .HasColumnType("INTEGER")
               .HasDefaultValue(1)
               .IsRequired();

        builder.Property(sp => sp.FacultyId)
               .HasColumnType("UUID")
               .HasDefaultValue(Guid.Empty)
               .IsRequired();

        builder.Property(sp => sp.SpecialityId)
               .HasColumnType("UUID")
               .HasDefaultValue(Guid.Empty)
               .IsRequired();

        builder.Property(sp => sp.StudyForm)
               .HasColumnType("INTEGER")
               .HasDefaultValue(StudyPlan.StudyForms.fulltime)
               .IsRequired();

        builder.Property(sp => sp.BeginYear)
               .HasColumnType("DATE")
               .HasDefaultValue(DateOnly.FromDateTime(DateTime.UtcNow))
               .IsRequired();

        builder.HasOne(sp => sp.Student)
               .WithOne(s => s.StudyPlan)
               .HasForeignKey<StudyPlan>(sp => sp.StudentId)
               .OnDelete(DeleteBehavior.Cascade)
               .IsRequired();

        builder.HasOne(sp => sp.Faculty)
               .WithMany(f => f.StudyPlans)
               .HasForeignKey(sp => sp.FacultyId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired();

        builder.HasOne(sp => sp.Speciality)
               .WithMany(spec => spec.StudyPlans)
               .HasForeignKey(sp => sp.SpecialityId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired();

        builder.HasMany(sp => sp.StudyPlanCourses)
               .WithOne(spc => spc.StudyPlan)
               .HasForeignKey(spc => spc.StudyPlanId)
               .OnDelete(DeleteBehavior.Cascade)
               .IsRequired(false);

        builder.HasIndex(sp => sp.StudentId)
               .IsUnique();
    }
}
