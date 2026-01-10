using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.DB.Postgres.Configurations;

public class VacancyAdditionalSkillConfiguration : IEntityTypeConfiguration<VacancyAdditionalSkill>
{
    public void Configure(EntityTypeBuilder<VacancyAdditionalSkill> builder)
    {
        builder.HasKey(vas => new { vas.VacancyId, vas.AdditionalSkillId });

        builder.HasOne(vas => vas.Vacancy)
               .WithMany(v => v.AdditionalSkills)
               .HasForeignKey(vas => vas.VacancyId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(vas => vas.AdditionalSkill)
               .WithMany(askill => askill.VacancyAdditionalSkills)
               .HasForeignKey(vas => vas.AdditionalSkillId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}