using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Speciality;

public class UpdateSpecialiityDto
{
    [MaxLength(255)]
    public string? Name { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }
}
