using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.SpecialityDto;

/// <summary>
/// Data transfer object for a speciality.
/// </summary>
public class SpecialityDto
{
    /// <summary>
    /// The unique identifier (GUID) of the speciality.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The name of the speciality.
    /// </summary>
    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The description of the speciality.
    /// </summary>
    [StringLength(1000)]
    public string? Description { get; set; }
}
