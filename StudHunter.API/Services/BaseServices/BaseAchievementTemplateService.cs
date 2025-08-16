using StudHunter.API.ModelsDto.AchievementTemplate;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.BaseServices;

public abstract class BaseAchievementTemplateService(StudHunterDbContext context) : BaseService(context)
{
    protected static AchievementTemplateDto MapToAchievementTemplateDto(AchievementTemplate achievementTemplate)
    {
        return new AchievementTemplateDto
        {
            Id = achievementTemplate.Id,
            Name = achievementTemplate.Name,
            Description = achievementTemplate.Description,
            Target = achievementTemplate.Target.ToString(),
            IconUrl = achievementTemplate.IconUrl
        };
    }
}
