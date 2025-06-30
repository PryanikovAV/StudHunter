using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Faculty;

public class CreateFacultyDto
{
    [Required, MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }
}
