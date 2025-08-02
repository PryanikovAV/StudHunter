using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.Faculty;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

/// <summary>
/// Service for managing faculties.
/// </summary>
public class FacultyService(StudHunterDbContext context) : BaseService(context)
{
    /// <summary>
    /// Retrieves all faculties.
    /// </summary>
    /// <returns>A tuple containing a list of all faculties, an optional status code, and an optional error message.</returns>
    public async Task<(List<FacultyDto>? Entities, int? StatusCode, string? ErrorMessage)> GetAllFacultiesAsync()
    {
        var faculties = await _context.Faculties
        .Select(f => new FacultyDto
        {
            Id = f.Id,
            Name = f.Name,
            Description = f.Description
        })
        .ToListAsync();

        return (faculties, null, null);
    }

    /// <summary>
    /// Retrieves a faculty by its ID.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the faculty.</param>
    /// <returns>A tuple containing the faculty, an optional status code, and an optional error message.</returns>
    public async Task<(FacultyDto? Entity, int? StatusCode, string? ErrorMessage)> GetFacultyAsync(Guid id)
    {
        var faculty = await _context.Faculties.FindAsync(id);

        #region Serializers
        if (faculty == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.NotFound(nameof(Faculty)));
        #endregion

        return (new FacultyDto
        {
            Id = faculty.Id,
            Name = faculty.Name,
            Description = faculty.Description
        }, null, null);
    }
}
