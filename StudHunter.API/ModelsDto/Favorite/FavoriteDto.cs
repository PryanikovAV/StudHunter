using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Favorite;

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
    [Required]
    public Guid UserId { get; set; }

    /// <summary>
    /// The unique identifier (GUID) of the vacancy.
    /// </summary>
    public Guid? VacancyId { get; set; }

    /// <summary>
    /// The unique identifier (GUID) of the resume.
    /// </summary>
    public Guid? ResumeId { get; set; }

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
}
