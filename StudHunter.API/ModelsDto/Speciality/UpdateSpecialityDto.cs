using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Speciality;

public class UpdateSpecialityDto
{
    [StringLength(255, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string? Name { get; set; }

    [StringLength(1000, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string? Description { get; set; }
}
