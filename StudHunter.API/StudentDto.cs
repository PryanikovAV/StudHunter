using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.Models;

public class StudentDto
{
    public Guid Id { get; set; }

    [Required]
    [StringLength(50), MinLength(1)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(50), MinLength(1)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = string.Empty;

    [Required]
    public string Gender { get; set; } = string.Empty;

    [Required]
    public DateOnly BirthDate { get; set; }

    public string? Photo { get; set; }

    public string? ContactPhone { get; set; }

    public bool IsForeign { get; set; } = false;

    public Guid? ResumeId { get; set; }

    [Required]
    public Guid StudyPlanId { get; set; }
}
