using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Vacancy;

public class UpdateVacancyDto
{
    [StringLength(255, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string? Title { get; set; }

    [StringLength(2500, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string? Description { get; set; }

    [Range(0, 1000000, ErrorMessage = "{0} must be between 0 and 1,000,000.")]
    public decimal? Salary { get; set; }

    [RegularExpression("Internship|Job", ErrorMessage = "{0} must be 'Internship' or 'Job'")]
    public string? Type { get; set; }
}
