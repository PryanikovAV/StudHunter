using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.Admin;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.AdminServices;

/// <summary>
/// Service for managing administrators.
/// </summary>
public class AdminService(StudHunterDbContext context, IPasswordHasher passwordHasher) : BaseService(context)
{
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    /// <summary>
    /// Retrieves all administrators.
    /// </summary>
    /// <returns>A tuple containing a list of all administrators, an optional status code, and an optional error message.</returns>
    public async Task<(List<AdminDto>? Entities, int? StatusCode, string? ErrorMessage)> GetAllAdministratorsAsync()
    {
        var administrators = await _context.Administrators
        .Select(a => new AdminDto
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
        })
        .ToListAsync();

        return (administrators, null, null);
    }

    /// <summary>
    /// Retrieves an administrator by their ID.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the administrator.</param>
    /// <returns>A tuple containing the administrator, an optional status code, and an optional error message.</returns>
    public async Task<(AdminDto? Entity, int? StatusCode, string? ErrorMessage)> GetAdministratorAsync(Guid id)
    {
        var administrator = await _context.Administrators.FirstOrDefaultAsync(a => a.Id == id);

        #region Serializers
        if (administrator == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.NotFound("admin"));
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

    /// <summary>
    /// Creates a new administrator.
    /// </summary>
    /// <param name="dto">The data transfer object containing administrator details.</param>
    /// <returns>A tuple containing the created administrator, an optional status code, and an optional error message.</returns>
    public async Task<(AdminDto? Entity, int? StatusCode, string? ErrorMessage)> CreateAdministratorAsync(CreateAdminDto dto)
    {
        #region Serializers
        var adminExists = await _context.Administrators.AnyAsync(e => e.Email == dto.Email);
        if (adminExists)
            return (null, StatusCodes.Status409Conflict, ErrorMessages.AlreadyExists("admin", "email"));
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

    /// <summary>
    /// Updates an existing administrator.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the administrator.</param>
    /// <param name="dto">The data transfer object containing updated administrator details.</param>
    /// <returns>A tuple indicating whether the update was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateAdministratorAsync(Guid id, UpdateAdminDto dto)
    {
        var administrator = await _context.Administrators.FirstOrDefaultAsync(a => a.Id == id);

        #region Serializers
        if (administrator == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.NotFound("admin"));

        if (dto.Email != null)
        {
            var adminExists = await _context.Administrators.AnyAsync(e => e.Email == dto.Email && e.Id != id);
            if (adminExists)
                return (false, StatusCodes.Status409Conflict, ErrorMessages.AlreadyExists("admin", "email"));
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

    /// <summary>
    /// Deletes an administrator.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the administrator.</param>
    /// <param name="hardDelete">A boolean indicating whether to perform a hard delete.</param>
    /// <returns>A tuple indicating whether the deletion was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteAdministratorAsync(Guid id, bool hardDelete = false)
    {
        return await DeleteEntityAsync<Administrator>(id, hardDelete);
    }
}
