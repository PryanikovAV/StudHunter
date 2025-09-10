using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.AchievementTemplateDto;

/// <summary>
/// Data transfer object for creating an achievement template.
/// </summary>
public class CreateAchievementTemplateDto
{
    /// <summary>
    /// The order number of the achievement template.
    /// </summary>
    [Required(ErrorMessage = "{0} is required")]
    public int OrderNumber { get; set; }

    /// <summary>
    /// The name of the achievement template.
    /// </summary>
    [Required(ErrorMessage = "{0} is required")]
    [StringLength(255, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The description of the achievement template.
    /// </summary>
    [StringLength(1000, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The target audience for the achievement (Student or Employer).
    /// </summary>
    [Required(ErrorMessage = "{0} is required")]
    [RegularExpression("Student|Employer", ErrorMessage = "{0} must be 'Student' or 'Employer'")]
    public string Target { get; set; } = string.Empty;

    /// <summary>
    /// The URL of the achievement template's icon.
    /// </summary>
    [StringLength(500, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string IconUrl { get; set; } = string.Empty;
}
