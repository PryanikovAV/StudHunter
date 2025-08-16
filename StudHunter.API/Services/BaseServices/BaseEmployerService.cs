using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.Employer;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.BaseServices;

public abstract class BaseEmployerService(StudHunterDbContext context) : BaseService(context)
{
    /// <summary>
    /// Maps an employer entity to a specified DTO type.
    /// </summary>
    /// <typeparam name="TDto">The type of DTO tp map to (must inherit from EmployerDto).</typeparam>
    /// <param name="employer">The employer entity to map.</param>
    /// <returns>The mapped DTO.</returns>
    protected TDto MapToEmployerDto<TDto>(Employer employer) where TDto : EmployerDto, new()
    {
        var dto = new TDto
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
            Specialization = employer.Specialization,
            VacancyIds = employer.Vacancies.Select(v => v.Id).ToList(),
            Achievements = employer.Achievements.Select(BaseUserAchievementService.MapToUserAchievementDto).ToList()
        };

        if (dto is AdminEmployerDto adminDto)
        {
            adminDto.IsDeleted = employer.IsDeleted;
        }

        return dto;
    }

    /// <summary>
    /// Retrieves an employer by their ID.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the employer.</param>
    /// <returns>A tuple containing the employer's details, an optional status code, and an optional error message.</returns>
    public async Task<(TDto? Entity, int? StatusCode, string? ErrorMessage)> GetEmployerAsync<TDto>(Guid id) where TDto : EmployerDto, new()
    {
        var employer = await _context.Employers
        .Include(e => e.Vacancies)
        .Include(s => s.Achievements).ThenInclude(ua => ua.AchievementTemplate)
        .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);

        if (employer == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Employer)));

        return (MapToEmployerDto<TDto>(employer), null, null);
    }

    /// <summary>
    /// Retrieves an employer by their email.
    /// </summary>
    /// <param name="email">The email of the employer.</param>
    /// <returns>A tuple containing the employer's details, an optional status code, and an optional error message.</returns>
    public async Task<(TDto? Entity, int? StatusCode, string? ErrorMessage)> GetEmployerByEmailAsync<TDto>(string email) where TDto : EmployerDto, new()
    {
        var employer = await _context.Employers
        .Include(e => e.Vacancies)
        .Include(s => s.Achievements).ThenInclude(ua => ua.AchievementTemplate)
        .FirstOrDefaultAsync(e => e.Email == email && !e.IsDeleted);

        if (employer == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Employer)));

        return (MapToEmployerDto<TDto>(employer), null, null);
    }

    /// <summary>
    /// Retrieves employers by their specialization, including deleted ones.
    /// </summary>
    /// <param name="specialization">Employer specialization.</param>
    /// <returns>A tuple containing the employer's details, an optional status code, and an optional error message.</returns>
    public async Task<(List<TDto>? Entities, int? StatusCode, string? ErrorMessage)> GetEmployersBySpecializationAsync<TDto>(string? specialization) where TDto : EmployerDto, new()
    {
        var query = _context.Employers
        .Include(e => e.Vacancies)
        .Include(e => e.Achievements).ThenInclude(ua => ua.AchievementTemplate)
        .Where(e => !e.IsDeleted && e.AccreditationStatus);

        if (!string.IsNullOrEmpty(specialization))
            query = query.Where(e => e.Specialization != null && e.Specialization.Contains(specialization));

        var employers = await query
        .Select(e => MapToEmployerDto<TDto>(e))
        .ToListAsync();

        return (employers, null, null);
    }
}
