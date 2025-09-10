namespace StudHunter.DB.Postgres.Models;

public class AdditionalSkill
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public virtual ICollection<StudentAdditionalSkill> StudentAdditionalSkills { get; set; } = new List<StudentAdditionalSkill>();
}
