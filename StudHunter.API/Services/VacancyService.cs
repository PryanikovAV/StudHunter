using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.BaseModelsDto;
using StudHunter.API.ModelsDto.VacancyDto;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

/// <summary>
/// Service for managing vacancies.
/// </summary>
public class VacancyService(StudHunterDbContext context) : BaseVacancyService(context)
{
    /// <summary>
    /// Retrieves all non-deleted vacancies.
    /// </summary>
    /// <returns>A tuple containing a list of vacancies, an optional status code, and an optional error message.</returns>
    public async Task<(List<VacancyDto>? Entities, int? StatusCode, string? ErrorMessage)> GetAllVacanciesAsync()
    {
        var vacancies = await _context.Vacancies
            .Select(v => MapToVacancyDto(v))
            .OrderByDescending(v => v.UpdatedAt)
            .ToListAsync();
        return (vacancies, null, null);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="employerId"></param>
    /// <param name="authUserId"></param>
    /// <returns></returns>
    public async Task<(List<VacancyDto>? Entities, int? StatusCode, string? ErrorMessage)> GetVacanciesByEmployerAsync(Guid authUserId, Guid employerId)
    {
        if (employerId != authUserId && await _context.Employers.AnyAsync(e => e.Id == authUserId))
            return (null, StatusCodes.Status403Forbidden, $"{nameof(Employer)} cannot view the {nameof(Vacancy)} of other {nameof(Employer)}.");
        if (!await _context.Employers.AnyAsync(e => e.Id == employerId))
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Employer)));

        var vacancies = await _context.Vacancies
            .Where(v => v.EmployerId == employerId)
            .Select(v => MapToVacancyDto(v))
            .OrderByDescending(v => v.UpdatedAt)
            .ToListAsync();

        return (vacancies, null, null);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="vacancyId"></param>
    /// <param name="authUserId"></param>
    /// <returns>A tuple containing the vacancy, an optional status code, and an optional error message.</returns>
    public async Task<(VacancyDto? Entity, int? StatusCode, string? ErrorMessage)> GetVacancyAsync(Guid authUserId, Guid vacancyId)
    {
        var vacancy = await _context.Vacancies.FindAsync(vacancyId);
        if (vacancy == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Vacancy)));
        if (vacancy.EmployerId != authUserId && await _context.Employers.AnyAsync(e => e.Id == authUserId))
            return (null, StatusCodes.Status403Forbidden, $"{nameof(Employer)} cannot view the {nameof(Vacancy)} of other {nameof(Employer)}.");
        return (MapToVacancyDto(vacancy), null, null);
    }

    /// <summary>
    /// Creates a new vacancy for an employer.
    /// </summary>
    /// <param name="authUserId">The unique identifier (GUID) of the employer.</param>
    /// <param name="dto">The data transfer object containing vacancy details.</param>
    /// <returns>A tuple containing the created vacancy, an optional status code, and an optional error message.</returns>
    public async Task<(VacancyDto? Entity, int? StatusCode, string? ErrorMessage)> CreateVacancyAsync(Guid authUserId, CreateVacancyDto dto)
    {
        var employer = await _context.Employers.FindAsync(authUserId);
        if (employer == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Employer)));
        if (!employer.AccreditationStatus)
            return (null, StatusCodes.Status403Forbidden, $"{nameof(Employer)} is not accredited.");

        var vacancy = new Vacancy
        {
            Id = Guid.NewGuid(),
            EmployerId = authUserId,
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
        return (MapToVacancyDto(vacancy), null, null);
    }

    /// <summary>
    /// Updates an existing vacancy.
    /// </summary>
    /// <param name="vacancyId">The unique identifier (GUID) of the vacancy.</param>
    /// <param name="authUserId"></param>
    /// <param name="dto">The data transfer object containing updated vacancy details.</param>
    /// <returns>A tuple indicating whether the update was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateVacancyAsync(Guid authUserId, Guid vacancyId, UpdateVacancyDto dto)
    {
        var vacancy = await _context.Vacancies.FindAsync(vacancyId);
        if (vacancy == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Vacancy)));
        if (vacancy.EmployerId != authUserId)
            return (false, StatusCodes.Status403Forbidden, ErrorMessages.RestrictOwnProfileAction("update", nameof(Vacancy)));

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
    /// <param name="authUserId"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateVacancyStatusAsync(Guid authUserId, Guid vacancyId, UpdateStatusDto dto)
    {
        var vacancy = await _context.Vacancies
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(v => v.Id == vacancyId);
        if (vacancy == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Vacancy)));
        if (vacancy.EmployerId != authUserId)
            return (false, StatusCodes.Status403Forbidden, ErrorMessages.RestrictOwnProfileAction("update status", nameof(Vacancy)));

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
}
