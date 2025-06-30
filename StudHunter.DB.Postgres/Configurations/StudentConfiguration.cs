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
               .IsRequired();

        builder.Property(s => s.BirthDate)
               .HasColumnType("DATE")
               .IsRequired();

        builder.Property(s => s.Photo)
               .HasColumnType("VARCHAR(255)")
               .HasMaxLength(255)
               .IsRequired(false);

        builder.Property(s => s.IsForeign)
               .HasColumnType("BOOLEAN")
               .HasDefaultValue(false)
               .IsRequired();

        builder.HasOne(s => s.Status)
               .WithMany()
               .HasForeignKey(s => s.StatusId)
               .IsRequired(false);

        builder.HasOne(s => s.Resume)
               .WithOne(r => r.Student)
               .IsRequired(false);

        builder.HasOne(s => s.StudyPlan)
               .WithOne(sp => sp.Student)
               .HasForeignKey<StudyPlan>(sp => sp.StudentId)
               .IsRequired();
    }
}
