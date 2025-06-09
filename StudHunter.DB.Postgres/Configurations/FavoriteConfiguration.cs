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
               .HasColumnType("UUID");

        builder.Property(f => f.TargetId)
               .HasColumnType("UUID");

        builder.Property(f => f.Target)
               .HasColumnType("INTEGER");

        builder.Property(f => f.AddedAt)
               .HasColumnType("TIMESTAMP")
               .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasOne(f => f.User)
               .WithMany(u => u.Favorites)
               .HasForeignKey(f => f.UserId)
               .IsRequired();

        builder.HasOne(f => f.Vacancy)
               .WithMany(v => v.Favorites)
               .HasForeignKey(f => f.TargetId)
               .IsRequired(false);

        builder.HasOne(f => f.Resume)
               .WithMany(r => r.Favorites)
               .HasForeignKey(f => f.TargetId)
               .IsRequired(false);

        builder.HasIndex(f => new { f.UserId, f.TargetId, f.Target })
               .IsUnique();
    }
}
