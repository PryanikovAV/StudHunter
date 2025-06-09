using System.ComponentModel.DataAnnotations;
namespace StudHunter.DB.Postgres.Models;

public class VacancyCourse
{
    [Required]
    public Guid VacancyId { get; set; }

    [Required]
    public Guid CourseId { get; set; }

    public virtual Vacancy Vacancy { get; set; } = null!;
    public virtual Course Course { get; set; } = null!;
}
