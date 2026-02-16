using Microsoft.EntityFrameworkCore;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.DB.Postgres;

namespace StudHunter.API.Services.BaseServices;
// TODO: добавить пагинацию
public abstract class BaseDictionariesService(StudHunterDbContext context, IRegistrationManager registrationManager)
    : BaseService(context, registrationManager)
{
    public async Task<Result<List<UniversityDto>>> GetUniversitiesAsync()
    {
        var data = await _context.Universities
            .AsNoTracking()
            .OrderBy(f => f.Name)
            .Select(f => new UniversityDto(f.Id, f.Name, f.Abbreviation))
            .ToListAsync();
        return Result<List<UniversityDto>>.Success(data);
    }

    public async Task<Result<List<LookupDto>>> GetFacultiesAsync()
    {
        var data = await _context.Faculties
            .AsNoTracking()
            .OrderBy(f => f.Name)
            .Select(f => new LookupDto(f.Id, f.Name))
            .ToListAsync();
        return Result<List<LookupDto>>.Success(data);
    }

    public async Task<Result<List<DepartmentDto>>> GetDepartmentsAsync()
    {
        var data = await _context.Departments
            .AsNoTracking()
            .OrderBy(d => d.Name)
            .Select(d => new DepartmentDto(d.Id, d.Name, d.Description))
            .ToListAsync();
        return Result<List<DepartmentDto>>.Success(data);
    }

    public async Task<Result<List<StudyDirectionDto>>> GetSpecialitiesAsync()
    {
        var data = await _context.StudyDirections
            .AsNoTracking()
            .OrderBy(s => s.Name)
            .Select(s => new StudyDirectionDto(s.Id, s.Name, s.Code))
            .ToListAsync();
        return Result<List<StudyDirectionDto>>.Success(data);
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

    public async Task<Result<List<CourseDto>>> GetAllCoursesAsync()
    {
        var data = await _context.Courses
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CourseDto(c.Id, c.Name, c.Description))
            .ToListAsync();
        return Result<List<CourseDto>>.Success(data);
    }

    public async Task<Result<List<CourseDto>>> SearchCoursesAsync(string searchTerm, int limit)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return Result<List<CourseDto>>.Success(new List<CourseDto>());

        var query = $"%{searchTerm.Trim()}%";

        var data = await _context.Courses
            .AsNoTracking()
            .Where(c => EF.Functions.ILike(c.Name, query))  // 'ILike' PostgreSQL: (А = а)
            .OrderBy(c => c.Name)
            .Take(limit)
            .Select(c => new CourseDto(c.Id, c.Name, c.Description))
            .ToListAsync();

        return Result<List<CourseDto>>.Success(data);
    }

    public async Task<Result<List<LookupDto>>> SearchSkillsAsync(string searchTerm, int limit)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return Result<List<LookupDto>>.Success(new List<LookupDto>());

        var query = $"%{searchTerm.Trim()}%";

        var data = await _context.AdditionalSkills
            .AsNoTracking()
            .Where(s => EF.Functions.ILike(s.Name, query))
            .OrderBy(s => s.Name)
            .Take(limit)
            .Select(s => new LookupDto(s.Id, s.Name))
            .ToListAsync();

        return Result<List<LookupDto>>.Success(data);
    }

    public async Task<Result<List<LookupDto>>> GetCitiesAsync()
    {
        var data = await _context.Cities
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new LookupDto(c.Id, c.Name))
            .ToListAsync();

        return Result<List<LookupDto>>.Success(data);
    }
}