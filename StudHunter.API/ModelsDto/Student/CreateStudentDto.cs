using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Student;

public class CreateStudentDto
{
    [Required, MaxLength(100), EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [EmailAddress, MaxLength(100)]
    public string? ContactEmail { get; set; }

    [Phone, MaxLength(20)]
    public string? ContactPhone { get; set; }

    [Required, MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required, RegularExpression("Male|Female", ErrorMessage = "Gender must be 'Male' or 'Female'")]
    public string Gender { get; set; } = string.Empty;

    [Required]
    public DateOnly BirthDate { get; set; }

    public string? Photo { get; set; }

    public bool IsForeign { get; set; }

    public int? StatusId { get; set; }

    // ===== StudyPlan =====
    public int CourseNumber { get; set; } = 1;

    [Required]
    public Guid FacultyId { get; set; }

    [Required]
    public Guid SpecialityId { get; set; }
    
    [Required, RegularExpression("fulltime|parttime|correspondence", ErrorMessage = "StudyForm must be 'fulltime', 'parttime', or 'correspondence'")]
    public string StudyForm { get; set; } = string.Empty;
    
    [Required]
    public DateOnly BeginYear { get; set; }
    // ===== StudyPlan =====
}
