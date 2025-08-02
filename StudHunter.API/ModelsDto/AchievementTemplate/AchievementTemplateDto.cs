using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.AchievementTemplate;

/// <summary>
/// Data transfer object for an achievement template.
/// </summary>
public class AchievementTemplateDto
{
    /// <summary>
    /// The unique identifier (GUID) of the achievement template.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The order number of the achievement template.
    /// </summary>
    public int OrderNumber { get; set; }

    /// <summary>
    /// The name of the achievement template.
    /// </summary>
    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The description of the achievement template.
    /// </summary>
    [StringLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// The target audience for the achievement (Student or Employer).
    /// </summary>
    [Required]
    [RegularExpression("Student|Employer")]
    public string Target { get; set; } = string.Empty;

    /// <summary>
    /// The URL of the achievement template's icon.
    /// </summary>
    [StringLength(500)]
    public string? IconUrl { get; set; }
}
