namespace StudHunter.DB.Postgres.Models;

public class Department
{
    public Guid Id { get; init; }
    public Guid FacultyId { get; set; }
    private string _name = null!;
    
    public string Name
    {
        get => _name;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            _name = value.Trim();
        }
    }

    public string? Description { get; set; }
    public virtual Faculty Faculty { get; set; } = null!;
    public virtual ICollection<StudyDirection> StudyDirections { get; set; } = new HashSet<StudyDirection>();
    public virtual ICollection<StudyPlan> StudyPlans { get; set; } = new HashSet<StudyPlan>();
}