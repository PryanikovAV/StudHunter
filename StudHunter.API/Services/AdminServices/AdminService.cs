﻿using Microsoft.EntityFrameworkCore;
using StudHunter.API.ModelsDto.Admin;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;
using StudHunter.API.Services.CommonService;

namespace StudHunter.API.Services.AdminServices;

public class AdminService(StudHunterDbContext context, IPasswordHasher passwordHasher) : BaseService(context)
{
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    public async Task<IEnumerable<AdminDto>> GetAllAdministratorsAsync()
    {
        return await _context.Administrators.Select(a => new AdminDto
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

    public async Task<AdminDto?> GetAdministratorAsync(Guid id)
    {
        var administrator = await _context.Administrators.FirstOrDefaultAsync(a => a.Id == id);

        if (administrator == null)
            return null;

        return new AdminDto
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

    public async Task<(AdminDto? Administrator, string? Error)> CreateAdministratorAsync(CreateAdminDto dto)
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

        var (success, error) = await SaveChangesAsync("create", "Administrator");
        if (!success)
            return (null, error);

        return (new AdminDto
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

    public async Task<(bool Success, string? Error)> UpdateAdministratorAsync(Guid id, UpdateAdminDto dto)
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

        return await SaveChangesAsync("update", "Administrator");
    }

    public async Task<(bool Success, string? Error)> DeletedministratorAsync(Guid id, bool hardDelete = false)
    {
        return hardDelete
        ? await HardDeleteEntityAsync<Administrator>(id)
        : await SoftDeleteEntityAsync<Administrator>(id);
    }
}
