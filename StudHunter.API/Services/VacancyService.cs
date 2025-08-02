using StudHunter.API.Common;
using StudHunter.API.ModelsDto.Vacancy;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

/// <summary>
/// Service for managing vacancies.
/// </summary>
public class VacancyService(StudHunterDbContext context, UserAchievementService userAchievementService) : BaseVacancyService(context, userAchievementService)
{
    /// <summary>
    /// Creates a new vacancy for an employer.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the employer.</param>
    /// <param name="dto">The data transfer object containing vacancy details.</param>
    /// <returns>A tuple containing the created vacancy, an optional status code, and an optional error message.</returns>
    public async Task<(VacancyDto? Entity, int? StatusCode, string? ErrorMessage)> CreateVacancyAsync(Guid id, CreateVacancyDto dto)
    {
        #region Serializers
        var employer = await _context.Employers.FindAsync(id);
        if (employer == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.NotFound(nameof(Employer)));

        if (!employer.AccreditationStatus)
            return (null, StatusCodes.Status403Forbidden, $"{nameof(Employer)} is not accredited.");
        #endregion

        var vacancy = new Vacancy
        {
            Id = Guid.NewGuid(),
            EmployerId = id,
            Title = dto.Title,
            Description = dto.Description,
            Salary = dto.Salary,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Type = Enum.Parse<Vacancy.VacancyType>(dto.Type),
            IsDeleted = false
        };

        _context.Vacancies.Add(vacancy);

        var (success, statusCode, errorMessage) = await SaveChangesAsync<Vacancy>();

        if (!success)
            return (null, statusCode, errorMessage);

        return (new VacancyDto
        {
            Id = vacancy.Id,
            EmployerId = vacancy.EmployerId,
            Title = vacancy.Title,
            Description = vacancy.Description,
            Salary = vacancy.Salary,
            CreatedAt = vacancy.CreatedAt,
            UpdatedAt = vacancy.UpdatedAt,
            Type = vacancy.Type.ToString()
        }, null, null);
    }

    /// <summary>
    /// Updates an existing vacancy.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the vacancy.</param>
    /// <param name="dto">The data transfer object containing updated vacancy details.</param>
    /// <returns>A tuple indicating whether the update was successful, an optional status code, and an optional error message.</returns>
    public virtual async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateVacancyAsync(Guid id, UpdateVacancyDto dto)
    {
        var vacancy = await _context.Vacancies.FindAsync(id);

        #region Serializers
        if (vacancy == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.NotFound(nameof(Vacancy)));
        if (vacancy.IsDeleted)
            return (false, StatusCodes.Status410Gone, ErrorMessages.AlreadyDeleted(nameof(Vacancy)));
        #endregion

        if (dto.Title != null)
            vacancy.Title = dto.Title;
        if (dto.Description != null)
            vacancy.Description = dto.Description;
        if (dto.Salary.HasValue)
            vacancy.Salary = dto.Salary;
        if (dto.Type != null)
            vacancy.Type = Enum.Parse<Vacancy.VacancyType>(dto.Type);
        vacancy.UpdatedAt = DateTime.UtcNow;

        var (success, statusCode, errorMessage) = await SaveChangesAsync<Vacancy>();

        return (success, statusCode, errorMessage);
    }

    /// <summary>
    /// Deletes a vacancy (soft delete).
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the vacancy.</param>
    /// <returns>A tuple indicating whether the deletion was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteVacancyAsync(Guid id)
    {
        return await DeleteEntityAsync<Vacancy>(id, hardDelete: false);
    }
}
