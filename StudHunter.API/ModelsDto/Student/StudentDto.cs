using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Student;

public class StudentDto
{
    public Guid Id { get; set; }

    [Required]
    public string Role => "Student";

    [Required, EmailAddress, MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [EmailAddress, MaxLength(100)]
    public string? ContactEmail { get; set; }

    [Phone, MaxLength(20)]
    public string? ContactPhone { get; set; }

    public DateTime CreatedAt { get; set; }

    [Required, MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required, RegularExpression("Male|Female", ErrorMessage = "Gender must be 'Male' or 'Female'")]
    public string Gender { get; set; } = string.Empty;

    public DateOnly BirthDate { get; set; }

    public string? Photo { get; set; }

    public bool IsForeign { get; set; }

    public int? StatusId { get; set; }

    public Guid? ResumeId { get; set; }

    // ===== StudyPlan =====
    public int CourseNumber { get; set; }

    public Guid FacultyId { get; set; }

    public Guid SpecialityId { get; set; }

    [RegularExpression("fulltime|parttime|correspondence", ErrorMessage = "StudyForm must be 'fulltime', 'parttime', or 'correspondence'")]
    public string StudyForm { get; set; } = string.Empty;

    public DateOnly BeginYear { get; set; }
    // ===== StudyPlan =====
}
