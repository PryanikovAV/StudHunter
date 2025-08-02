using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.Employer;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.AdminServices;

/// <summary>
/// Service for managing employers with administrative privileges.
/// </summary>
public class AdminEmployerService(StudHunterDbContext context, UserAchievementService userAchievementService) : BaseEmployerService(context, userAchievementService)
{
    /// <summary>
    /// Retrieves all employers.
    /// </summary>
    /// <returns>A tuple containing a list of all employers, an optional status code, and an optional error message.</returns>
    public async Task<(List<AdminEmployerDto>? Entities, int? StatusCode, string? ErrorMessage)> GetAllEmployersAsync()
    {
        var employers = await _context.Employers
        .Include(e => e.Vacancies)
        .Select(e => new AdminEmployerDto
        {
            Id = e.Id,
            Email = e.Email,
            ContactEmail = e.ContactEmail,
            ContactPhone = e.ContactPhone,
            CreatedAt = e.CreatedAt,
            IsDeleted = e.IsDeleted,
            AccreditationStatus = e.AccreditationStatus,
            Name = e.Name,
            Description = e.Description,
            Website = e.Website,
            Specialization = e.Specialization,
            VacancyIds = e.Vacancies.Select(v => v.Id).ToList()
        }).ToListAsync();

        return (employers, null, null);
    }

    /// <summary>
    /// Updates an existing employer.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the employer.</param>
    /// <param name="dto">The data transfer object containing updated employer details.</param>
    /// <returns>A tuple indicating whether the update was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateEmployerAsync(Guid id, AdminUpdateEmployerDto dto)
    {
        var employer = await _context.Employers.FindAsync(id);

        #region Serializers
        if (employer == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.NotFound(nameof(Employer)));

        if (dto.Email != null)
        {
            if (await _context.Employers.AnyAsync(e => e.Email == dto.Email && e.Id != id))
                return (false, StatusCodes.Status409Conflict, ErrorMessages.AlreadyExists(nameof(Employer), "email"));
        }
        #endregion

        if (dto.ContactEmail != null)
            employer.ContactEmail = dto.ContactEmail;
        if (dto.ContactPhone != null)
            employer.ContactPhone = dto.ContactPhone;
        if (dto.Name != null)
            employer.Name = dto.Name;
        if (dto.Description != null)
            employer.Description = dto.Description;
        if (dto.Website != null)
            employer.Website = dto.Website;
        if (dto.Specialization != null)
            employer.Specialization = dto.Specialization;
        if (dto.IsDeleted.HasValue)
            employer.IsDeleted = dto.IsDeleted.Value;
        if (dto.AccreditationStatus.HasValue)
            employer.AccreditationStatus = dto.AccreditationStatus.Value;

        var (success, statusCode, errorMessage) = await SaveChangesAsync<Employer>();

        return (success, statusCode, errorMessage);
    }

    /// <summary>
    /// Deletes an employer (hard or soft delete).
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the employer.</param>
    /// <param name="hardDelete">A boolean indicating whether to perform a hard delete (default is false).</param>
    /// <returns>A tuple indicating whether the deletion was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteEmployerAsync(Guid id, bool hardDelete = false)
    {
        return await DeleteEntityAsync<Employer>(id, hardDelete);
    }
}
