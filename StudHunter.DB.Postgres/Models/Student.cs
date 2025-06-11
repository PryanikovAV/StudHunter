using System.ComponentModel.DataAnnotations;
namespace StudHunter.DB.Postgres.Models;

public class Student : User
{
    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string FirstName { get; set; } = null!;

    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string LastName { get; set; } = null!;

    [Required]
    public StudentGender Gender { get; set; }

    [Required]
    public DateOnly BirthDate { get; set; }

    [StringLength(255)]
    public string? Photo { get; set; }

    [StringLength(20)]
    public string? ContactPhone { get; set; }

    public bool IsForeign { get; set; } = false;

    [Required]
    [Range(1, 10)]
    public int CourseNumber { get; set; }

    [Required]
    public int StatusId { get; set; }

    public virtual StudentStatus Status { get; set; } = null!;    
    public virtual StudyPlan StudyPlan { get; set; } = null!;
    public virtual Resume? Resume { get; set; }
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    
    public enum StudentGender   
    {
        Male,
        Female
    }
}
