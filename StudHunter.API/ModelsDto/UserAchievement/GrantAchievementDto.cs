using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.UserAchievement;

public class GrantAchievementDto
{
    [Required(ErrorMessage = "{0} is required")]
    public Guid UserId { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    public int AchievementTemplateOrderNumber { get; set; }
}
