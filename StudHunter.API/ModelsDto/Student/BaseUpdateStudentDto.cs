using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Student;

/// <summary>
/// Base data transfer object for updating student information.
/// </summary>
public class BaseUpdateStudentDto
{
    /// <summary>
    /// The student's email address (optional).
    /// </summary>
    [StringLength(100, ErrorMessage = "{0} cannot exceed {1} characters")]
    [EmailAddress(ErrorMessage = "Invalid {0} format")]
    public string? Email { get; set; }

    /// <summary>
    /// The student's first name (optional).
    /// </summary>
    [StringLength(50, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string? FirstName { get; set; }

    /// <summary>
    /// The student's last name (optional).
    /// </summary>
    [StringLength(50, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string? LastName { get; set; }

    /// <summary>
    /// The student's contact phone number (optional).
    /// </summary>
    [StringLength(20, ErrorMessage = "{0} cannot exceed {1} characters")]
    [Phone(ErrorMessage = "Invalid {0} format")]
    public string? ContactPhone { get; set; }

    /// <summary>
    /// The student's contact email address (optional).
    /// </summary>
    [StringLength(100, ErrorMessage = "{0} cannot exceed {1} characters")]
    [EmailAddress(ErrorMessage = "Invalid {0} format")]
    public string? ContactEmail { get; set; }

    /// <summary>
    /// The student's gender (Male or Female, optional).
    /// </summary>
    [RegularExpression("Male|Female", ErrorMessage = "{0} must be 'Male' or 'Female'")]
    public string? Gender { get; set; }

    /// <summary>
    /// The student's date of birth (optional).
    /// </summary>
    public DateOnly? BirthDate { get; set; }

    /// <summary>
    /// The student's profile photo URL (optional).
    /// </summary>
    [StringLength(200, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string? Photo { get; set; }

    /// <summary>
    /// Indicates whether the student is foreign (optional).
    /// </summary>
    public bool? IsForeign { get; set; }

    /// <summary>
    /// The student's status (Studying, SeekingInternship, SeekingJob, Interning, Working, optional).
    /// </summary>
    [RegularExpression("Studying|SeekingInternship|SeekingJob|Interning|Working", ErrorMessage = "{0} must be 'Studying', 'SeekingInternship', 'SeekingJob', 'Interning', or 'Working'")]
    public string? Status { get; set; }
}
