using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.AuthDto;
using StudHunter.API.ModelsDto.EmployerDto;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

/// <summary>
/// Service for managing employers.
/// </summary>
public class EmployerService(StudHunterDbContext context, IPasswordHasher passwordHasher, AuthService authService) : BaseEmployerService(context)
{
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IAuthService _authService = authService;

    /// <summary>
    /// Retrieves an employer by their ID.
    /// </summary>
    /// <param name="employerId">The unique identifier (GUID) of the employer.</param>
    /// <param name="authUserId"></param>
    /// <returns>A tuple containing the employer's details, an optional status code, and an optional error message.</returns>
    public async Task<(EmployerDto? Entity, int? StatusCode, string? ErrorMessage)> GetEmployerAsync(Guid employerId, Guid authUserId)
    {
        var employer = await _context.Employers
            .Include(s => s.Achievements).ThenInclude(ua => ua.AchievementTemplate)
            .Include(e => e.Vacancies)
            .FirstOrDefaultAsync(e => e.Id == employerId);

        if (employer == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Employer)));

        if (employer.Id != authUserId)
        {
            var isStudent = await _context.Students.AnyAsync(s => s.Id == authUserId);
            if (!isStudent || !employer.AccreditationStatus)
                return (null, StatusCodes.Status403Forbidden, ErrorMessages.RestrictOwnProfileAction("get", nameof(Employer)));
        }

        return (MapToEmployerDto<EmployerDto>(employer), null, null);
    }

    /// <summary>
    /// Retrieves an employer by their email.
    /// </summary>
    /// <param name="email">The email of the employer.</param>
    /// <param name="authUserId"></param>
    /// <returns>A tuple containing the employer's details, an optional status code, and an optional error message.</returns>
    public async Task<(EmployerDto? Entity, int? StatusCode, string? ErrorMessage)> GetEmployerAsync(string email, Guid authUserId)
    {
        var employer = await _context.Employers
            .Include(s => s.Achievements).ThenInclude(ua => ua.AchievementTemplate)
            .Include(e => e.Vacancies)
            .FirstOrDefaultAsync(e => e.Email == email);

        if (employer == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Employer)));

        if (employer.Id != authUserId)
        {
            var isStudent = await _context.Students.AnyAsync(s => s.Id == authUserId && !s.IsDeleted);
            if (!isStudent || !employer.AccreditationStatus)
                return (null, StatusCodes.Status403Forbidden, ErrorMessages.RestrictOwnProfileAction("get", nameof(Employer)));
        }

        return (MapToEmployerDto<EmployerDto>(employer), null, null);
    }

    /// <summary>
    /// Retrieves employers by their specialization, including deleted ones.
    /// </summary>
    /// <param name="specialization">Employer specialization.</param>
    /// <param name="authUserId"></param>
    /// <returns>A tuple containing the employer's details, an optional status code, and an optional error message.</returns>
    public async Task<(List<EmployerDto>? Entities, int? StatusCode, string? ErrorMessage)> GetEmployersBySpecializationAsync(string? specialization, Guid authUserId)
    {
        var isUser = await _context.Users.FindAsync(authUserId);
        if (isUser == null)
            return (null, StatusCodes.Status401Unauthorized, $"Invalid {nameof(User.Id)}.");
        if (isUser is Employer isEmployer && !isEmployer.AccreditationStatus)
            return (null, StatusCodes.Status403Forbidden, $"{nameof(Employer)} is not accredited.");

        var query = _context.Employers
            .Include(e => e.Achievements).ThenInclude(ua => ua.AchievementTemplate)
            .Include(e => e.Vacancies)
            .OrderByDescending(e => e.Name)
            .Where(e => e.AccreditationStatus);

        if (!string.IsNullOrEmpty(specialization))
            query = query.Where(e => e.Specialization != null && e.Specialization.Contains(specialization));

        var employers = await query
            .Select(e => MapToEmployerDto<EmployerDto>(e))
            .ToListAsync();

        return (employers, null, null);
    }

    /// <summary>
    /// Registers a new employer with minimal details (email, password, name).
    /// </summary>
    /// <param name="dto">The data transfer object containing employer registration details.</param>
    /// <returns>A tuple containing the employer DTO, an optional status code, and an optional error message.</returns>
    public async Task<(EmployerDto? Entity, int? StatusCode, string? ErrorMessage)> CreateEmployerAsync(RegisterEmployerDto dto)
    {
        var userExists = await _context.Users
            .IgnoreQueryFilters()
            .AnyAsync(u => u.Email == dto.Email);
        if (userExists)
            return (null, StatusCodes.Status409Conflict, ErrorMessages.EntityAlreadyExists(nameof(User), nameof(User.Email)));

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

        return (MapToEmployerDto<EmployerDto>(employer), null, null);
    }

    /// <summary>
    /// Updates an employer's profile.
    /// </summary>
    /// <param name="employerId">The unique identifier (GUID) of the employer.</param>
    /// <param name="authUserId"></param>
    /// <param name="dto">The data transfer object containing updated employer details.</param>
    /// <returns>A tuple indicating whether the update was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateEmployerAsync(Guid authUserId, Guid employerId, UpdateEmployerDto dto)
    {
        var employer = await _context.Employers
            .Include(s => s.Achievements).ThenInclude(ua => ua.AchievementTemplate)
            .Include(e => e.Vacancies)
            .FirstOrDefaultAsync(e => e.Id == employerId);

        if (employer == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Employer)));
        if (employer.Id != authUserId)
            return (false, StatusCodes.Status403Forbidden, ErrorMessages.RestrictOwnProfileAction("update", nameof(Employer)));
        if (dto.Email != null && await _context.Employers.AnyAsync(s => s.Email == dto.Email && s.Id != employerId))
            return (false, StatusCodes.Status409Conflict, ErrorMessages.EntityAlreadyExists(nameof(Employer), nameof(Employer.Email)));
        if (dto.ContactPhone != null && await _context.Employers.AnyAsync(e => e.ContactPhone == dto.ContactPhone && e.Id != employerId))
            return (false, StatusCodes.Status409Conflict, ErrorMessages.EntityAlreadyExists(nameof(Employer), nameof(Employer.ContactPhone)));

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
    /// <param name="employerId">The unique identifier (GUID) of the employer.</param>
    /// <param name="authUserId"></param>
    /// <returns>A tuple indicating whether the deletion was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteEmployerAsync(Guid authUserId, Guid employerId)
    {
        var employer = await _context.Employers
            .Include(s => s.Achievements).ThenInclude(ua => ua.AchievementTemplate)
            .Include(e => e.Vacancies)
            .FirstOrDefaultAsync(e => e.Id == employerId);

        if (employer == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Employer)));
        if (employer.Id != authUserId)
            return (false, StatusCodes.Status403Forbidden, ErrorMessages.RestrictOwnProfileAction("delete", nameof(Employer)));

        employer.IsDeleted = true;
        employer.DeletedAt = DateTime.UtcNow;

        if (employer.Vacancies != null)
        {
            var vacancies = await _context.Vacancies
                .Where(v => v.EmployerId == employer.Id)
                .ToListAsync();

            foreach (var vacancy in vacancies)
            {
                vacancy.IsDeleted = true;
                vacancy.DeletedAt = DateTime.UtcNow;
            }
        }

        var invitations = await _context.Invitations
            .Where(i => i.SenderId == employer.Id && i.Status != Invitation.InvitationStatus.Rejected || i.ReceiverId == employer.Id && i.Status != Invitation.InvitationStatus.Rejected)
            .ToListAsync();

        foreach (var invitation in invitations)
        {
            invitation.Status = Invitation.InvitationStatus.Rejected;
            invitation.UpdatedAt = DateTime.UtcNow;
        }

        return await SaveChangesAsync<Employer>();
    }
}
