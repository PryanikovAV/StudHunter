namespace StudHunter.API.ModelsDto.UserAchievementDto;

/// <summary>
/// Data transfer object for a user achievement.
/// </summary>
public class UserAchievementDto
{
    /// <summary>
    /// The unique identifier (GUID) of the user achievement.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The unique identifier (GUID) of the user.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Guid AchievementTemplateId { get; set; }

    /// <summary>
    /// The date and time the achievement was granted.
    /// </summary>
    public DateTime AchievementAt { get; set; }

    /// <summary>
    /// The name of the achievement (from AchievementTemplate).
    /// </summary>
    public string? AchievementName { get; set; } = string.Empty;

    /// <summary>
    /// The description of the achievement (from AchievementTemplate).
    /// </summary>
    public string? AchievementDescription { get; set; } = string.Empty;

    /// <summary>
    /// The URL of the achievement icon.
    /// </summary>
    public string? IconUrl { get; set; } = string.Empty;
}
