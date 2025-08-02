using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Resume;

/// <summary>
/// Data transfer object for updating a resume (administrative functions).
/// </summary>
public class AdminUpdateResumeDto : UpdateResumeDto
{
    /// <summary>
    /// Indicates whether the resume is deleted.
    /// </summary>
    [Required(ErrorMessage = "{0} is required")]
    public bool? IsDeleted { get; set; }
}
