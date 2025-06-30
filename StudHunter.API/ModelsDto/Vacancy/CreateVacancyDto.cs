using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Vacancy;

public class CreateVacancyDto
{
    [Required]
    public Guid EmployerId { get; set; }

    [Required, MaxLength(255)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2500)]
    public string? Description { get; set; }

    [Range(0, 1000000, ErrorMessage = "Salary must be between 0 and 1,000,000.")]
    public decimal? Salary { get; set; }

    [Required, RegularExpression("Internship|Job", ErrorMessage = "Type must be 'Internship' or 'Job'")]
    public string Type { get; set; } = string.Empty;
}
