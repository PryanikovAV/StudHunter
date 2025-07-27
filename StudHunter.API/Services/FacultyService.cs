using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.Faculty;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;

namespace StudHunter.API.Services;

public class FacultyService(StudHunterDbContext context) : BaseService(context)
{
    public async Task<(List<FacultyDto>? Entities, int? StatusCode, string? ErrorMessage)> GetAllFacultiesAsync()
    {
        var faculties = await _context.Faculties.Select(f => new FacultyDto
        {
            Id = f.Id,
            Name = f.Name,
            Description = f.Description
        }).ToListAsync();

        return (faculties, null, null);
    }

    public async Task<(FacultyDto? Entity, int? StatusCode, string? ErrorMessage)> GetFacultyAsync(Guid id)
    {
        var faculty = await _context.Faculties.FirstOrDefaultAsync(f => f.Id == id);

        #region Serializers
        if (faculty == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.NotFound("Faculty"));
        #endregion

        return (new FacultyDto
        {
            Id = faculty.Id,
            Name = faculty.Name,
            Description = faculty.Description
        }, null, null);
    }
}
