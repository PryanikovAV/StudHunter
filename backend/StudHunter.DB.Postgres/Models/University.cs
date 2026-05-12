namespace StudHunter.DB.Postgres.Models;

public class University
{
    public Guid Id { get; init; }
    public Guid CityId { get; set; }
    private string _name = null!;
    private string _abbreviation = null!;
    
    public string Name
    {
        get => _name;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            _name = value.Trim();
        }
    }

    public string Abbreviation
    {
        get => _abbreviation;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            _abbreviation = value.Trim();
        }
    }

    public virtual City City { get; set; } = null!;
    public virtual ICollection<Faculty> Faculties { get; set; } = new HashSet<Faculty>();
    public virtual ICollection<StudyPlan> StudyPlans { get; set; } = new HashSet<StudyPlan>();
}