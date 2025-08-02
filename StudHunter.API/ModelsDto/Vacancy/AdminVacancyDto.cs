using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Vacancy;

/// <summary>
/// Data transfer object for a vacancy (administrative functions).
/// </summary>
public class AdminVacancyDto : VacancyDto
{
    /// <summary>
    /// Indicates whether the vacancy is deleted.
    /// </summary>
    public bool IsDeleted { get; set; }
}
