using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.FavoriteDto;

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

    [CustomValidation(typeof(CreateFavoriteDto), nameof(ValidateFavoriteTarget))]
    public static ValidationResult ValidateFavoriteTarget(CreateFavoriteDto dto, ValidationContext context)
    {
        var filledFields = new[] { dto.VacancyId, dto.EmployerId, dto.StudentId }.Count(id => id != null);
        if (filledFields == 0)
            return new ValidationResult("At least one of VacancyId, EmployerId or StudentId must be provided.");
        if (filledFields > 1)
            return new ValidationResult("Only one of VacancyId, EmployerId or StudentId can be provided.");
        return ValidationResult.Success!;
    }
}
