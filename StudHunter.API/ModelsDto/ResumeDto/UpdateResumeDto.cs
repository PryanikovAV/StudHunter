using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.ResumeDto;

/// <summary>
/// Data transfer object for updating a resume.
/// </summary>
public class UpdateResumeDto
{
    /// <summary>
    /// The title of the resume.
    /// </summary>
    [StringLength(255, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string? Title { get; set; }

    /// <summary>
    /// The description of the resume.
    /// </summary>
    [StringLength(2500, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string? Description { get; set; }
}
