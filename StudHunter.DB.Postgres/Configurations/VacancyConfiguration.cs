using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudHunter.DB.Postgres.Models;
namespace StudHunter.DB.Postgres.Configurations;

public class VacancyConfiguration : IEntityTypeConfiguration<Vacancy>
{
    public void Configure(EntityTypeBuilder<Vacancy> builder)
    {
        builder.HasKey(vc => vc.Id);

        builder.Property(vc => vc.Id)
               .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(vc => vc.EmployerId)
               .HasColumnType("UUID")
               .IsRequired();

        builder.Property(vc => vc.Title)
               .HasColumnType("VARCHAR(255)")
               .HasMaxLength(255)
               .IsRequired();

        builder.Property(vc => vc.Description)
               .HasColumnType("TEXT")
               .HasMaxLength(2500)
               .IsRequired(false);

        builder.Property(vc => vc.Salary)
               .HasColumnType("DECIMAL(10, 2)")
               .HasPrecision(10, 2)
               .HasAnnotation("CheckConstraint", "Salary >= 0 AND Salary <= 1000000")
               .IsRequired(false);

        builder.Property(vc => vc.CreatedAt)
               .HasColumnType("TIMESTAMP")
               .HasDefaultValueSql("CURRENT_TIMESTAMP")
               .IsRequired();

        builder.Property(vc => vc.UpdatedAt)
               .HasColumnType("TIMESTAMP")
               .HasDefaultValueSql("CURRENT_TIMESTAMP")
               .IsRequired();

        builder.Property(vc => vc.Type)
               .HasColumnType("INTEGER")
               .IsRequired();

        builder.HasMany(vc => vc.Courses)
               .WithOne(vc => vc.Vacancy)
               .HasForeignKey(vc => vc.VacancyId)
               .IsRequired();

        builder.HasMany(v => v.Favorites)
               .WithOne(f => f.Vacancy)
               .HasForeignKey(f => f.VacancyId)
               .IsRequired(false);

        builder.HasMany(v => v.Invitations)
               .WithOne(i => i.Vacancy)
               .HasForeignKey(i => i.VacancyId)
               .IsRequired(false);

        builder.HasIndex(vc => vc.CreatedAt);
    }
}
