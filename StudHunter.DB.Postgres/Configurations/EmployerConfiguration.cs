using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudHunter.DB.Postgres.Models;
namespace StudHunter.DB.Postgres.Configurations;

public class EmployerConfiguration : IEntityTypeConfiguration<Employer>
{
    public void Configure(EntityTypeBuilder<Employer> builder)
    {
        builder.Property(e => e.AccreditationStatus)
               .HasColumnType("BOOLEAN")
               .HasDefaultValue(false)
               .IsRequired();

        builder.Property(e => e.Name)
               .HasColumnType("VARCHAR(255)")
               .HasMaxLength(255)
               .IsRequired();

        builder.Property(e => e.Description)
               .HasColumnType("TEXT")
               .HasMaxLength(1000)
               .IsRequired(false);

        builder.Property(e => e.Website)
               .HasColumnType("VARCHAR(255)")
               .HasMaxLength(255)
               .HasAnnotation("Url", true)
               .IsRequired(false);

        builder.Property(e => e.Specialization)
               .HasColumnType("VARCHAR(255)")
               .HasMaxLength(255)
               .IsRequired(false);

        builder.HasMany(e => e.Vacancies)
               .WithOne(v => v.Employer)
               .HasForeignKey(v => v.EmployerId)
               .OnDelete(DeleteBehavior.Cascade)
               .IsRequired();
    }
}
