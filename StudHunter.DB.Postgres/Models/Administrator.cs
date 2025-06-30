namespace StudHunter.DB.Postgres.Models;

public class Administrator : User
{
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public AdministratorLevel AdminLevel { get; set; }

    public enum AdministratorLevel
    {
        SuperAdmin,
        Moderator
    }
}
