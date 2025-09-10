using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.AdditionalSkillDto;

public class AdditionalSkillDto
{
    public Guid Id { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    [StringLength(255)]
    public string? Description { get; set; }
}
