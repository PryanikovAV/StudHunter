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
public class AdminEmployerService(StudHunterDbContext context) : BaseEmployerService(context)
{
    /// <summary>
    /// Retrieves all employers, including deleted ones.
    /// </summary>
    /// <returns>A tuple containing a list of all employers, an optional status code, and an optional error message.</returns>
    public async Task<(List<AdminEmployerDto>? Entities, int? StatusCode, string? ErrorMessage)> GetAllEmployersAsync()
    {
        var employers = await _context.Employers
        .Include(e => e.Vacancies)
        .ToListAsync();

        var dtos = employers.Select(MapToEmployerDto<AdminEmployerDto>).ToList();
        return (dtos, null, null);
    }

    /// <summary>
    /// Updates an employer's profile by an administrator.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the employer.</param>
    /// <param name="dto">The data transfer object containing updated employer details.</param>
    /// <returns>A tuple indicating whether the update was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateEmployerAsync(Guid id, AdminUpdateEmployerDto dto)
    {
        var employer = await _context.Employers.FindAsync(id);

        if (employer == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Employer)));

        if (dto.Email != null && await _context.Employers.AnyAsync(e => e.Email == dto.Email && e.Id != id))
            return (false, StatusCodes.Status409Conflict, ErrorMessages.EntityAlreadyExists(nameof(Employer), "email"));

        if (dto.Name != null && string.IsNullOrWhiteSpace(dto.Name))
            return (false, StatusCodes.Status400BadRequest, "Name is required and cannot be empty.");

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
        if (dto.AccreditationStatus.HasValue)
            employer.AccreditationStatus = dto.AccreditationStatus.Value;
        if (dto.IsDeleted.HasValue)
            employer.IsDeleted = dto.IsDeleted.Value;

        return await SaveChangesAsync<Employer>();
    }

    /// <summary>
    /// Deletes an employer (soft or hard delete).
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the employer.</param>
    /// <param name="hardDelete">A boolean indicating whether to perform a hard delete (default is false).</param>
    /// <returns>A tuple indicating whether the deletion was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteEmployerAsync(Guid id, bool hardDelete = false)
    {
        if (hardDelete)
            return await DeleteEntityAsync<Employer>(id, hardDelete);

        var employer = await _context.Employers
        .Include(e => e.Vacancies)
        .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);

        if (employer == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Employer)));

        employer.IsDeleted = true;
        foreach (var vacancy in employer.Vacancies)
            vacancy.IsDeleted = true;

        return await SaveChangesAsync<Employer>();
    }
}
