using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.DB.Postgres.Configurations;

public class AchievementTemplateConfiguration : IEntityTypeConfiguration<AchievementTemplate>
{
    public void Configure(EntityTypeBuilder<AchievementTemplate> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
               .HasColumnType("UUID")
               .HasDefaultValueSql("gen_random_uuid()")
               .IsRequired();

        builder.Property(a => a.OrderNumber)
               .HasColumnType("INTEGER")
               .IsRequired();

        builder.Property(a => a.Name)
               .HasColumnType("VARCHAR(255)")
               .HasMaxLength(255)
               .IsRequired();

        builder.Property(a => a.Description)
               .HasColumnType("TEXT")
               .HasMaxLength(1000)
               .IsRequired(false);

        builder.Property(a => a.IconUrl)
               .HasColumnType("VARCHAR(500)")
               .HasMaxLength(500)
               .IsRequired(false);

        builder.Property(a => a.Target)
               .HasColumnType("INTEGER")
               .IsRequired();

        builder.HasIndex(a => a.OrderNumber)
               .IsUnique();
    }
}
