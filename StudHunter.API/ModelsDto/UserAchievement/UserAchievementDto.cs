using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.UserAchievement;

public class UserAchievementDto
{
    public Guid UserId { get; set; }

    public int AchievementTemplateId { get; set; }

    public DateTime AchievementAt { get; set; }

    public string? AchievementName { get; set; } = string.Empty;  // <-- from AchievementTemplate

    public string? AchievementDescription { get; set; } = string.Empty;  // <-- from AchievementTemplate 
}
