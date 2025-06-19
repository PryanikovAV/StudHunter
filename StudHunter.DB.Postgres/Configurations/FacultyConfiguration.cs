using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudHunter.DB.Postgres.Models;
namespace StudHunter.DB.Postgres.Configurations;

public class FacultyConfiguration: IEntityTypeConfiguration<Faculty>
{
    public void Configure(EntityTypeBuilder<Faculty> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Id)
               .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(f => f.Name)
               .HasColumnType("VARCHAR(255)")
               .HasMaxLength(255)
               .IsRequired();

        builder.Property(f => f.Description)
               .HasColumnType("TEXT")
               .HasMaxLength(1000)
               .IsRequired(false);

        builder.HasIndex(f => f.Name)
               .IsUnique();
    }
}
