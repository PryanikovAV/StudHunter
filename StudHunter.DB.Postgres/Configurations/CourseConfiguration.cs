using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudHunter.DB.Postgres.Models;
namespace StudHunter.DB.Postgres.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.HasKey(cc =>  cc.Id);

        builder.Property(cc => cc.Id)
               .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(cc => cc.Name)
               .HasColumnType("VARCHAR(255)");

        builder.Property(cc => cc.Description)
               .HasColumnType("TEXT");

        builder.HasIndex(cc => cc.Name)
               .IsUnique();

        builder.HasMany(cc => cc.VacancyCourses)
               .WithOne(vc => vc.Course)
               .HasForeignKey(vc => vc.CourseId)
               .IsRequired();

        builder.HasMany(cc => cc.StudyPlanCourses)
               .WithOne(spc => spc.Course)
               .HasForeignKey(spc => spc.CourseId)
               .IsRequired();
    }
}
