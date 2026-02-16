namespace StudHunter.DB.Postgres.Models;
// TODO: Добавить связь с городом, факультетами, кафедрами и специальностями
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