namespace StudHunter.DB.Postgres.Models;

public class Student : User
{
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public StudentGender Gender { get; set; }

    public DateOnly BirthDate { get; set; } = DateOnly.MinValue;

    public string? Photo { get; set; }

    public bool IsForeign { get; set; }

    public StudentStatus Status { get; set; }

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

    public void UpdateEmail(string email) => SetEmail(email);
    public void UpdatePassword(string passwordHash) => SetPasswordHash(passwordHash);
}
