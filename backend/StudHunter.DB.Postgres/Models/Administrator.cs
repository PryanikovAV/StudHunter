namespace StudHunter.DB.Postgres.Models;

public class Administrator : User
{
    private string _firstName = null!;
    private string _lastName = null!;
    private string? _patronymic;

    public string FirstName
    {
        get => _firstName;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            _firstName = value.Trim();
        }
    }

    public string LastName
    {
        get => _lastName;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            _lastName = value.Trim();
        }
    }

    public string? Patronymic
    {
        get => _patronymic;
        set => _patronymic = value?.Trim();
    }

    public DateTime? LastLoginAt { get; set; }
}
