using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.VacancyDto;

/// <summary>
/// Data transfer object for a vacancy.
/// </summary>
public class VacancyDto
{
    /// <summary>
    /// The unique identifier (GUID) of the vacancy.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The unique identifier (GUID) of the employer.
    /// </summary>
    [Required]
    public Guid EmployerId { get; set; }

    /// <summary>
    /// The title of the vacancy.
    /// </summary>
    [Required]
    [StringLength(255)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The description of the vacancy.
    /// </summary>
    [StringLength(2500)]
    public string? Description { get; set; }

    /// <summary>
    /// The salary for the vacancy.
    /// </summary>
    public decimal? Salary { get; set; }

    /// <summary>
    /// The date and time the vacancy was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// The date and time the vacancy was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// The type of the vacancy (Internship or Job).
    /// </summary>
    [Required]
    [RegularExpression("Internship|Job")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// The list of course IDs associated with the vacancy.
    /// </summary>
    public List<Guid> CourseIds { get; set; } = new List<Guid>();

    public List<Guid> AdditionalSkills { get; set; } = new List<Guid>;
}
