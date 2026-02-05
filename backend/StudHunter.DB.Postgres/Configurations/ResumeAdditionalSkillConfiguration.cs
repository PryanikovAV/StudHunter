using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.DB.Postgres.Configurations;

public class ResumeAdditionalSkillConfiguration : IEntityTypeConfiguration<ResumeAdditionalSkill>
{
    public void Configure(EntityTypeBuilder<ResumeAdditionalSkill> builder)
    {
        builder.HasKey(ras => new { ras.ResumeId, ras.AdditionalSkillId });

        builder.HasOne(ras => ras.Resume)
               .WithMany(r => r.AdditionalSkills)
               .HasForeignKey(ras => ras.ResumeId);

        builder.HasOne(ras => ras.AdditionalSkill)
               .WithMany(s => s.ResumeAdditionalSkills)
               .HasForeignKey(ras => ras.AdditionalSkillId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}