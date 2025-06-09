using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudHunter.DB.Postgres.Models;
namespace StudHunter.DB.Postgres.Configurations;

public class UserAchievementConfiguration : IEntityTypeConfiguration<UserAchievement>
{
    public void Configure(EntityTypeBuilder<UserAchievement> builder)
    {
        builder.HasKey(ua => new { ua.UserId, ua.AchievementTemplateId });

        builder.Property(ua => ua.UserId)
               .HasColumnType("UUID");

        builder.Property(ua => ua.AchievementTemplateId)
               .HasColumnType("INTEGER");

        builder.Property(ua => ua.AchievementAt)
               .HasColumnType("TIMESTAMP");

        builder.HasOne(ua => ua.User)
               .WithMany(u => u.Achievements)
               .HasForeignKey(ua => ua.UserId)
               .IsRequired();

        builder.HasOne(ua => ua.AchievementTemplate)
               .WithMany(a => a.UserAchievements)
               .HasForeignKey(ua => ua.AchievementTemplateId)
               .IsRequired();

        builder.HasIndex(ua => new { ua.UserId, ua.AchievementTemplateId })
               .IsUnique();
    }
}
