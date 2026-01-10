using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.DB.Postgres.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.Property(s => s.FirstName)
               .HasColumnType("VARCHAR(50)")
               .HasMaxLength(50)
               .IsRequired();

        builder.Property(s => s.LastName)
               .HasColumnType("VARCHAR(50)")
               .HasMaxLength(50)
               .IsRequired();

        builder.Property(s => s.Gender)
               .HasColumnType("INTEGER")
               .IsRequired(false);

        builder.Property(s => s.Status)
               .HasColumnType("INTEGER")
               .HasDefaultValue(Student.StudentStatus.Studying)
               .IsRequired();

        builder.Property(s => s.BirthDate)
               .HasColumnType("DATE")
               .IsRequired(false);

        builder.Property(s => s.IsForeign)
               .HasColumnType("BOOLEAN")
               .IsRequired(false);

        builder.HasOne(s => s.Resume)
               .WithOne(r => r.Student)
               .HasForeignKey<Resume>(r => r.StudentId)
               .OnDelete(DeleteBehavior.Cascade)
               .IsRequired(false);

        builder.HasOne(s => s.StudyPlan)
               .WithOne(sp => sp.Student)
               .HasForeignKey<StudyPlan>(sp => sp.StudentId)
               .OnDelete(DeleteBehavior.Cascade)
               .IsRequired(false);

        builder.HasIndex(s => s.Email)
               .IsUnique();
    }
}
