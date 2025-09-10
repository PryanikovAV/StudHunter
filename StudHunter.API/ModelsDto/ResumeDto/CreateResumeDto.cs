using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.ResumeDto;

/// <summary>
/// Data transfer object for creating a resume.
/// </summary>
public class CreateResumeDto
{
    /// <summary>
    /// The title of the resume.
    /// </summary>
    [Required(ErrorMessage = "{0} is required")]
    [StringLength(255, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The description of the resume.
    /// </summary>
    [StringLength(2500, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string? Description { get; set; }
}
