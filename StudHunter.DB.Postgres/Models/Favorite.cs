namespace StudHunter.DB.Postgres.Models;

public class Favorite
{
    public Guid Id { get; init; }
    public Guid UserId { get; set; }

    public Guid? VacancyId { get; set; }
    public Guid? EmployerId { get; set; }
    public Guid? ResumeId { get; set; }

    public DateTime AddedAt { get; init; } = DateTime.UtcNow;

    public virtual User User { get; set; } = null!;
    public virtual Vacancy? Vacancy { get; set; }
    public virtual Employer? Employer { get; set; }
    public virtual Resume? Resume { get; set; }
}
