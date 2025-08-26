namespace StudHunter.DB.Postgres.Models;

public class Employer : User
{
    public bool AccreditationStatus { get; set; } = false;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Website { get; set; }

    public string? Specialization { get; set; }

    public virtual ICollection<Vacancy> Vacancies { get; set; } = new List<Vacancy>();

    public void UpdateEmail(string email) => SetEmail(email);

    public void UpdatePassword(string passwordHash) => SetPasswordHash(passwordHash);
}
