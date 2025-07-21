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
               .HasColumnName("VacancyId")
               .IsRequired(false);

        builder.Property(f => f.ResumeId)
               .HasColumnType("UUID")
               .HasColumnName("ResumeId")
               .IsRequired(false);

        builder.Property(f => f.AddedAt)
               .HasColumnType("TIMESTAMP")
               .HasDefaultValueSql("CURRENT_TIMESTAMP")
               .IsRequired();

        builder.HasOne(f => f.User)
               .WithMany(u => u.Favorites)
               .HasForeignKey(f => f.UserId)
               .IsRequired();

        builder.HasOne(f => f.Vacancy)
               .WithMany(v => v.Favorites)
               .HasForeignKey(f => f.VacancyId)
               .IsRequired(false);

        builder.HasOne(f => f.Resume)
               .WithMany(r => r.Favorites)
               .HasForeignKey(f => f.ResumeId)
               .IsRequired(false);

        builder.HasIndex(f => f.ResumeId);

        builder.HasIndex(v => v.VacancyId);

        builder.HasIndex(f => new { f.UserId, f.ResumeId })
               .IsUnique()
               .HasFilter("\"ResumeId\" IS NOT NULL");

        builder.HasIndex(f => new { f.UserId, f.VacancyId })
               .IsUnique()
               .HasFilter("\"VacancyId\" IS NOT NULL");
    }
}
