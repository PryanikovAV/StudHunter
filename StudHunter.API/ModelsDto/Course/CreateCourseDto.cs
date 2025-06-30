using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Course;

public class CreateCourseDto
{
    [Required, MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }
}
