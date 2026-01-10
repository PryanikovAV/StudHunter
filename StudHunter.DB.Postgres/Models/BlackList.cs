namespace StudHunter.DB.Postgres.Models;

public class BlackList
{
    public Guid Id { get; init; }
    public Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;
    public Guid BlockedUserId { get; set; }
    public virtual User BlockedUser { get; set; } = null!;
    public DateTime BlockedAt { get; init; } = DateTime.UtcNow;
}
