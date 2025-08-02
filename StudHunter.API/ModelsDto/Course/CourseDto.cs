using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Course;

/// <summary>
/// Data transfer object for a course.
/// </summary>
public class CourseDto
{
    /// <summary>
    /// The unique identifier (GUID) of the course.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The name of the course.
    /// </summary>
    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The description of the course.
    /// </summary>
    [StringLength(1000)]
    public string? Description { get; set; }
}
