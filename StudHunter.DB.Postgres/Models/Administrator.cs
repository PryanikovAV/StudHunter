namespace StudHunter.DB.Postgres.Models;

public class Administrator : User
{
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Patronymic { get; set; }

    public void UpdateEmail(string email) => SetEmail(email);

    public void UpdatePassword(string passwordHash) => SetPasswordHash(passwordHash);
}
