using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.BaseModelsDto;
using StudHunter.API.ModelsDto.VacancyDto;
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
    public async Task<(List<VacancyDto>? Entities, int? StatusCode, string? ErrorMessage)> GetAllVacanciesAsync()
    {
        var vacancies = await _context.Vacancies
            .IgnoreQueryFilters()
            .Select(v => MapToVacancyDto(v))
            .ToListAsync();
        return (vacancies, null, null);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="vacancyId"></param>
    /// <returns></returns>
    public async Task<(VacancyDto? Entity, int? StatusCode, string? ErrorMessage)> GetVacancyAsync(Guid vacancyId)
    {
        var vacancy = await _context.Vacancies
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(v => v.Id == vacancyId);
        if (vacancy == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Vacancy)));
        return (MapToVacancyDto(vacancy), null, null);
    }

    /// <summary>
    /// Updates an existing vacancy.
    /// </summary>
    /// <param name="vacancyId">The unique identifier (GUID) of the vacancy.</param>
    /// <param name="dto">The data transfer object containing updated vacancy details.</param>
    /// <returns>A tuple indicating whether the update was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateVacancyAsync(Guid vacancyId, UpdateVacancyDto dto)
    {
        var vacancy = await _context.Vacancies
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(v => v.Id == vacancyId);
        if (vacancy == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Vacancy)));
        if (vacancy.IsDeleted)
            return (false, StatusCodes.Status410Gone, ErrorMessages.EntityAlreadyDeleted(nameof(Vacancy), nameof(UpdateVacancyStatusAsync)));

        if (dto.Title != null)
            vacancy.Title = dto.Title;
        if (dto.Description != null)
            vacancy.Description = dto.Description;
        if (dto.Salary.HasValue)
            vacancy.Salary = dto.Salary;
        if (dto.Type != null)
            vacancy.Type = Enum.Parse<Vacancy.VacancyType>(dto.Type);
        vacancy.UpdatedAt = DateTime.UtcNow;

        return await SaveChangesAsync<Vacancy>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="vacancyId"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateVacancyStatusAsync(Guid vacancyId, UpdateStatusDto dto)
    {
        var vacancy = await _context.Vacancies
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(v => v.Id == vacancyId);
        if (vacancy == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Vacancy)));

        vacancy.IsDeleted = dto.IsDeleted;
        vacancy.DeletedAt = dto.IsDeleted ? DateTime.UtcNow : null;

        if (dto.IsDeleted)
        {
            var invitations = await _context.Invitations
                .Where(i => i.VacancyId == vacancyId && i.Status != Invitation.InvitationStatus.Rejected)
                .ToListAsync();
            foreach (var invitation in invitations)
            {
                invitation.Status = Invitation.InvitationStatus.Rejected;
                invitation.UpdatedAt = DateTime.UtcNow;
            }
        }

        return await SaveChangesAsync<Vacancy>();
    }

    /// <summary>
    /// Deletes a resume.
    /// </summary>
    /// <param name="vacancyId">The unique identifier (GUID) of the resume.</param>
    /// <returns>A tuple indicating whether the deletion was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteVacancyAsync(Guid vacancyId)
    {
        var vacancy = await _context.Vacancies
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(v => v.Id == vacancyId);
        if (vacancy == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Vacancy)));

        _context.Vacancies.Remove(vacancy);
        return await SaveChangesAsync<Vacancy>();
    }
}
