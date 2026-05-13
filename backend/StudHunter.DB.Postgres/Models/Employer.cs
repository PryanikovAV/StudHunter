namespace StudHunter.DB.Postgres.Models;

public class Employer : User
{
    public string Name { get; set; } = null!;  // TODO: проработать запись в БД: буквы, пробьелы, символы
    public string? Description { get; set; }
    public string? Website { get; set; }
    public Guid? SpecializationId { get; set; }
    public virtual Specialization? Specialization { get; set; }
    public virtual ICollection<Vacancy> Vacancies { get; set; } = new HashSet<Vacancy>();
    public virtual ICollection<Invitation> Invitations { get; set; } = new HashSet<Invitation>();
    public virtual OrganizationDetail? OrganizationDetails { get; set; }
}
