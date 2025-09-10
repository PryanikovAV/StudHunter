using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.SpecialityDto;

/// <summary>
/// Data transfer object for creating a speciality.
/// </summary>
public class CreateSpecialityDto
{
    /// <summary>
    /// The name of the speciality.
    /// </summary>
    [Required(ErrorMessage = "{0} is required")]
    [StringLength(255, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The description of the speciality.
    /// </summary>
    [StringLength(1000, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string? Description { get; set; }
}
