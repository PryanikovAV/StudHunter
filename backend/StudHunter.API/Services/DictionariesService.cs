using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;

namespace StudHunter.API.Services;

public interface IDictionariesService
{
    Task<Result<List<UniversityDto>>> GetUniversitiesAsync();
    Task<Result<List<LookupDto>>> GetFacultiesAsync();
    Task<Result<List<DepartmentDto>>> GetDepartmentsAsync();
    Task<Result<List<StudyDirectionDto>>> GetSpecialitiesAsync();
    Task<Result<List<LookupDto>>> GetSkillsAsync();
    Task<Result<List<CourseDto>>> GetAllCoursesAsync();
    Task<Result<List<CourseDto>>> SearchCoursesAsync(string searchTerm, int limit);
    Task<Result<List<LookupDto>>> SearchSkillsAsync(string searchTerm, int limit);
    Task<Result<List<LookupDto>>> GetCitiesAsync();
    Task<Result<List<LookupDto>>> GetAllSpecializationsAsync();
}

public class DictionariesService(StudHunterDbContext context, IRegistrationManager registrationManager)
    : BaseDictionariesService(context, registrationManager), IDictionariesService;