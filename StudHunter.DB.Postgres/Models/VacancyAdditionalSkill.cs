namespace StudHunter.DB.Postgres.Models;

public class VacancyAdditionalSkill
{
    public Guid VacancyId { get; init; }
    public Guid AdditionalSkillId { get; init; }

    public virtual Vacancy Vacancy { get; set; } = null!;
    public virtual AdditionalSkill AdditionalSkill { get; set; } = null!;
}