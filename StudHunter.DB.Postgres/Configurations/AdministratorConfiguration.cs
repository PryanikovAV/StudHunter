using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.DB.Postgres.Configurations;

public class AdministratorConfiguration : IEntityTypeConfiguration<Administrator>
{
    public void Configure(EntityTypeBuilder<Administrator> builder)
    {
        builder.Property(s => s.FirstName)
               .HasColumnType("VARCHAR(50)")
               .HasMaxLength(50)
               .IsRequired();

        builder.Property(s => s.LastName)
               .HasColumnType("VARCHAR(50)")
               .HasMaxLength(50)
               .IsRequired();

        builder.Property(s => s.Patronymic)
               .HasColumnType("VARCHAR(50)")
               .HasMaxLength(50)
               .IsRequired(false);
    }
}
