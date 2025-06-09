using System.ComponentModel.DataAnnotations;
namespace StudHunter.DB.Postgres.Models;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public UserRole Role { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(255, MinimumLength = 1)]
    public string Email { get; set; } = null!;

    [Required]
    [StringLength(255)]
    public string PasswordHash { get; set; } = null!;

    [Required]
    public DateTime CreatedAt { get; set; }

    public virtual ICollection<UserAchievement> Achievements { get; set; } = new List<UserAchievement>();
    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    public virtual ICollection<Invitation> SentInvitations { get; set; } = new List<Invitation>();
    public virtual ICollection<Invitation> ReceivedInvitations { get; set; } = new List<Invitation>();

    public enum UserRole
    {
        [Display(Name = "Администратор")]
        Admin,
        [Display(Name = "Работодатель")]
        Employer,
        [Display(Name = "Студент")]
        Student
    }
}
