using StudHunter.API.ModelsDto.UserAchievement;
using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Employer;

/// <summary>
/// Data transfer object for an employer.
/// </summary>
public class EmployerDto
{
    /// <summary>
    /// The unique identifier (GUID) of the employer.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The role of the user (always "Employer").
    /// </summary>
    public string Role { get; } = "Employer";

    /// <summary>
    /// The employer's email address.
    /// </summary>
    [Required]
    [StringLength(100)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// The employer's contact email address.
    /// </summary>
    [StringLength(100)]
    [EmailAddress]
    public string? ContactEmail { get; set; }

    /// <summary>
    /// The employer's contact phone number.
    /// </summary>
    [StringLength(20)]
    [Phone]
    public string? ContactPhone { get; set; }

    /// <summary>
    /// The date and time the employer was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Indicates whether the employer is accredited.
    /// </summary>
    public bool AccreditationStatus { get; set; }

    /// <summary>
    /// The employer's name.
    /// </summary>
    [Required]
    [StringLength(255)]
    public string Name { get; set; } = null!;

    /// <summary>
    /// The employer's description.
    /// </summary>
    [StringLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// The employer's website URL.
    /// </summary>
    [StringLength(255)]
    [Url]
    public string? Website { get; set; }

    /// <summary>
    /// The employer's specialization.
    /// </summary>
    [StringLength(255)]
    public string? Specialization { get; set; }

    /// <summary>
    /// The list of vacancy IDs associated with the employer.
    /// </summary>
    public List<Guid> VacancyIds { get; set; } = new List<Guid>();

    /// <summary>
    /// The list of employer's achievements.
    /// </summary>
    public List<UserAchievementDto> Achievements { get; set; } = new List<UserAchievementDto>();
}
