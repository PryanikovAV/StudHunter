namespace StudHunter.API.ModelsDto.UserAchievement;

public class UserAchievementDto
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public int AchievementTemplateOrderNumber { get; set; }

    public DateTime AchievementAt { get; set; }

    public string? AchievementName { get; set; } = string.Empty;  // <-- from AchievementTemplate

    public string? AchievementDescription { get; set; } = string.Empty;  // <-- from AchievementTemplate

    public string? IconUrl { get; set; } = string.Empty;
}
