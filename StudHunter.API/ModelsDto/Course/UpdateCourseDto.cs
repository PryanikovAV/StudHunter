using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Course;

public class UpdateCourseDto
{
    [MaxLength(255)]
    public string? Name { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }
}
