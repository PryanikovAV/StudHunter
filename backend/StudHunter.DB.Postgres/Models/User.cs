namespace StudHunter.DB.Postgres.Models;

public abstract class User
{
    public enum AccountStatus
    {
        Anonymous = 1,
        ProfileFilled = 2,
        FullyActivated = 3
    }

    public Guid Id { get; init; }

    public AccountStatus RegistrationStage { get; set; } = AccountStatus.Anonymous;

    private string _email = null!;
    public string Email
    {
        get => _email;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            _email = value.Trim().ToLower();
        }
    }

    private string _passwordHash = null!;
    public string PasswordHash
    {
        get => _passwordHash;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            _passwordHash = value;
        }
    }

    public Guid? CityId { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public string? AvatarUrl { get; set; }

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }

    public virtual City? City { get; set; }
    public virtual ICollection<BlackList> BlackLists { get; set; } = new HashSet<BlackList>();
    public virtual ICollection<Chat> ChatsAsUser1 { get; set; } = new HashSet<Chat>();
    public virtual ICollection<Chat> ChatsAsUser2 { get; set; } = new HashSet<Chat>();
    public virtual ICollection<Favorite> Favorites { get; set; } = new HashSet<Favorite>();
    public virtual ICollection<Invitation> ReceivedInvitations { get; set; } = new HashSet<Invitation>();
    public virtual ICollection<Invitation> SentInvitations { get; set; } = new HashSet<Invitation>();
    protected User() { }
}