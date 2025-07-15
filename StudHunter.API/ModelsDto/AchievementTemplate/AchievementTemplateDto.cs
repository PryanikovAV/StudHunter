using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.AchievementTemplate;

public class AchievementTemplateDto
{
    public int Id { get; set; }

    [Required, MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Required]
    public string Target { get; set; } = string.Empty;
}
