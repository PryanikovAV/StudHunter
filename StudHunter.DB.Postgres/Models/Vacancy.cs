using System.ComponentModel.DataAnnotations;
namespace StudHunter.DB.Postgres.Models;

public class Vacancy
{
    public Guid Id { get; set; }

    [Required]
    public Guid EmployerId { get; set; }

    [Required]
    [StringLength(255, MinimumLength = 1)]
    public string? Title { get; set; } = null!;

    [StringLength(2500)]
    public string? Description { get; set; }

    [Range(0, 1000000)]
    public decimal? Salary { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    public DateTime UpdatedAt { get; set; }

    [Required]
    public VacancyType Type { get; set; }

    public virtual Employer Employer { get; set; } = null!;
    public virtual ICollection<VacancyCourse> Courses { get; set; } = new List<VacancyCourse>();
    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    public virtual ICollection<Invitation> Invitations { get; set; } = new List<Invitation>();

    public enum VacancyType
    {
        [Display(Name = "Стажировка")]
        Internship,
        [Display(Name = "Работа")]
        Job
    }
}
