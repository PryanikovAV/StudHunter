using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.FacultyDto;

/// <summary>
/// Data transfer object for creating a faculty.
/// </summary>
public class CreateFacultyDto
{
    /// <summary>
    /// The name of the faculty.
    /// </summary>
    [Required(ErrorMessage = "{0} is required")]
    [StringLength(255, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The description of the faculty.
    /// </summary>
    [StringLength(1000, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string? Description { get; set; }
}
