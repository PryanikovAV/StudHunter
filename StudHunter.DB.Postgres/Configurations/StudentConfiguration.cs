using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudHunter.DB.Postgres.Models;
namespace StudHunter.DB.Postgres.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.Property(s => s.FirstName)
               .HasColumnType("VARCHAR(50)");

        builder.Property(s => s.LastName)
               .HasColumnType("VARCHAR(50)");

        builder.Property(s => s.BirthDate)
               .HasColumnType("DATE");

        builder.Property(s => s.Photo)
               .HasColumnType("VARCHAR(255)");

        builder.Property(s => s.ContactPhone)
               .HasColumnType("VARCHAR(20)");

        builder.Property(s => s.Gender)
               .HasColumnType("INTEGER");

        builder.Property(s => s.IsForeign)
               .HasColumnType("BOOLEAN");

        builder.Property(s => s.CourseNumber)
               .HasColumnType("INTEGER");

        builder.HasOne(s => s.Status)
               .WithMany()
               .HasForeignKey(s => s.StatusId)
               .IsRequired();

        builder.HasOne(s => s.StudyPlan)
               .WithOne(sp => sp.Student)
               .HasForeignKey<StudyPlan>(sp => sp.StudentId)
               .IsRequired();

        builder.HasMany(s => s.Messages)
               .WithOne(m => m.Student)
               .HasForeignKey(m => m.StudentId)
               .IsRequired();

        builder.HasOne(s => s.Resume)
               .WithOne(r => r.Student)
               .IsRequired();
    }
}
