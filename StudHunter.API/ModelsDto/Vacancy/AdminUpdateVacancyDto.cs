using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Vacancy;

/// <summary>
/// Data transfer object for updating a vacancy (administrative functions).
/// </summary>
public class AdminUpdateVacancyDto : UpdateVacancyDto
{
    /// <summary>
    /// Indicates whether the vacancy is deleted.
    /// </summary>
    [Required(ErrorMessage = "{0} is required")]
    public bool? IsDeleted { get; set; }
}
