using System.ComponentModel.DataAnnotations;
namespace StudHunter.DB.Postgres.Models;

public class Favorite
{
    public Guid Id { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid TargetId { get; set; }

    [Required]
    public TargetType Target { get; set; }

    [Required]
    public DateTime AddedAt { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual Vacancy? Vacancy { get; set; }
    public virtual Resume? Resume { get; set; }

    public enum TargetType
    {
        Vacancy,
        Resume
    }
}
