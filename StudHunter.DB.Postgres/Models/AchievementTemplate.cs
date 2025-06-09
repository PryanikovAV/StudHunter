using System.ComponentModel.DataAnnotations;
namespace StudHunter.DB.Postgres.Models;

public class AchievementTemplate
{
    public int Id { get; set; }

    [Required]
    [StringLength(255, MinimumLength = 1)]
    public string Name { get; set; } = null!;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    public User.UserRole Target { get; set; }

    public virtual ICollection<UserAchievement> UserAchievements { get; set; } = new List<UserAchievement>();
}
