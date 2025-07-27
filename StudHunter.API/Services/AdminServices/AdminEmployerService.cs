using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.Employer;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.AdminServices;

public class AdminEmployerService(StudHunterDbContext context) : BaseService(context)
{
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

    public async Task<(AdminEmployerDto? Entity, int? StatusCode, string? ErrorMessage)> GetEmployerAsync(Guid id)
    {
        var employer = await _context.Employers
        .Include(e => e.Vacancies)
        .FirstOrDefaultAsync(e => e.Id == id);

        #region Serializers
        if (employer == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.NotFound("Employer"));
        #endregion

        return (new AdminEmployerDto
        {
            Id = employer.Id,
            Email = employer.Email,
            ContactEmail = employer.ContactEmail,
            ContactPhone = employer.ContactPhone,
            CreatedAt = employer.CreatedAt,
            IsDeleted = employer.IsDeleted,
            AccreditationStatus = employer.AccreditationStatus,
            Name = employer.Name,
            Description = employer.Description,
            Website = employer.Website,
            Specialization = employer.Specialization,
            VacancyIds = employer.Vacancies.Select(v => v.Id).ToList()
        }, null, null);
    }

    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateEmployerAsync(Guid id, AdminUpdateEmployerDto dto)
    {
        var employer = await _context.Employers.FirstOrDefaultAsync(e => e.Id == id);

        #region Serializers
        if (employer == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.NotFound("Employer"));

        if (dto.Email != null)
        {
            var employerExists = await _context.Employers.AnyAsync(e => e.Email == dto.Email && e.Id != id);
            if (employerExists)
                return (false, StatusCodes.Status409Conflict, ErrorMessages.AlreadyExists("Employer", "Email"));
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

        var (success, statusCode, errorMessage) = await SaveChangesAsync("Employer");

        return (success, statusCode, errorMessage);
    }

    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteEmployerAsync(Guid id, bool hardDelete = false)
    {
        return await DeleteEntityAsync<Employer>(id, hardDelete);
    }
}
