namespace StudHunter.DB.Postgres.Models;

public class UserAchievement
{
    public Guid UserId { get; set; }

    public int AchievementTemplateId { get; set; }

    public DateTime AchievementAt { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual AchievementTemplate AchievementTemplate { get; set; } = null!;
}
