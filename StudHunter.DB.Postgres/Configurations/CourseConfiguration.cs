using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudHunter.DB.Postgres.Models;
namespace StudHunter.DB.Postgres.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
               .HasDefaultValueSql("gen_random_uuid()")
               .IsRequired();

        builder.Property(c => c.Name)
               .HasColumnType("VARCHAR(255)")
               .HasMaxLength(255)
               .IsRequired();

        builder.Property(c => c.Description)
               .HasColumnType("TEXT")
               .HasMaxLength(1000)
               .IsRequired(false);

        builder.HasIndex(c => c.Name)
               .IsUnique();

        builder.HasMany(c => c.VacancyCourses)
               .WithOne(vc => vc.Course)
               .HasForeignKey(vc => vc.CourseId)
               .IsRequired(false);

        builder.HasMany(c => c.StudyPlanCourses)
               .WithOne(spc => spc.Course)
               .HasForeignKey(spc => spc.CourseId)
               .IsRequired(false);
    }
}
