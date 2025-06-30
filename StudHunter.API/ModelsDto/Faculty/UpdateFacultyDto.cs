using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Faculty;

public class UpdateFacultyDto
{
    [MaxLength(255)]
    public string? Name { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }
}
