using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudHunter.DB.Postgres.Models;
namespace StudHunter.DB.Postgres.Configurations;

public class SpecialityConfiguration : IEntityTypeConfiguration<Speciality>
{
    public void Configure(EntityTypeBuilder<Speciality> builder)
    {
        builder.HasKey(s  => s.Id);

        builder.Property(s => s.Id)
               .HasDefaultValueSql("gen_random_uuid()");
        
        builder.Property(s => s.Name)
               .HasColumnType("VARCHAR(255)")
               .HasMaxLength(255)
               .IsRequired();

        builder.Property(s => s.Description)
               .HasColumnType("TEXT")
               .HasMaxLength(1000)
               .IsRequired(false);

        builder.HasIndex(s => s.Name)
               .IsUnique();
    }
}
