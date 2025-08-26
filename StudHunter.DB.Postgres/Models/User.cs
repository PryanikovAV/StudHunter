using StudHunter.DB.Postgres.Interfaces;

namespace StudHunter.DB.Postgres.Models;

public abstract class User : ISoftDeletable
{
    public Guid Id { get; set; }

    public string Email { get; private set; } = null!;

    public string? ContactEmail { get; set; }

    public string? ContactPhone { get; set; }

    public string PasswordHash { get; private set; } = null!;

    public DateTime CreatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<UserAchievement> Achievements { get; set; } = new List<UserAchievement>();
    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    public virtual ICollection<Invitation> SentInvitations { get; set; } = new List<Invitation>();
    public virtual ICollection<Invitation> ReceivedInvitations { get; set; } = new List<Invitation>();
    public virtual ICollection<Message> SentMessages { get; set; } = new List<Message>();
    public virtual ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();
    public virtual ICollection<Chat> ChatsAsUser1 { get; set; } = new List<Chat>();
    public virtual ICollection<Chat> ChatsAsUser2 { get; set; } = new List<Chat>();

    protected User() { }

    protected void SetEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentNullException("Email cannot be empty.");

        Email = email;
    }

    protected void SetPasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentNullException("PasswordHash cannot be empty.");

        PasswordHash = passwordHash;
    }
}
