using StudHunter.DB.Postgres.Interfaces;

namespace StudHunter.DB.Postgres.Models;

public class Resume : ISoftDeletable
{
    public Guid Id { get; set; }

    public Guid StudentId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Student Student { get; set; } = null!;
    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    public virtual ICollection<Invitation> Invitations { get; set; } = new List<Invitation>();
}
