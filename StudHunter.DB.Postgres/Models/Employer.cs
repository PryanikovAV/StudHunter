using System.ComponentModel.DataAnnotations;
namespace StudHunter.DB.Postgres.Models;

public class Employer : User
{
    public bool AccreditationStatus { get; set; } = false;

    [Required]
    [StringLength(255, MinimumLength = 1)]
    public string Name { get; set; } = null!;

    [StringLength(1000)]
    public string? Description { get; set; }

    [StringLength(255)]
    [Url]
    public string? Website { get; set; }

    [StringLength(20)]
    [Phone]
    public string? ContactPhone { get; set; }

    [StringLength(100)]
    [EmailAddress]
    public string? ContactEmail { get; set; }

    [StringLength(255)]
    public string? Specialization { get; set; }

    public virtual ICollection<Vacancy> Vacancies { get; set; } = new List<Vacancy>();
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
