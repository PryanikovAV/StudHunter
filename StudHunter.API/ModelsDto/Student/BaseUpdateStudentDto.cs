using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Student;

public class BaseUpdateStudentDto
{
    [StringLength(100, ErrorMessage = "{0} cannot exceed {1} characters")]
    [EmailAddress(ErrorMessage = "Invalid {0} format")]
    public string? Email { get; set; }

    [StringLength(100, ErrorMessage = "{0} cannot exceed {1} characters")]
    [EmailAddress(ErrorMessage = "Invalid {0} format")]
    public string? ContactEmail { get; set; }

    [StringLength(20, ErrorMessage = "{0} cannot exceed {1} characters")]
    [Phone(ErrorMessage = "Invalid {0} format")]
    public string? ContactPhone { get; set; }

    [StringLength(50, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string? FirstName { get; set; }

    [StringLength(50, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string? LastName { get; set; }

    [RegularExpression("Male|Female", ErrorMessage = "{0} must be 'Male' or 'Female'")]
    public string? Gender { get; set; }

    public DateOnly? BirthDate { get; set; }

    [StringLength(200, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string? Photo { get; set; }

    public bool? IsForeign { get; set; }

    public int? StatusId { get; set; }

    // ===== StudyPlan =====
    [Range(1, 7, ErrorMessage = "{0} must be between {1} and {2}")]
    public int? CourseNumber { get; set; }

    public Guid? FacultyId { get; set; }

    public Guid? SpecialityId { get; set; }

    [RegularExpression("fulltime|parttime|correspondence", ErrorMessage = "{0}} must be 'fulltime', 'parttime', or 'correspondence'")]
    public string? StudyForm { get; set; }

    public DateOnly? BeginYear { get; set; }
    // ===== StudyPlan =====
}
