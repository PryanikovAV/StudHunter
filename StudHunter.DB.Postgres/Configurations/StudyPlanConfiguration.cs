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
               .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(sp => sp.StudentId)
               .HasColumnType("UUID")
               .IsRequired();

        builder.Property(s => s.CourseNumber)
               .HasColumnType("INTEGER")
               .HasDefaultValue(1)
               .IsRequired();

        builder.Property(sp => sp.FacultyId)
               .HasColumnType("UUID")
               .IsRequired();

        builder.Property(sp => sp.SpecialityId)
               .HasColumnType("UUID")
               .IsRequired();

        builder.Property(sp => sp.StudyForm)
               .HasColumnType("INTEGER")
               .IsRequired();

        builder.Property(sp => sp.BeginYear)
               .HasColumnType("DATE")
               .IsRequired();

        builder.HasOne(sp => sp.Student)
               .WithOne(s => s.StudyPlan)
               .HasForeignKey<StudyPlan>(sp => sp.StudentId)
               .IsRequired();

        builder.HasOne(sp => sp.Faculty)
               .WithMany(f => f.StudyPlans)
               .HasForeignKey(sp => sp.FacultyId)
               .IsRequired();

        builder.HasOne(sp => sp.Speciality)
               .WithMany(spec => spec.StudyPlans)
               .HasForeignKey(sp => sp.SpecialityId)
               .IsRequired();

        builder.HasMany(sp => sp.StudyPlanCourses)
               .WithOne(spc => spc.StudyPlan)
               .HasForeignKey(spc => spc.StudyPlanId)
               .IsRequired();

        builder.HasIndex(sp => sp.StudentId)
               .IsUnique();
    }
}
