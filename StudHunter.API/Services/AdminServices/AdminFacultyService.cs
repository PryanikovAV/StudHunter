using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.Faculty;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.AdminServices;

/// <summary>
/// Service for managing faculties with administrative privileges.
/// </summary>
public class AdminFacultyService(StudHunterDbContext context) : FacultyService(context)
{
    /// <summary>
    /// Creates a new faculty.
    /// </summary>
    /// <param name="dto">The data transfer object containing faculty details.</param>
    /// <returns>A tuple containing the created faculty, an optional status code, and an optional error message.</returns>
    public async Task<(FacultyDto? Entity, int? StatusCode, string? ErrorMessage)> CreateFacultyAsync(CreateFacultyDto dto)
    {
        if (await _context.Faculties.AnyAsync(f => f.Name == dto.Name))
            return (null, StatusCodes.Status409Conflict, ErrorMessages.EntityAlreadyExists(nameof(Faculty), "name"));

        var faculty = new Faculty
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description
        };

        _context.Faculties.Add(faculty);

        var (success, statusCode, errorMessage) = await SaveChangesAsync<Faculty>();

        if (!success)
            return (null, statusCode, errorMessage);

        return (new FacultyDto
        {
            Id = faculty.Id,
            Name = faculty.Name,
            Description = faculty.Description
        }, null, null);
    }

    /// <summary>
    /// Updates an existing faculty.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the faculty.</param>
    /// <param name="dto">The data transfer object containing updated faculty details.</param>
    /// <returns>A tuple indicating whether the update was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateFacultyAsync(Guid id, UpdateFacultyDto dto)
    {
        var faculty = await _context.Faculties.FindAsync(id);

        if (faculty == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Faculty)));

        if (dto.Name != null && await _context.Faculties.AnyAsync(f => f.Name == dto.Name && f.Id != id))
            return (false, StatusCodes.Status409Conflict, ErrorMessages.EntityAlreadyExists(nameof(Faculty), "name"));

        if (dto.Name != null)
            faculty.Name = dto.Name;
        if (dto.Description != null)
            faculty.Description = dto.Description;

        return await SaveChangesAsync<Faculty>();
    }

    /// <summary>
    /// Deletes a faculty.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the faculty.</param>
    /// <returns>A tuple indicating whether the deletion was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteFacultyAsync(Guid id)
    {
        if (await _context.StudyPlans.AnyAsync(sp => sp.FacultyId == id))
            return (false, StatusCodes.Status400BadRequest, ErrorMessages.CannotDelete(nameof(Faculty), nameof(StudyPlan)));

        return await DeleteEntityAsync<Faculty>(id, hardDelete: true);
    }
}
