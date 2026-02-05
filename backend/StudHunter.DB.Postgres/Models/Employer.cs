namespace StudHunter.DB.Postgres.Models;

public class Employer : User
{
    // TODO: добавить таблицу EmployerDetails и вынести туда ИНН, ОГРН, адрес и прочие реквизиты
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? Website { get; set; }
    public string? Specialization { get; set; }
    public virtual ICollection<Vacancy> Vacancies { get; set; } = new HashSet<Vacancy>();
}
