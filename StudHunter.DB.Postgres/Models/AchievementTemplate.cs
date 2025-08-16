using StudHunter.DB.Postgres.Interfaces;

namespace StudHunter.DB.Postgres.Models;

public class AchievementTemplate : IEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? IconUrl { get; set; }

    public AchievementTarget Target { get; set; }

    public virtual ICollection<UserAchievement> UserAchievements { get; set; } = new List<UserAchievement>();

    public enum AchievementTarget
    {
        Student,
        Employer
    }
}
