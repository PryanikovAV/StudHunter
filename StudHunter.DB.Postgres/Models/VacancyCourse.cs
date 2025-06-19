namespace StudHunter.DB.Postgres.Models;

public class VacancyCourse
{
    public Guid VacancyId { get; set; }

    public Guid CourseId { get; set; }

    public virtual Vacancy Vacancy { get; set; } = null!;
    public virtual Course Course { get; set; } = null!;
}
