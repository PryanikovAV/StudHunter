namespace StudHunter.API.ModelsDto.Favorite;

/// <summary>
/// Data transfer object for creating a favorite.
/// </summary>
public class CreateFavoriteDto
{
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
}