namespace StudHunter.DB.Postgres.Models;

public class ResumeAdditionalSkill
{
    public Guid ResumeId { get; init; }
    public Guid AdditionalSkillId { get; init; }

    public virtual Resume Resume { get; set; } = null!;
    public virtual AdditionalSkill AdditionalSkill { get; set; } = null!;
}