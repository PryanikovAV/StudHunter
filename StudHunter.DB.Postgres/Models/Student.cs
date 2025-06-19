namespace StudHunter.DB.Postgres.Models;

public class Student : User
{
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public StudentGender Gender { get; set; }

    public DateOnly BirthDate { get; set; }

    public string? Photo { get; set; }

    public bool IsForeign { get; set; }

    public int? StatusId { get; set; }

    public virtual StudentStatus? Status { get; set; }    
    public virtual StudyPlan StudyPlan { get; set; } = null!;
    public virtual Resume? Resume { get; set; }

    public enum StudentGender
    {
        Male,
        Female
    }
}
