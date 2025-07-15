using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Vacancy;

public class VacancyDto
{
    public Guid Id { get; set; }

    [Required]
    public Guid EmployerId { get; set; }

    [Required, MaxLength(255)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2500)]
    public string? Description { get; set; }

    public decimal? Salary { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    [Required, RegularExpression("Internship|Job", ErrorMessage = "Type must be 'Internship' or 'Job'")]
    public string Type { get; set; } = string.Empty;

    public List<Guid> CourseIds { get; set; } = new List<Guid>();
}
