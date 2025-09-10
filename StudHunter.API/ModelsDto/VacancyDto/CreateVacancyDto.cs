using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.VacancyDto;

/// <summary>
/// Data transfer object for creating a vacancy.
/// </summary>
public class CreateVacancyDto
{
    /// <summary>
    /// The title of the vacancy.
    /// </summary>
    [Required(ErrorMessage = "{0} is required")]
    [StringLength(255, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The description of the vacancy.
    /// </summary>
    [StringLength(2500, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string? Description { get; set; }

    /// <summary>
    /// The salary for the vacancy.
    /// </summary>
    [Range(0, 1000000, ErrorMessage = "{0} must be between 0 and 1,000,000")]
    public decimal? Salary { get; set; }

    /// <summary>
    /// The type of the vacancy (Internship or Job).
    /// </summary>
    [Required(ErrorMessage = "{0} is required")]
    [RegularExpression("Internship|Job", ErrorMessage = "{0} must be 'Internship' or 'Job'")]
    public string Type { get; set; } = string.Empty;
}
