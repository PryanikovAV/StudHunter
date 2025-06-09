using System.ComponentModel.DataAnnotations;
namespace StudHunter.DB.Postgres.Models;

public class Resume
{
    public Guid Id { get; set; }

    [Required]
    public Guid StudentId { get; set; }    

    [Required]
    [StringLength(255, MinimumLength = 1)]
    public string Title { get; set; } = null!;

    [StringLength(2500)]
    public string? Description { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    public DateTime UpdatedAt { get; set; }

    public virtual Student Student { get; set; } = null!;
    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    public virtual ICollection<Invitation> Invitations { get; set; } = new List<Invitation>();
}
