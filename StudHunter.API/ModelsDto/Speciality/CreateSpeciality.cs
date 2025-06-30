using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Speciality;

public class CreateSpeciality
{
    [Required, MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }
}
