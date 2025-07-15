using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.AchievementTemplate;

public class UpdateAchievementTemplateDto
{
    [MaxLength(255)]
    public string? Name { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Required, RegularExpression("Student|Employer", ErrorMessage = "Target must be 'Student' or 'Employer'")]
    public string? Target { get; set; }
}
