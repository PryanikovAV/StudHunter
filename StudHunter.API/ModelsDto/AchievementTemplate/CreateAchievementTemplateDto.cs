using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.AchievementTemplate;

public class CreateAchievementTemplateDto
{
    [Required, MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required, RegularExpression("Student|Employer", ErrorMessage = "Target must be 'Student' or 'Employer'")]
    public string Target { get; set; } = string.Empty;
}
