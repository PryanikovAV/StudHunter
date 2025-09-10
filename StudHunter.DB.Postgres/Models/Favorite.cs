namespace StudHunter.DB.Postgres.Models;

public class Favorite
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid? VacancyId { get; set; }

    public Guid? EmployerId { get; set; }

    public Guid? StudentId { get; set; }

    public DateTime AddedAt { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual Vacancy? Vacancy { get; set; }
    public virtual Employer? Employer { get; set; }
    public virtual Student? Student { get; set; }
}
