using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.DB.Postgres.Configurations;

public class VacancyConfiguration : IEntityTypeConfiguration<Vacancy>
{
    public void Configure(EntityTypeBuilder<Vacancy> builder)
    {
        builder.HasKey(v => v.Id);

        builder.Property(v => v.Id)
               .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(v => v.EmployerId)
               .HasColumnType("UUID")
               .IsRequired();

        builder.Property(v => v.Title)
               .HasColumnType("VARCHAR(255)")
               .HasMaxLength(255)
               .IsRequired();

        builder.Property(v => v.Description)
               .HasColumnType("TEXT")
               .HasMaxLength(2500);

        builder.Property(v => v.Salary)
               .HasColumnType("DECIMAL(10, 2)")
               .HasPrecision(10, 2);

        builder.ToTable(t => t.HasCheckConstraint("CK_Vacancy_Salary",
                        "\"Salary\" >= 0 AND \"Salary\" <= 1000000"));

        builder.Property(v => v.CreatedAt)
               .HasColumnType("TIMESTAMPTZ")
               .HasDefaultValueSql("CURRENT_TIMESTAMP")
               .IsRequired();

        builder.Property(v => v.UpdatedAt)
               .HasColumnType("TIMESTAMPTZ")
               .HasDefaultValueSql("CURRENT_TIMESTAMP")
               .IsRequired();

        builder.Property(v => v.IsDeleted)
               .HasColumnType("BOOLEAN")
               .HasDefaultValue(false);

        builder.Property(v => v.DeletedAt)
               .HasColumnType("TIMESTAMPTZ");

        builder.Property(v => v.Type)
               .HasColumnType("INTEGER")
               .HasDefaultValue(Vacancy.VacancyType.Internship);

        builder.HasOne(v => v.Employer)
               .WithMany(e => e.Vacancies)
               .HasForeignKey(v => v.EmployerId)
               .OnDelete(DeleteBehavior.Cascade)
               .IsRequired();

        builder.HasMany(v => v.Courses)
               .WithOne(vc => vc.Vacancy)
               .HasForeignKey(vc => vc.VacancyId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(v => v.Invitations)
               .WithOne(i => i.Vacancy)
               .HasForeignKey(i => i.VacancyId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(v => !v.IsDeleted);
    }
}
