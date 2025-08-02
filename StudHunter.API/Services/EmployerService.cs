using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.Employer;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

/// <summary>
/// Service for managing employers.
/// </summary>
public class EmployerService(StudHunterDbContext context, UserAchievementService userAchievementService, IPasswordHasher passwordHasher)
: BaseEmployerService(context, userAchievementService)
{
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    /// <summary>
    /// Creates a new employer.
    /// </summary>
    /// <param name="dto">The data transfer object containing employer details.</param>
    /// <returns>A tuple containing the created employer, an optional status code, and an optional error message.</returns>
    public async Task<(EmployerDto? Entity, int? StatusCode, string? ErrorMessage)> CreateEmployerAsync(CreateEmployerDto dto)
    {
        #region Serializers
        if (await _context.Employers.AnyAsync(e => e.Email == dto.Email))
            return (null, StatusCodes.Status409Conflict, ErrorMessages.AlreadyExists(nameof(Employer), "email"));
        #endregion

        var employer = new Employer
        {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            PasswordHash = _passwordHasher.HashPassword(dto.Password),
            ContactEmail = dto.ContactEmail,
            ContactPhone = dto.ContactPhone,
            CreatedAt = DateTime.UtcNow,
            AccreditationStatus = false,
            Name = dto.Name,
            Description = dto.Description,
            Website = dto.Website,
            Specialization = dto.Specialization
        };

        _context.Employers.Add(employer);

        var (success, statusCode, errorMessage) = await SaveChangesAsync<Employer>();

        if (!success)
            return (null, statusCode, errorMessage);

        return (new EmployerDto
        {
            Id = employer.Id,
            Email = employer.Email,
            ContactEmail = employer.ContactEmail,
            ContactPhone = employer.ContactPhone,
            CreatedAt = employer.CreatedAt,
            AccreditationStatus = employer.AccreditationStatus,
            Name = employer.Name,
            Description = employer.Description,
            Website = employer.Website,
            Specialization = employer.Specialization
        }, null, null);
    }

    /// <summary>
    /// Updates an existing employer.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the employer.</param>
    /// <param name="dto">The data transfer object containing updated employer details.</param>
    /// <returns>A tuple indicating whether the update was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateEmployerAsync(Guid id, UpdateEmployerDto dto)
    {
        var employer = await _context.Employers.FindAsync(id);

        #region Serializers
        if (employer == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.NotFound(nameof(Employer)));

        if (dto.Email != null)
        {
            if (await _context.Employers.AnyAsync(s => s.Email == dto.Email && s.Id != id))
                return (false, StatusCodes.Status409Conflict, ErrorMessages.AlreadyExists(nameof(Employer), "email"));
        }
        #endregion

        if (dto.Email != null)
            employer.Email = dto.Email;
        if (dto.Password != null)
            employer.PasswordHash = _passwordHasher.HashPassword(dto.Password);
        if (dto.ContactPhone != null)
            employer.ContactPhone = dto.ContactPhone;
        if (dto.ContactEmail != null)
            employer.ContactEmail = dto.ContactEmail;
        if (dto.Name != null)
            employer.Name = dto.Name;
        if (dto.Description != null)
            employer.Description = dto.Description;
        if (dto.Website != null)
            employer.Website = dto.Website;
        if (dto.Specialization != null)
            employer.Specialization = dto.Specialization;

        var (success, statusCode, errorMessage) = await SaveChangesAsync<Employer>();

        return (success, statusCode, errorMessage);
    }

    /// <summary>
    /// Deletes an employer (soft delete).
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the employer.</param>
    /// <returns>A tuple indicating whether the deletion was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteEmployerAsync(Guid id)
    {
        return await DeleteEntityAsync<Employer>(id, hardDelete: false);
    }
}
