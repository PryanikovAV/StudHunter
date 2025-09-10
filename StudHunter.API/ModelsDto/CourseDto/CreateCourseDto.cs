using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.CourseDto;

/// <summary>
/// Data transfer object for creating a course.
/// </summary>
public class CreateCourseDto
{
    /// <summary>
    /// The name of the course.
    /// </summary>
    [Required(ErrorMessage = "{0} is required")]
    [StringLength(255, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The description of the course.
    /// </summary>
    [StringLength(1000, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string? Description { get; set; }
}
