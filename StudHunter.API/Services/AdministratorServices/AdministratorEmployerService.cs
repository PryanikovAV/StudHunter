using Microsoft.EntityFrameworkCore;
using StudHunter.API.ModelsDto.Employer;
using StudHunter.API.Services.CommonService;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.AdministratorServices;

public class AdministratorEmployerService(StudHunterDbContext context) : BaseAdministratorService(context)
{
    public async Task<IEnumerable<EmployerDto>> GetAllEmployersAsync()
    {
        return await _context.Employers
            .Include(e => e.Vacancies)
            .Select(e => new EmployerDto
            {
                Id = e.Id,
                Email = e.Email,
                ContactEmail = e.ContactEmail,
                ContactPhone = e.ContactPhone,
                CreatedAt = e.CreatedAt,
                AccreditationStatus = e.AccreditationStatus,
                Name = e.Name,
                Description = e.Description,
                Website = e.Website,
                Specialization = e.Specialization,
                VacancyIds = e.Vacancies.Select(v => v.Id).ToList()
            })
            .ToListAsync();
    }

    public async Task<EmployerDto?> GetEmployerAsync(Guid id)
    {
        var employer = await _context.Employers
            .Include(e => e.Vacancies)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employer == null)
            return null;

        return new EmployerDto
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
            VacancyIds = employer.Vacancies.Select(v => v.Id).ToList()
        };
    }

    public async Task<(bool Success, string? Error)> UpdateEmployerAsync(Guid id, UpdateEmployerByAdministratorDto dto)
    {
        var employer = await _context.Employers.FirstOrDefaultAsync(e => e.Id == id);

        if (employer == null)
            return (false, "Employer not found");

        if (dto.Email != null && await _context.Employers.AnyAsync(e => e.Email == dto.Email && e.Id != id))
            return (false, "Another employer with this email already exists");

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
        employer.AccreditationStatus = dto.AccreditationStatus;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return (false, $"Failed to update employer: {ex.InnerException?.Message}");
        }

        return (true, null);
    }

    public async Task<(bool Success, string? Error)> DeleteEmployerAsync(Guid id)
    {
        return await DeleteEntityAsync<Employer>(id);
    }
}
