namespace StudHunter.DB.Postgres.Models;

public abstract class User
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;

    public string? ContactEmail { get; set; }

    public string? ContactPhone { get; set; }

    public string PasswordHash { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<UserAchievement> Achievements { get; set; } = new List<UserAchievement>();
    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    public virtual ICollection<Invitation> SentInvitations { get; set; } = new List<Invitation>();
    public virtual ICollection<Invitation> ReceivedInvitations { get; set; } = new List<Invitation>();
    public virtual ICollection<Message> SentMessages { get; set; } = new List<Message>();
    public virtual ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();
}
