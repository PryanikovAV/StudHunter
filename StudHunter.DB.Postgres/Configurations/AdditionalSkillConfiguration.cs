using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.DB.Postgres.Configurations;

public  class AdditionalSkillConfiguration : IEntityTypeConfiguration<AdditionalSkill>
{
    public void Configure(EntityTypeBuilder<AdditionalSkill> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
               .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(s => s.Name)
               .HasColumnType("VARCHAR(255)")
               .HasMaxLength(255)
               .IsRequired();

        builder.HasIndex(s => s.Name)
               .IsUnique();
    }
}
