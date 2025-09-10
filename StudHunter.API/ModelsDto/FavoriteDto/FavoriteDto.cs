using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.FavoriteDto;

/// <summary>
/// Data transfer object for a favorite.
/// </summary>
public class FavoriteDto
{
    /// <summary>
    /// The unique identifier (GUID) of the favorite.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The unique identifier (GUID) of the user.
    /// </summary>
    [Required(ErrorMessage = "{0} is required")]
    public Guid UserId { get; set; }

    /// <summary>
    /// The unique identifier (GUID) of the vacancy.
    /// </summary>
    public Guid? VacancyId { get; set; }

    /// <summary>
    /// The unique identifier (GUID) of the employer.
    /// </summary>
    public Guid? EmployerId { get; set; }

    /// <summary>
    /// The unique identifier (GUID) of the student.
    /// </summary>
    public Guid? StudentId { get; set; }

    /// <summary>
    /// The date and time the favorite was added.
    /// </summary>
    public DateTime AddedAt { get; set; }

    public VacancySummaryDto? Vacancy { get; set; }

    public EmployerSummaryDto? Employer { get; set; }

    public StudentSummaryDto? Student { get; set; }
}

public class VacancySummaryDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
}

public class EmployerSummaryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class StudentSummaryDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}