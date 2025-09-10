using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.AuthDto;

/// <summary>
/// Data transfer object for registering a student.
/// </summary>
public class RegisterStudentDto
{
    /// <summary>
    /// The student's email address.
    /// </summary>
    [Required(ErrorMessage = "{0} is required")]
    [StringLength(100, ErrorMessage = "{0} cannot exceed {1} characters")]
    [EmailAddress(ErrorMessage = "Invalid {0} format")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// The student's password.
    /// </summary>
    [Required(ErrorMessage = "{0} is required")]
    [StringLength(255, MinimumLength = 8, ErrorMessage = "{0} must be between {2} and {1} characters")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// The student's first name.
    /// </summary>
    [Required(ErrorMessage = "{0} is required")]
    [StringLength(50, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// The student's last name.
    /// </summary>
    [Required(ErrorMessage = "{0} is required")]
    [StringLength(50, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// The student's contact phone number (optional).
    /// </summary>
    [StringLength(20, ErrorMessage = "{0} cannot exceed {1} characters")]
    [Phone(ErrorMessage = "Invalid {0} format")]
    public string? ContactPhone { get; set; }

    /// <summary>
    /// The student's gender (Male or Female).
    /// </summary>
    [Required(ErrorMessage = "{0} is required")]
    [RegularExpression("Male|Female", ErrorMessage = "{0} must be 'Male' or 'Female'")]
    public string Gender { get; set; } = string.Empty;

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
    /// Indicates whether the student is foreign (default: false).
    /// </summary>
    [Required(ErrorMessage = "{0} is required")]
    public bool IsForeign { get; set; } = false;

    /// <summary>
    /// The student's status (Studying, SeekingInternship, SeekingJob, Interning, Working, default: Studying).
    /// </summary>
    [Required(ErrorMessage = "{0} is required")]
    [RegularExpression("Studying|SeekingInternship|SeekingJob|Interning|Working", ErrorMessage = "{0} must be 'Studying', 'SeekingInternship', 'SeekingJob', 'Interning', or 'Working'")]
    public string Status { get; set; } = "Studying";
}
