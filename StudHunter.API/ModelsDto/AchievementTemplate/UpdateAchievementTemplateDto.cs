using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.AchievementTemplate;

public class UpdateAchievementTemplateDto
{
    [StringLength(255, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string? Name { get; set; }

    [StringLength(1000, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    [RegularExpression("Student|Employer", ErrorMessage = "Target must be 'Student' or 'Employer'")]
    public string? Target { get; set; }
}
