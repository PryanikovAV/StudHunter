using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Faculty;

/// <summary>
/// Data transfer object for updating a faculty.
/// </summary>
public class UpdateFacultyDto
{
    /// <summary>
    /// The name of the faculty.
    /// </summary>
    [StringLength(255, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string? Name { get; set; }

    /// <summary>
    /// The description of the faculty.
    /// </summary>
    [StringLength(1000, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string? Description { get; set; }
}
