using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.Auth;
using StudHunter.API.ModelsDto.Employer;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;
using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.Services;

/// <summary>
/// Service for managing employers.
/// </summary>
public class EmployerService(StudHunterDbContext context, IPasswordHasher passwordHasher, AuthService authService) : BaseEmployerService(context)
{
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IAuthService _authService = authService;

    /// <summary>
    /// Registers a new employer with minimal details (email, password, name).
    /// </summary>
    /// <param name="dto">The data transfer object containing employer registration details.</param>
    /// <returns>A tuple containing the employer DTO, an optional status code, and an optional error message.</returns>
    public async Task<(EmployerDto? Entity, int? StatusCode, string? ErrorMessage)> CreateEmployerAsync(RegisterEmployerDto dto)
    {
        if (!new EmailAddressAttribute().IsValid(dto.Email))
            return (null, StatusCodes.Status400BadRequest, ErrorMessages.InvalidData("email format"));

        if (await _context.Users.AnyAsync(u => u.Email == dto.Email && !u.IsDeleted))
            return (null, StatusCodes.Status409Conflict, ErrorMessages.EntityAlreadyExists(nameof(Employer), "email"));

        if (string.IsNullOrEmpty(dto.Name))
            return (null, StatusCodes.Status400BadRequest, ErrorMessages.InvalidData("name"));

        var employer = new Employer
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            ContactPhone = dto.ContactPhone,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false,
            AccreditationStatus = false,
        };

        employer.UpdateEmail(dto.Email);
        employer.UpdatePassword(_passwordHasher.HashPassword(dto.Password));

        _context.Employers.Add(employer);

        var (success, statusCode, errorMessage) = await SaveChangesAsync<Employer>();

        if (!success)
            return (null, statusCode, errorMessage);

        var employerEntity = await _context.Employers
            .Include(e => e.Vacancies)
            .Include(e => e.Achievements).ThenInclude(a => a.AchievementTemplate)
            .FirstOrDefaultAsync(e => e.Id == employer.Id && !e.IsDeleted);

        if (employerEntity == null)
            return (null, StatusCodes.Status500InternalServerError, ErrorMessages.FailedToRetrieve(nameof(Employer)));

        var token = _authService.GenerateJwtToken(employer.Id, nameof(Employer));
        return (MapToEmployerDto<EmployerDto>(employerEntity), null, null);
    }

    /// <summary>
    /// Updates an employer's profile.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the employer.</param>
    /// <param name="dto">The data transfer object containing updated employer details.</param>
    /// <returns>A tuple indicating whether the update was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateEmployerAsync(Guid id, UpdateEmployerDto dto)
    {
        var employer = await _context.Employers.FindAsync(id);
        if (employer == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Employer)));

        if (dto.Email != null && await _context.Employers.AnyAsync(s => s.Email == dto.Email && s.Id != id))
            return (false, StatusCodes.Status409Conflict, ErrorMessages.EntityAlreadyExists(nameof(Employer), "email"));

        if (dto.Name != null && string.IsNullOrWhiteSpace(dto.Name))
            return (false, StatusCodes.Status400BadRequest, ErrorMessages.InvalidData("name"));

        if (dto.Email != null)
            employer.UpdateEmail(dto.Email);
        if (dto.Password != null)
            employer.UpdatePassword(_passwordHasher.HashPassword(dto.Password));

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

        return await SaveChangesAsync<Employer>();
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
