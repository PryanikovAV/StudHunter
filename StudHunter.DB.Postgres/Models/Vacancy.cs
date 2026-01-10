namespace StudHunter.DB.Postgres.Models;

public class Vacancy
{
    public Guid Id { get; init; }
    public Guid EmployerId { get; set; }

    private string _title = null!;
    public string Title
    {
        get => _title;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            _title = value.Trim();
        }
    }

    public string? Description { get; set; }
    public decimal? Salary { get; set; }

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public VacancyType Type { get; set; }

    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }

    public virtual Employer Employer { get; set; } = null!;
    public virtual ICollection<VacancyCourse> Courses { get; set; } = new HashSet<VacancyCourse>();
    public virtual ICollection<Invitation> Invitations { get; set; } = new HashSet<Invitation>();
    public virtual ICollection<VacancyAdditionalSkill> AdditionalSkills { get; set; } = new HashSet<VacancyAdditionalSkill>();
    public enum VacancyType
    {
        Internship = 0,
        Job = 1
    }
}
