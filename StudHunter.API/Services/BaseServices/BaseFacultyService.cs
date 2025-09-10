using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.FacultyDto;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.BaseServices;

public abstract class BaseFacultyService(StudHunterDbContext context) : BaseService(context)
{
    protected static FacultyDto MapToFacultyDto(Faculty faculty)
    {
        return new FacultyDto
        {
            Id = faculty.Id,
            Name = faculty.Name,
            Description = faculty.Description,
        };
    }

    /// <summary>
    /// Retrieves all faculties.
    /// </summary>
    /// <returns>A tuple containing a list of all faculties, an optional status code, and an optional error message.</returns>
    public async Task<(List<FacultyDto>? Entities, int? StatusCode, string? ErrorMessage)> GetAllFacultiesAsync()
    {
        var faculties = await _context.Faculties
            .Select(f => MapToFacultyDto(f))
            .OrderByDescending(f => f.Name)
            .ToListAsync();

        return (faculties, null, null);
    }

    /// <summary>
    /// Retrieves a faculty by its ID.
    /// </summary>
    /// <param name="facultyId">The unique identifier (GUID) of the faculty.</param>
    /// <returns>A tuple containing the faculty, an optional status code, and an optional error message.</returns>
    public async Task<(FacultyDto? Entity, int? StatusCode, string? ErrorMessage)> GetFacultyAsync(Guid facultyId)
    {
        var faculty = await _context.Faculties.FindAsync(facultyId);
        if (faculty == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Faculty)));

        return (MapToFacultyDto(faculty), null, null);
    }

    /// <summary>
    /// Retrieves a faculty by its name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public async Task<(FacultyDto? Entity, int? StatusCode, string? ErrorMessage)> GetFacultyAsync(string name)
    {
        var faculty = await _context.Faculties.FirstOrDefaultAsync(f => f.Name == name);
        if (faculty == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Faculty)));

        return (MapToFacultyDto(faculty), null, null);
    }
}
