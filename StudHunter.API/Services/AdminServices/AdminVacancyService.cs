using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.Vacancy;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.AdminServices;

/// <summary>
/// Service for managing vacancies with administrative privileges.
/// </summary>
public class AdminVacancyService(StudHunterDbContext context, UserAchievementService userAchievementService) : BaseVacancyService(context, userAchievementService)
{
    /// <summary>
    /// Updates an existing vacancy.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the vacancy.</param>
    /// <param name="dto">The data transfer object containing updated vacancy details.</param>
    /// <returns>A tuple indicating whether the update was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateVacancyAsync(Guid id, AdminUpdateVacancyDto dto)
    {
        var vacancy = await _context.Vacancies.FirstOrDefaultAsync(v => v.Id == id);

        #region Serializers
        if (vacancy == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.NotFound(nameof(Vacancy)));
        #endregion

        if (dto.Title != null)
            vacancy.Title = dto.Title;
        if (dto.Description != null)
            vacancy.Description = dto.Description;
        if (dto.Salary.HasValue)
            vacancy.Salary = dto.Salary;
        if (dto.Type != null)
            vacancy.Type = Enum.Parse<Vacancy.VacancyType>(dto.Type);
        if (dto.IsDeleted.HasValue)
            vacancy.IsDeleted = dto.IsDeleted.Value;
        vacancy.UpdatedAt = DateTime.UtcNow;

        var (success, statusCode, errorMessage) = await SaveChangesAsync<Vacancy>();

        return (success, statusCode, errorMessage);
    }

    /// <summary>
    /// Deletes a vacancy (hard or soft delete).
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the vacancy.</param>
    /// <param name="hardDelete">A boolean indicating whether to perform a hard delete (true) or soft delete (false).</param>
    /// <returns>A tuple indicating whether the deletion was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteVacancyAsync(Guid id, bool hardDelete = false)
    {
        return await DeleteEntityAsync<Vacancy>(id, hardDelete);
    }
}
