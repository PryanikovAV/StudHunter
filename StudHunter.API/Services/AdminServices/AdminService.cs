using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.Admin;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.AdminServices;

public class AdminService(StudHunterDbContext context, IPasswordHasher passwordHasher) : BaseService(context)
{
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    public async Task<(List<AdminDto>? Entities, int? StatusCode, string? ErrorMessage)> GetAllAdministratorsAsync()
    {
        var administrators = await _context.Administrators.Select(a => new AdminDto
        {
            Id = a.Id,
            Email = a.Email,
            ContactEmail = a.ContactEmail,
            ContactPhone = a.ContactPhone,
            CreatedAt = a.CreatedAt,
            IsDeleted = a.IsDeleted,
            FirstName = a.FirstName,
            LastName = a.LastName,
            AdminLevel = a.AdminLevel.ToString()
        }).ToListAsync();

        return (administrators, null, null);
    }

    public async Task<(AdminDto? Entity, int? StatusCode, string? ErrorMessage)> GetAdministratorAsync(Guid id)
    {
        var administrator = await _context.Administrators.FirstOrDefaultAsync(a => a.Id == id);

        #region Serializers
        if (administrator == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.NotFound("Admin"));
        #endregion

        return (new AdminDto
        {
            Id = administrator.Id,
            Email = administrator.Email,
            ContactEmail = administrator.ContactEmail,
            ContactPhone = administrator.ContactPhone,
            CreatedAt = administrator.CreatedAt,
            IsDeleted = administrator.IsDeleted,
            FirstName = administrator.FirstName,
            LastName = administrator.LastName,
            AdminLevel = administrator.AdminLevel.ToString()
        }, null, null);
    }

    public async Task<(AdminDto? Entity, int? StatusCode, string? ErrorMessage)> CreateAdministratorAsync(CreateAdminDto dto)
    {
        #region Serializers
        var adminExists = await _context.Administrators.AnyAsync(e => e.Email == dto.Email);
        if (adminExists)
            return (null, StatusCodes.Status409Conflict, ErrorMessages.AlreadyExists("Admin", "Email"));
        #endregion

        var administrator = new Administrator
        {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            PasswordHash = _passwordHasher.HashPassword(dto.Password),
            ContactEmail = dto.ContactEmail,
            ContactPhone = dto.ContactPhone,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = dto.IsDeleted,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            AdminLevel = Enum.Parse<Administrator.AdministratorLevel>(dto.AdminLevel)
        };

        _context.Administrators.Add(administrator);

        var (success, statusCode, errorMessage) = await SaveChangesAsync<Administrator>();

        if (!success)
            return (null, statusCode, errorMessage);

        return (new AdminDto
        {
            Id = administrator.Id,
            Email = administrator.Email,
            ContactEmail = administrator.ContactEmail,
            ContactPhone = administrator.ContactPhone,
            CreatedAt = administrator.CreatedAt,
            IsDeleted = administrator.IsDeleted,
            FirstName = administrator.FirstName,
            LastName = administrator.LastName,
            AdminLevel = administrator.AdminLevel.ToString()
        }, null, null);
    }

    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateAdministratorAsync(Guid id, UpdateAdminDto dto)
    {
        var administrator = await _context.Administrators.FirstOrDefaultAsync(a => a.Id == id);

        #region Serializers
        if (administrator == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.NotFound("Admin"));

        if (dto.Email != null)
        {
            var adminExists = await _context.Administrators.AnyAsync(e => e.Email == dto.Email && e.Id != id);
            if (adminExists)
                return (false, StatusCodes.Status409Conflict, ErrorMessages.AlreadyExists("Admin", "Email"));
        }
        #endregion

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
        if (dto.IsDeleted.HasValue)
            administrator.IsDeleted = dto.IsDeleted.Value;
        if (dto.AdminLevel != null)
            administrator.AdminLevel = Enum.Parse<Administrator.AdministratorLevel>(dto.AdminLevel);

        var (success, statusCode, errorMessage) = await SaveChangesAsync<Administrator>();

        return (success, statusCode, errorMessage);
    }

    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeletedministratorAsync(Guid id, bool hardDelete = false)
    {
        return await DeleteEntityAsync<Administrator>(id, hardDelete);
    }
}
