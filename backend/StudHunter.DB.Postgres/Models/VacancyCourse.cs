namespace StudHunter.DB.Postgres.Models;

public class VacancyCourse
{
    public Guid VacancyId { get; init; }
    public Guid CourseId { get; init; }

    public virtual Vacancy Vacancy { get; set; } = null!;
    public virtual Course Course { get; set; } = null!;
}
