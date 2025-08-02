using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Student;

/// <summary>
/// Base data transfer object for updating a student.
/// </summary>
public class BaseUpdateStudentDto
{
    /// <summary>
    /// The student's email address.
    /// </summary>
    [StringLength(100, ErrorMessage = "{0} cannot exceed {1} characters")]
    [EmailAddress(ErrorMessage = "Invalid {0} format")]
    public string? Email { get; set; }

    /// <summary>
    /// The student's contact email address.
    /// </summary>
    [StringLength(100, ErrorMessage = "{0} cannot exceed {1} characters")]
    [EmailAddress(ErrorMessage = "Invalid {0} format")]
    public string? ContactEmail { get; set; }

    /// <summary>
    /// The student's contact phone number.
    /// </summary>
    [StringLength(20, ErrorMessage = "{0} cannot exceed {1} characters")]
    [Phone(ErrorMessage = "Invalid {0} format")]
    public string? ContactPhone { get; set; }

    /// <summary>
    /// The student's first name.
    /// </summary>
    [StringLength(50, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string? FirstName { get; set; }

    /// <summary>
    /// The student's last name.
    /// </summary>
    [StringLength(50, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string? LastName { get; set; }

    /// <summary>
    /// The student's gender (Male or Female).
    /// </summary>
    [RegularExpression("Male|Female", ErrorMessage = "{0} must be 'Male' or 'Female'")]
    public string? Gender { get; set; }

    /// <summary>
    /// The student's date of birth.
    /// </summary>
    public DateOnly? BirthDate { get; set; }

    /// <summary>
    /// The student's profile photo URL (optional).
    /// </summary>
    [StringLength(200, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string? Photo { get; set; }

    /// <summary>
    /// Indicates whether the student is foreign.
    /// </summary>
    public bool? IsForeign { get; set; }

    /// <summary>
    /// The student's status ID (optional).
    /// </summary>
    public int? StatusId { get; set; }

    /// <summary>
    /// The course number for the study plan.
    /// </summary>
    [Range(1, 7, ErrorMessage = "{0} must be between {1} and {2}")]
    public int? CourseNumber { get; set; }

    /// <summary>
    /// The faculty ID for the study plan.
    /// </summary>
    public Guid? FacultyId { get; set; }

    /// <summary>
    /// The speciality ID for the study plan.
    /// </summary>
    public Guid? SpecialityId { get; set; }

    /// <summary>
    /// The study form (FullTime, PartTime, or Correspondence).
    /// </summary>
    [RegularExpression("FullTime|PartTime|Correspondence", ErrorMessage = "{0} must be 'FullTime', 'PartTime', or 'Correspondence'")]
    public string? StudyForm { get; set; }

    /// <summary>
    /// The year the student began studying.
    /// </summary>
    [Range(2000, 2025, ErrorMessage = "{0} must be between {1} and {2}")]
    public DateOnly? BeginYear { get; set; }
}
