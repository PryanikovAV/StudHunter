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
               .HasDefaultValue(false);

        builder.Property(e => e.Name)
               .HasColumnType("VARCHAR(255)")
               .IsRequired();

        builder.Property(e => e.Description)
               .HasColumnType("TEXT");

        builder.Property(e => e.Website)
               .HasColumnType("VARCHAR(255)");

        builder.Property(e => e.ContactPhone)
               .HasColumnType("VARCHAR(20)");

        builder.Property(e => e.ContactEmail)
               .HasColumnType("VARCHAR(100)");

        builder.Property(e => e.Specialization)
               .HasColumnType("VARCHAR(255)");

        builder.HasMany(e => e.Vacancies)
               .WithOne(v => v.Employer)
               .HasForeignKey(v => v.EmployerId)
               .IsRequired();

        builder.HasMany(e => e.Messages)
               .WithOne(m => m.Employer)
               .HasForeignKey(m => m.EmployerId)
               .IsRequired();
    }
}
