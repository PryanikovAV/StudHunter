using Microsoft.EntityFrameworkCore;
using StudHunter.API.ModelsDto.Administrator;
using StudHunter.API.ModelsDto.Message;
using StudHunter.API.ModelsDto.Invitation;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;
using StudHunter.API.Services.CommonService;

namespace StudHunter.API.Services.AdministratorServices;

public class AdministratorService(StudHunterDbContext context, IPasswordHasher passwordHasher) : BaseAdministratorService(context)
{
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    public async Task<IEnumerable<AdministratorDto>> GetAdministratorsAsync()
    {
        return await _context.Administrators.Select(a => new AdministratorDto
        {
            Id = a.Id,
            Email = a.Email,
            ContactEmail = a.ContactEmail,
            ContactPhone = a.ContactPhone,
            CreatedAt = a.CreatedAt,
            FirstName = a.FirstName,
            LastName = a.LastName,
            AdminLevel = a.AdminLevel.ToString()
        })
            .ToListAsync();
    }

    public async Task<AdministratorDto?> GetAdministratorAsync(Guid id)
    {
        var administrator = await _context.Administrators.FirstOrDefaultAsync(a => a.Id == id);

        if (administrator == null)
            return null;

        return new AdministratorDto
        {
            Id = administrator.Id,
            Email = administrator.Email,
            ContactEmail = administrator.ContactEmail,
            ContactPhone = administrator.ContactPhone,
            CreatedAt = administrator.CreatedAt,
            FirstName = administrator.FirstName,
            LastName = administrator.LastName,
            AdminLevel = administrator.AdminLevel.ToString()
        };
    }

    public async Task<(AdministratorDto? Administrator, string? Error)> CreateAdministratorAsync(CreateAdministratorDto dto)
    {
        if (await _context.Administrators.AnyAsync(e => e.Email == dto.Email))
            return (null, "Administrator with this email already exists");

        var administrator = new Administrator
        {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            PasswordHash = _passwordHasher.HashPassword(dto.Password),
            ContactEmail = dto.ContactEmail,
            ContactPhone = dto.ContactPhone,
            CreatedAt = DateTime.UtcNow,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            AdminLevel = Enum.Parse<Administrator.AdministratorLevel>(dto.AdminLevel)
        };

        _context.Administrators.Add(administrator);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return (null, $"Failed to create administrator: {ex.InnerException?.Message}");
        }

        return (new AdministratorDto
        {
            Id = administrator.Id,
            Email = administrator.Email,
            ContactEmail = administrator.ContactEmail,
            ContactPhone = administrator.ContactPhone,
            CreatedAt = administrator.CreatedAt,
            FirstName = administrator.FirstName,
            LastName = administrator.LastName,
            AdminLevel = administrator.AdminLevel.ToString()
        }, null);
    }

    public async Task<(bool Success, string? Error)> UpdateAdministratorAsync(Guid id, UpdateAdministratorDto dto)
    {
        var administrator = await _context.Administrators.FirstOrDefaultAsync(a => a.Id == id);

        if (administrator == null)
            return (false, "Administrator not found");

        if (dto.Email != null && await _context.Administrators.AnyAsync(e => e.Email == dto.Email && e.Id != id))
            return (false, "Administrator with this email already exists");

        if (dto.Email != null)
            administrator.Email = dto.Email;
        if (dto.Password != null)
            administrator.PasswordHash = _passwordHasher.HashPassword(dto.Password);
        if (dto.ContactEmail != null)
            administrator.ContactEmail = dto.ContactEmail;
        if (dto.ContactPhone != null)
            administrator.ContactPhone = dto.ContactPhone;
        if (dto.FirstName != null)
            administrator.FirstName = dto.FirstName;
        if (dto.LastName != null)
            administrator.LastName = dto.LastName;
        if (dto.AdminLevel != null)
            administrator.AdminLevel = Enum.Parse<Administrator.AdministratorLevel>(dto.AdminLevel);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return (false, $"Failed to update administrator: {ex.InnerException?.Message}");
        }
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> DeleteAdministratorAsync(Guid id)
    {
        return await DeleteEntityAsync<Administrator>(id);
    }
}
