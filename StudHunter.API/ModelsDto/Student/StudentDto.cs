using StudHunter.API.ModelsDto.UserAchievement;
using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Student;

/// <summary>
/// Data transfer object for a student.
/// </summary>
public class StudentDto
{
    /// <summary>
    /// The unique identifier (GUID) of the student.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The role of the user (always "Student").
    /// </summary>
    [Required]
    public string Role { get; set; } = "Student";

    /// <summary>
    /// The student's email address.
    /// </summary>
    [Required]
    [StringLength(100)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// The student's contact email address.
    /// </summary>
    [StringLength(100)]
    [EmailAddress]
    public string? ContactEmail { get; set; }

    /// <summary>
    /// The student's contact phone number.
    /// </summary>
    [StringLength(20)]
    [Phone]
    public string? ContactPhone { get; set; }

    /// <summary>
    /// The date and time the student was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

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
    /// The student's gender (Male or Female).
    /// </summary>
    [Required]
    [RegularExpression("Male|Female")]
    public string Gender { get; set; } = string.Empty;

    /// <summary>
    /// The student's date of birth.
    /// </summary>
    public DateOnly BirthDate { get; set; }

    /// <summary>
    /// The student's profile photo URL.
    /// </summary>
    [StringLength(200)]
    public string? Photo { get; set; }

    /// <summary>
    /// Indicates whether the student is foreign.
    /// </summary>
    public bool IsForeign { get; set; }

    /// <summary>
    /// The student's status ID.
    /// </summary>
    public int? StatusId { get; set; }

    /// <summary>
    /// The unique identifier (GUID) of the student's resume.
    /// </summary>
    public Guid? ResumeId { get; set; }

    /// <summary>
    /// The list of student's achievements.
    /// </summary>
    public List<UserAchievementDto> Achievements { get; set; } = new List<UserAchievementDto>();

    // ===== StudyPlan =====
    /// <summary>
    /// The course number for the study plan.
    /// </summary>
    public int CourseNumber { get; set; }

    /// <summary>
    /// The faculty ID for the study plan.
    /// </summary>
    public Guid FacultyId { get; set; }

    /// <summary>
    /// The speciality ID for the study plan.
    /// </summary>
    public Guid SpecialityId { get; set; }

    /// <summary>
    /// The study form (FullTime, PartTime, or Correspondence).
    /// </summary>
    [RegularExpression("FullTime|PartTime|Correspondence")]
    public string StudyForm { get; set; } = string.Empty;

    /// <summary>
    /// The year the student began studying.
    /// </summary>
    public DateOnly BeginYear { get; set; }
    // ===== StudyPlan =====
}
