using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.AchievementTemplate;

public class CreateAchievementTemplateDto
{
    [Required(ErrorMessage = "{0} is required")]
    [StringLength(255, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "{0} is required")]
    [RegularExpression("Student|Employer", ErrorMessage = "{0} must be 'Student' or 'Employer'")]
    public string Target { get; set; } = string.Empty;
}
