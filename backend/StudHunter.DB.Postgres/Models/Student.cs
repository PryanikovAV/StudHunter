namespace StudHunter.DB.Postgres.Models;

public class Student : User
{
    private string _firstName = null!;
    public string FirstName
    {
        get => _firstName;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            _firstName = value.Trim();
        }
    }
    private string _lastName = null!;
    public string LastName
    {
        get => _lastName;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            _lastName = value.Trim();
        }
    }

    private string? _patronymic;
    public string? Patronymic
    {
        get => _patronymic;
        set => _patronymic = value?.Trim();
    }

    public StudentGender? Gender { get; set; }
    public DateOnly? BirthDate { get; set; }
    public bool? IsForeign { get; set; }
    public StudentStatus Status { get; set; } = StudentStatus.Studying;

    public virtual StudyPlan? StudyPlan { get; set; }
    public virtual Resume? Resume { get; set; }

    public enum StudentGender
    {
        Male,
        Female
    }

    public enum StudentStatus
    {
        Studying = 1,
        SeekingInternship = 2,
        SeekingJob = 3,
        Interning = 4,
        Working = 5
    }
}
