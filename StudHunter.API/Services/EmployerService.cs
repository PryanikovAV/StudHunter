using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.Employer;
using StudHunter.API.ModelsDto.UserAchievement;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

public class EmployerService(StudHunterDbContext context, IPasswordHasher passwordHasher) : BaseService(context)
{
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    public async Task<(EmployerDto? Entity, int? StatusCode, string? ErrorMessage)> GetEmployerAsync(Guid id)
    {
        var employer = await _context.Employers
        .Include(e => e.Vacancies)
        .Include(s => s.Achievements)
        .ThenInclude(ua => ua.AchievementTemplate)
        .FirstOrDefaultAsync(e => e.Id == id);

        #region Serializers
        if (employer == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.NotFound("Employer"));
        #endregion

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
            Specialization = employer.Specialization,
            VacancyIds = employer.Vacancies.Select(v => v.Id).ToList(),
            Achievements = employer.Achievements.Select(userAchievement => new UserAchievementDto
            {
                UserId = userAchievement.UserId,
                AchievementTemplateId = userAchievement.AchievementTemplateId,
                AchievementAt = userAchievement.AchievementAt,
                AchievementName = userAchievement.AchievementTemplate.Name,
                AchievementDescription = userAchievement.AchievementTemplate.Description
            }).ToList()
        }, null, null);
    }

    public async Task<(EmployerDto? Entity, int? StatusCode, string? ErrorMessage)> CreateEmployerAsync(CreateEmployerDto dto)
    {
        var emailExists = await _context.Employers.FirstOrDefaultAsync(e => e.Email == dto.Email);

        #region Serializers
        if (emailExists != null)
            return (null, StatusCodes.Status409Conflict, ErrorMessages.AlreadyExists("Employer", "Email"));
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

        var (success, statusCode, errorMessage) = await SaveChangesAsync("Employer");

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

    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateEmployerAsync(Guid id, UpdateEmployerDto dto)
    {
        var employer = await _context.Employers.FirstOrDefaultAsync(e => e.Id == id);

        #region Serializers
        if (employer == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.NotFound("Employer"));

        if (dto.Email != null)
        {
            var emailExists = await _context.Employers.AnyAsync(s => s.Email == dto.Email && s.Id != id);
            if (emailExists)
                return (false, StatusCodes.Status409Conflict, ErrorMessages.AlreadyExists("Employer", "Email"));
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

        var (success, statusCode, errorMessage) = await SaveChangesAsync("Employer");

        return (success, statusCode, errorMessage);
    }

    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteEmployerAsync(Guid id)
    {
        return await SoftDeleteEntityAsync<Employer>(id);
    }
}
