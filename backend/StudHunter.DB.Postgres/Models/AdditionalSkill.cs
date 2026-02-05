namespace StudHunter.DB.Postgres.Models;

public class AdditionalSkill
{
    public Guid Id { get; init; }

    private string _name = null!;
    public string Name
    {
        get => _name;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            _name = value.Trim().ToLower();
        }
    }

    public virtual ICollection<ResumeAdditionalSkill> ResumeAdditionalSkills { get; set; } = new HashSet<ResumeAdditionalSkill>();
    public virtual ICollection<VacancyAdditionalSkill> VacancyAdditionalSkills { get; set; } = new HashSet<VacancyAdditionalSkill>();
}