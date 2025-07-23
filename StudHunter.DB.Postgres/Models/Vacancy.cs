using StudHunter.DB.Postgres.Interfaces;
using System.ComponentModel.DataAnnotations;
namespace StudHunter.DB.Postgres.Models;

public class Vacancy : ISoftDeletable
{
    public Guid Id { get; set; }

    public Guid EmployerId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public decimal? Salary { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public VacancyType Type { get; set; }

    public bool IsDeleted { get; set; }

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
