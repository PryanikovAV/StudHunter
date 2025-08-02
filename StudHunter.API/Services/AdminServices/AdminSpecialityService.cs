using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.Speciality;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.AdminServices;

/// <summary>
/// Service for managing specialties with administrative privileges.
/// </summary>
public class AdminSpecialityService(StudHunterDbContext context) : SpecialityService(context)
{
    /// <summary>
    /// Creates a new specialty.
    /// </summary>
    /// <param name="dto">The data transfer object containing specialty details.</param>
    /// <returns>A tuple containing the created specialty, an optional status code, and an optional error message.</returns>
    public async Task<(SpecialityDto? Entity, int? StatusCode, string? ErrorMessage)> CreateSpecialityAsync(CreateSpecialityDto dto)
    {
        #region Serializers
        if (await _context.Specialities.AnyAsync(s => s.Name == dto.Name))
            return (null, StatusCodes.Status409Conflict, ErrorMessages.AlreadyExists(nameof(Speciality), "name"));
        #endregion

        var speciality = new Speciality
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description
        };

        _context.Specialities.Add(speciality);

        var (success, statusCode, errorMessage) = await SaveChangesAsync<Speciality>();

        if (!success)
            return (null, statusCode, errorMessage);

        return (new SpecialityDto
        {
            Id = speciality.Id,
            Name = speciality.Name,
            Description = speciality.Description
        }, null, null);
    }

    /// <summary>
    /// Updates an existing specialty.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the specialty.</param>
    /// <param name="dto">The data transfer object containing updated specialty details.</param>
    /// <returns>A tuple indicating whether the update was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateSpecialityAsync(Guid id, UpdateSpecialityDto dto)
    {
        var speciality = await _context.Specialities.FindAsync(id);

        #region Serializers
        if (speciality == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.NotFound(nameof(Speciality)));

        if (dto.Name != null && await _context.Specialities.AnyAsync(s => s.Name == dto.Name && s.Id != id))
            return (false, StatusCodes.Status409Conflict, ErrorMessages.AlreadyExists(nameof(Speciality), "name"));
        #endregion

        if (dto.Name != null)
            speciality.Name = dto.Name;
        if (dto.Description != null)
            speciality.Description = dto.Description;

        var (success, statusCode, errorMessage) = await SaveChangesAsync<Speciality>();

        return (success, statusCode, errorMessage);
    }

    /// <summary>
    /// Deletes a specialty.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the specialty.</param>
    /// <returns>A tuple indicating whether the deletion was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteSpecialityAsync(Guid id)
    {
        #region Serializers
        if (await _context.StudyPlans.AnyAsync(sp => sp.SpecialityId == id))
            return (false, StatusCodes.Status400BadRequest, ErrorMessages.CannotDelete(nameof(Speciality), nameof(StudyPlan)));
        #endregion

        return await DeleteEntityAsync<Speciality>(id, hardDelete: true);
    }
}
