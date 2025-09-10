using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudHunter.DB.Postgres.Models;
namespace StudHunter.DB.Postgres.Configurations;

public class FavoriteConfiguration : IEntityTypeConfiguration<Favorite>
{
    public void Configure(EntityTypeBuilder<Favorite> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Id)
               .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(f => f.UserId)
               .HasColumnType("UUID")
               .IsRequired();

        builder.Property(f => f.VacancyId)
               .HasColumnType("UUID")
               .IsRequired(false);

        builder.Property(f => f.EmployerId)
               .HasColumnType("UUID")
               .IsRequired(false);

        builder.Property(f => f.StudentId)
               .HasColumnType("UUID")
               .IsRequired(false);

        builder.Property(f => f.AddedAt)
               .HasColumnType("TIMESTAMPTZ")
               .HasDefaultValueSql("CURRENT_TIMESTAMP")
               .IsRequired();

        builder.HasOne(f => f.User)
               .WithMany(u => u.Favorites)
               .HasForeignKey(f => f.UserId)
               .OnDelete(DeleteBehavior.Cascade)
               .IsRequired();

        builder.HasOne(f => f.Vacancy)
               .WithMany()
               .HasForeignKey(f => f.VacancyId)
               .OnDelete(DeleteBehavior.Cascade)
               .IsRequired(false);

        builder.HasOne(f => f.Employer)
               .WithMany()
               .HasForeignKey(f => f.EmployerId)
               .OnDelete(DeleteBehavior.Cascade)
               .IsRequired(false);

        builder.HasOne(f => f.Student)
               .WithMany()
               .HasForeignKey(f => f.StudentId)
               .OnDelete(DeleteBehavior.Cascade)
               .IsRequired(false);

        builder.HasIndex(f => f.UserId);

        builder.HasIndex(f => f.VacancyId)
               .HasFilter("\"EmployerId\" IS NOT NULL");

        builder.HasIndex(f => f.EmployerId)
               .HasFilter("\"EmployerId\" IS NOT NULL");

        builder.HasIndex(f => f.StudentId)
               .HasFilter("\"StudentId\" IS NOT NULL");

        builder.HasIndex(f => new { f.UserId, f.VacancyId })
               .IsUnique()
               .HasFilter("\"VacancyId\" IS NOT NULL");

        builder.HasIndex(f => new { f.UserId, f.EmployerId })
               .IsUnique()
               .HasFilter("\"EmployerId\" IS NOT NULL");

        builder.HasIndex(f => new { f.UserId, f.StudentId })
               .IsUnique()
               .HasFilter("\"StudentId\" IS NOT NULL");
    }
}
