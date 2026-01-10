namespace StudHunter.DB.Postgres.Models;

public class Resume
{
    public Guid Id { get; init; }
    public Guid StudentId { get; set; }

    private string _title = null!;
    public string Title
    {
        get => _title;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            _title = value.Trim();
        }
    }

    public string? Description { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public virtual Student Student { get; set; } = null!;

    public virtual ICollection<ResumeAdditionalSkill> AdditionalSkills { get; set; } = new HashSet<ResumeAdditionalSkill>();
    public virtual ICollection<Invitation> Invitations { get; set; } = new HashSet<Invitation>();
}
