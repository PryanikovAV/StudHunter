using System.ComponentModel.DataAnnotations;
namespace StudHunter.DB.Postgres.Models;

public class Message
{
    public Guid Id { get; set; }

    [Required]
    public Guid EmployerId { get; set; }

    [Required]
    public Guid StudentId { get; set; }

    [Required]
    public Guid SenderId { get; set; }

    [Required]
    [StringLength(1000, MinimumLength = 0)]
    public string? Context { get; set; } = "";

    [Required]
    public DateTime SentAt { get; set; }

    public virtual Employer Employer { get; set; } = null!;
    public virtual Student Student { get; set; } = null!;
}
