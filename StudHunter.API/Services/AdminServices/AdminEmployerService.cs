using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.EmployerDto;
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
            .IgnoreQueryFilters()
            .Include(e => e.Vacancies)
            .Select(e => MapToEmployerDto<AdminEmployerDto>(e))
            .ToListAsync();

        return (employers, null, null);
    }

    /// <summary>
    /// Retrieves an employer by their ID.
    /// </summary>
    /// <param name="employerId">The unique identifier (GUID) of the employer.</param>
    /// <returns>A tuple containing the employer's details, an optional status code, and an optional error message.</returns>
    public async Task<(AdminEmployerDto? Entity, int? StatusCode, string? ErrorMessage)> GetEmployerAsync(Guid employerId)
    {
        var employer = await _context.Employers
            .IgnoreQueryFilters()
            .Include(s => s.Achievements).ThenInclude(ua => ua.AchievementTemplate)
            .Include(e => e.Vacancies)
            .FirstOrDefaultAsync(e => e.Id == employerId);

        if (employer == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Employer)));

        return (MapToEmployerDto<AdminEmployerDto>(employer), null, null);
    }

    /// <summary>
    /// Retrieves an employer by their email.
    /// </summary>
    /// <param name="email">The email of the employer.</param>
    /// <returns>A tuple containing the employer's details, an optional status code, and an optional error message.</returns>
    public async Task<(AdminEmployerDto? Entity, int? StatusCode, string? ErrorMessage)> GetEmployerAsync(string email)
    {
        var employer = await _context.Employers
            .IgnoreQueryFilters()
            .Include(s => s.Achievements).ThenInclude(ua => ua.AchievementTemplate)
            .Include(e => e.Vacancies)
            .FirstOrDefaultAsync(e => e.Email == email);

        if (employer == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Employer)));

        return (MapToEmployerDto<AdminEmployerDto>(employer), null, null);
    }

    /// <summary>
    /// Retrieves employers by their specialization, including deleted ones.
    /// </summary>
    /// <param name="specialization">Employer specialization.</param>
    /// <returns>A tuple containing the employer's details, an optional status code, and an optional error message.</returns>
    public async Task<(List<AdminEmployerDto>? Entities, int? StatusCode, string? ErrorMessage)> GetEmployersBySpecializationAsync(string? specialization)
    {
        var query = _context.Employers
            .IgnoreQueryFilters()
            .Include(e => e.Achievements).ThenInclude(ua => ua.AchievementTemplate)
            .Include(e => e.Vacancies)
            .OrderByDescending(e => e.Name)
            .Where(e => e.AccreditationStatus);

        if (!string.IsNullOrEmpty(specialization))
            query = query.Where(e => e.Specialization != null && e.Specialization.Contains(specialization));

        var employers = await query
            .Select(e => MapToEmployerDto<AdminEmployerDto>(e))
            .ToListAsync();

        return (employers, null, null);
    }

    /// <summary>
    /// Updates an employer's profile by an administrator.
    /// </summary>
    /// <param name="employerId">The unique identifier (GUID) of the employer.</param>
    /// <param name="dto">The data transfer object containing updated employer details.</param>
    /// <returns>A tuple indicating whether the update was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateEmployerAsync(Guid employerId, AdminUpdateEmployerDto dto)
    {
        var employer = await _context.Employers
            .IgnoreQueryFilters()
            .Include(s => s.Achievements).ThenInclude(ua => ua.AchievementTemplate)
            .Include(e => e.Vacancies)
            .FirstOrDefaultAsync(e => e.Id == employerId);

        if (employer == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Employer)));
        if (dto.Email != null && await _context.Users.AnyAsync(u => u.Email == dto.Email && u.Id != employerId))
            return (false, StatusCodes.Status409Conflict, ErrorMessages.EntityAlreadyExists(nameof(User), nameof(User.Email)));
        if (dto.ContactPhone != null && await _context.Users.AnyAsync(u => u.ContactPhone == dto.ContactPhone && u.Id != employerId))
            return (false, StatusCodes.Status409Conflict, ErrorMessages.EntityAlreadyExists(nameof(User), nameof(User.ContactPhone)));

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
    /// <param name="employerId">The unique identifier (GUID) of the employer.</param>
    /// <param name="hardDelete">A boolean indicating whether to perform a hard delete (default is false).</param>
    /// <returns>A tuple indicating whether the deletion was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteEmployerAsync(Guid employerId, bool hardDelete = false)
    {
        var employer = await _context.Employers
            .IgnoreQueryFilters()
            .Include(s => s.Achievements).ThenInclude(ua => ua.AchievementTemplate)
            .Include(e => e.Vacancies)
            .FirstOrDefaultAsync(e => e.Id == employerId);

        if (employer == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Employer)));

        if (hardDelete)
        {
            _context.Employers.Remove(employer);
        }
        else
        {
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
                .Where(i => (i.SenderId == employer.Id || i.ReceiverId == employer.Id) && i.Status != Invitation.InvitationStatus.Rejected)
                .ToListAsync();

            foreach (var invitation in invitations)
            {
                invitation.Status = Invitation.InvitationStatus.Rejected;
                invitation.UpdatedAt = DateTime.UtcNow;
            }

            var vacancyIds = employer.Vacancies?.Select(v => v.Id).ToList();
            if (vacancyIds != null)
            {
                var favorites = await _context.Favorites
                    .Where(f => f.EmployerId == employer.Id || (f.VacancyId.HasValue && vacancyIds.Contains(f.VacancyId.Value)))
                    .ToListAsync();

                _context.Favorites.RemoveRange(favorites);
            }
        }

        return await SaveChangesAsync<Employer>();
    }
}
