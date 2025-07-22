using Microsoft.EntityFrameworkCore;
using StudHunter.API.ModelsDto.Faculty;
using StudHunter.API.Services.CommonService;
using StudHunter.DB.Postgres;

namespace StudHunter.API.Services;

public class FacultyService(StudHunterDbContext context) : BaseService(context)
{
    public async Task<IEnumerable<FacultyDto>> GetAllFacultiesAsync()
    {
        return await _context.Faculties.Select(f => new FacultyDto
        {
            Id = f.Id,
            Name = f.Name,
            Description = f.Description
        })
        .ToListAsync();
    }

    public async Task<FacultyDto?> GetFacultyAsync(Guid id)
    {
        var faculty = await _context.Faculties.FirstOrDefaultAsync(f => f.Id == id);

        if (faculty == null)
            return null;

        return new FacultyDto
        {
            Id = faculty.Id,
            Name = faculty.Name,
            Description = faculty.Description
        };
    }
}
