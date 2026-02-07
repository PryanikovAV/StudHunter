namespace StudHunter.DB.Postgres.Models;

public class University
{
    public Guid Id { get; init; }

    private string _name = null!;
    public string Name
    {
        get => _name;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            _name = value.Trim();
        }
    }

    public string? Abbreviation
    {
        get; set;
    }
}