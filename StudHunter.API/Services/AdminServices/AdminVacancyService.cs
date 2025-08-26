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
public class AdminVacancyService(StudHunterDbContext context) : BaseVacancyService(context)
{
    /// <summary>
    /// Retrieves all vacancies.
    /// </summary>
    /// <returns>A tuple containing a list of vacancies, an optional status code, and an optional error message.</returns>
    public async Task<(List<AdminVacancyDto>? Entities, int? StatusCode, string? ErrorMessage)> GetAllVacanciesAsync()
    {
        var vacancies = await _context.Vacancies
        .Select(v => MapToVacancyDto<AdminVacancyDto>(v))
        .ToListAsync();

        return (vacancies, null, null);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<(AdminVacancyDto? Entity, int? StatusCode, string? ErrorMessage)> GetVacancyAsync(Guid id)
    {
        var vacancy = await _context.Vacancies
            .FirstOrDefaultAsync(v => v.Id == id);

        if (vacancy == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Vacancy)));

        return (MapToVacancyDto<AdminVacancyDto>(vacancy), null, null);
    }

    /// <summary>
    /// Updates an existing vacancy.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the vacancy.</param>
    /// <param name="dto">The data transfer object containing updated vacancy details.</param>
    /// <returns>A tuple indicating whether the update was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateVacancyAsync(Guid id, AdminUpdateVacancyDto dto)
    {
        var vacancy = await _context.Vacancies
            .FirstOrDefaultAsync(v => v.Id == id);

        if (vacancy == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Vacancy)));

        if (dto.Title != null)
            vacancy.Title = dto.Title;
        if (dto.Description != null)
            vacancy.Description = dto.Description;
        if (dto.Salary.HasValue)
            vacancy.Salary = dto.Salary;
        if (dto.Type != null)
            vacancy.Type = Enum.Parse<Vacancy.VacancyType>(dto.Type);
        if (dto.IsDeleted.HasValue)
        {
            vacancy.IsDeleted = dto.IsDeleted.Value;
            vacancy.DeletedAt = dto.IsDeleted.Value ? DateTime.UtcNow : null;
        }
        vacancy.UpdatedAt = DateTime.UtcNow;

        return await SaveChangesAsync<Vacancy>();
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

    /// <summary>
    /// Deletes a vacancyCourse.
    /// </summary>
    /// <param name="vacancyId">The unique identifier (GUID) of the vacancyCourse.</param>
    /// <param name="courseId">The unique identifier (GUID) of the Course.</param>
    /// <returns>A tuple indicating whether the deletion was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteVacancyCourseAsync(Guid vacancyId, Guid courseId)
    {
        var entity = await _context.Set<VacancyCourse>()
            .FirstOrDefaultAsync(vc => vc.VacancyId == vacancyId && vc.CourseId == courseId);

        if (entity == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(VacancyCourse)));

        _context.Set<VacancyCourse>().Remove(entity);

        return await SaveChangesAsync<VacancyCourse>();
    }
}
