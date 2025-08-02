using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Resume;

/// <summary>
/// Data transfer object for a resume.
/// </summary>
public class ResumeDto
{
    /// <summary>
    /// The unique identifier (GUID) of the resume.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The unique identifier (GUID) of the student.
    /// </summary>
    [Required]
    public Guid StudentId { get; set; }

    /// <summary>
    /// The title of the resume.
    /// </summary>
    [Required]
    [StringLength(255)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The description of the resume.
    /// </summary>
    [StringLength(2500)]
    public string? Description { get; set; }

    /// <summary>
    /// The date and time the resume was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// The date and time the resume was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
