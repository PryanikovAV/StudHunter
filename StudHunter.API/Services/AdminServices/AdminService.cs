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
        .Where(a => !a.IsDeleted)
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
            Patronymic = a.Patronymic
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
        var administrator = await _context.Administrators
        .Where(a => a.Id == id && !a.IsDeleted)
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
            Patronymic = a.Patronymic
        })
        .FirstOrDefaultAsync();

        if (administrator == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Administrator)));

        return (administrator, null, null);
    }

    /// <summary>
    /// Updates an existing administrator.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the administrator.</param>
    /// <param name="dto">The data transfer object containing updated administrator details.</param>
    /// <returns>A tuple indicating whether the update was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateAdministratorAsync(Guid id, UpdateAdminDto dto)
    {
        var administrator = await _context.Administrators
        .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);

        if (administrator == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Administrator)));

        if (dto.Email != null)
        {
            if (await _context.Administrators.AnyAsync(e => e.Email == dto.Email && e.Id != id))
                return (false, StatusCodes.Status409Conflict, ErrorMessages.EntityAlreadyExists(nameof(Administrator), "email"));
        }

        if (dto.Email != null)
            administrator.UpdateEmail(dto.Email);
        if (dto.Password != null)
            administrator.UpdatePassword(_passwordHasher.HashPassword(dto.Password));
        if (dto.ContactEmail != null)
            administrator.ContactEmail = dto.ContactEmail;
        if (dto.ContactPhone != null)
            administrator.ContactPhone = dto.ContactPhone;
        if (dto.FirstName != null)
            administrator.FirstName = dto.FirstName;
        if (dto.LastName != null)
            administrator.LastName = dto.LastName;
        if (dto.Patronymic != null)
            administrator.Patronymic = dto.Patronymic;
        if (dto.IsDeleted.HasValue)
            administrator.IsDeleted = dto.IsDeleted.Value;

        return await SaveChangesAsync<Administrator>();
    }
}
