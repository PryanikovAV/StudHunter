using System.ComponentModel.DataAnnotations;
namespace StudHunter.DB.Postgres.Models;

public class UserAchievement
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public int AchievementTemplateId { get; set; }

    [Required]
    public DateTime AchievementAt { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual AchievementTemplate AchievementTemplate { get; set; } = null!;
}
