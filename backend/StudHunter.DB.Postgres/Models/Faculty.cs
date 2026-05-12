namespace StudHunter.DB.Postgres.Models;

public class Faculty
{
    public Guid Id { get; init; }
    public Guid UniversityId { get; set; }

    private string _name = null!;
    private string? _abbreviation;
    public string Name
    {
        get => _name;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            _name = value.Trim();
        }
    }

    public string? Abbreviation
    {
        get => _abbreviation;
        set
        {
            if (value is not null)
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(value);
                _abbreviation = value.Trim();
            }
            else
            {
                _abbreviation = null;
            }
        }
    }

    public string? Description { get; set; }
    public virtual University University { get; set; } = null!;
    public virtual ICollection<Department> Departments { get; set; } = new HashSet<Department>();
    public virtual ICollection<StudyPlan> StudyPlans { get; set; } = new HashSet<StudyPlan>();
}
