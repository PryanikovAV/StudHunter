namespace StudHunter.DB.Postgres.Models;

public class Vacancy
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

    public DateTime? DeletedAt { get; set; }

    public virtual Employer Employer { get; set; } = null!;
    public virtual ICollection<VacancyCourse> Courses { get; set; } = new List<VacancyCourse>();
    public virtual ICollection<Invitation> Invitations { get; set; } = new List<Invitation>();
    public virtual ICollection<AdditionalSkill> AdditionalSkills { get; set; } = new List<AdditionalSkill>();

    public enum VacancyType
    {
        Internship,
        Job
    }
}
