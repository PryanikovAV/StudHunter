using StudHunter.API.ModelsDto.UserAchievement;
using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Student;

/// <summary>
/// Data transfer object representing a student.
/// </summary>
public class StudentDto
{
    /// <summary>
    /// The unique identifier (GUID) of the student.
    /// </summary>
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// The student's email address.
    /// </summary>
    [Required]
    [StringLength(100)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// The student's first name.
    /// </summary>
    [Required]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// The student's last name.
    /// </summary>
    [Required]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// The student's contact phone number (optional).
    /// </summary>
    [StringLength(20)]
    [Phone]
    public string? ContactPhone { get; set; }

    /// <summary>
    /// The student's contact email address (optional).
    /// </summary>
    [StringLength(100)]
    [EmailAddress]
    public string? ContactEmail { get; set; }

    /// <summary>
    /// The date and time the student was created.
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// The student's gender (Male or Female).
    /// </summary>
    [Required]
    [RegularExpression("Male|Female")]
    public string Gender { get; set; } = string.Empty;

    /// <summary>
    /// The student's date of birth (optional).
    /// </summary>
    public DateOnly? BirthDate { get; set; }

    /// <summary>
    /// The student's profile photo URL (optional).
    /// </summary>
    [StringLength(200)]
    public string? Photo { get; set; }

    /// <summary>
    /// Indicates whether the student is foreign (default: false).
    /// </summary>
    [Required]
    public bool IsForeign { get; set; }

    /// <summary>
    /// The student's status (Studying, SeekingInternship, SeekingJob, Interning, Working).
    /// </summary>
    [Required]
    [RegularExpression("Studying|SeekingInternship|SeekingJob|Interning|Working")]
    public string Status { get; set; } = "Studying";

    /// <summary>
    /// The unique identifier (GUID) of the student's resume (optional).
    /// </summary>
    public Guid? ResumeId { get; set; }

    /// <summary>
    /// The list of student's achievements (optional).
    /// </summary>
    public List<UserAchievementDto> Achievements { get; set; } = new List<UserAchievementDto>();
}
