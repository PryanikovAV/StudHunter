using Microsoft.EntityFrameworkCore;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.DB.Postgres;

namespace StudHunter.API.Services.BaseServices;

public abstract class BaseDictionariesService(StudHunterDbContext context) : BaseService(context)
{
    public async Task<Result<List<LookupDto>>> GetFacultiesAsync()
    {
        var data = await _context.Faculties
            .AsNoTracking()
            .OrderBy(f => f.Name)
            .Select(f => new LookupDto(f.Id, f.Name))
            .ToListAsync();
        return Result<List<LookupDto>>.Success(data);
    }

    public async Task<Result<List<SpecialityLookupDto>>> GetSpecialitiesAsync()
    {
        var data = await _context.StudyDirections
            .AsNoTracking()
            .OrderBy(s => s.Name)
            .Select(s => new SpecialityLookupDto(s.Id, s.Name, s.Code))
            .ToListAsync();
        return Result<List<SpecialityLookupDto>>.Success(data);
    }

    public async Task<Result<List<LookupDto>>> GetSkillsAsync()
    {
        var data = await _context.AdditionalSkills
            .AsNoTracking()
            .OrderBy(s => s.Name)
            .Select(s => new LookupDto(s.Id, s.Name))
            .ToListAsync();
        return Result<List<LookupDto>>.Success(data);
    }

    public async Task<Result<List<CourseLookupDto>>> GetCoursesAsync()
    {
        var data = await _context.Courses
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CourseLookupDto(c.Id, c.Name, c.Description))
            .ToListAsync();
        return Result<List<CourseLookupDto>>.Success(data);
    }
}