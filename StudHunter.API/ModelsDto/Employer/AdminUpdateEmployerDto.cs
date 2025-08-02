using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Employer;

/// <summary>
/// Data transfer object for updating an employer (administrative functions).
/// </summary>
public class AdminUpdateEmployerDto : BaseUpdateEmployerDto
{
    /// <summary>
    /// Indicates whether the employer is accredited.
    /// </summary>
    [Required(ErrorMessage = "{0} is required")]
    public bool? AccreditationStatus { get; set; }

    /// <summary>
    /// Indicates whether the employer is deleted.
    /// </summary>
    [Required(ErrorMessage = "{0} is required")]
    public bool? IsDeleted { get; set; }
}
