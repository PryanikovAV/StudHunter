using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.UserAchievement;

/// <summary>
/// Data transfer object for granting an achievement to a user.
/// </summary>
public class GrantAchievementDto
{
    /// <summary>
    /// The unique identifier (GUID) of the user.
    /// </summary>
    [Required(ErrorMessage = "{0} is required")]
    public Guid UserId { get; set; }

    /// <summary>
    /// The order number of the achievement template.
    /// </summary>
    [Required(ErrorMessage = "{0} is required")]
    public int AchievementTemplateOrderNumber { get; set; }
}
