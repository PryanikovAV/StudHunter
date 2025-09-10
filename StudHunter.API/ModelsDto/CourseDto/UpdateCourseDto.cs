using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.CourseDto;

/// <summary>
/// Data transfer object for updating a course.
/// </summary>
public class UpdateCourseDto
{
    /// <summary>
    /// The name of the course.
    /// </summary>
    [StringLength(255, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string? Name { get; set; }

    /// <summary>
    /// The description of the course.
    /// </summary>
    [StringLength(1000, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string? Description { get; set; }
}
