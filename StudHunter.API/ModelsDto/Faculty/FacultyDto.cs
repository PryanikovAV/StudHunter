using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Faculty;

/// <summary>
/// Data transfer object for a faculty.
/// </summary>
public class FacultyDto
{
    /// <summary>
    /// The unique identifier (GUID) of the faculty.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The name of the faculty.
    /// </summary>
    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The description of the faculty.
    /// </summary>
    [StringLength(1000)]
    public string? Description { get; set; }
}
