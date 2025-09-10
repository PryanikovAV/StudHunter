using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.SpecialityDto;

/// <summary>
/// Data transfer object for updating a speciality.
/// </summary>
public class UpdateSpecialityDto
{
    /// <summary>
    /// The name of the speciality.
    /// </summary>
    [StringLength(255, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string? Name { get; set; }

    /// <summary>
    /// The description of the speciality.
    /// </summary>
    [StringLength(1000, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string? Description { get; set; }
}
