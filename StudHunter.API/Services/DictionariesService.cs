using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;

namespace StudHunter.API.Services;

public interface IDictionariesService
{
    Task<Result<List<LookupDto>>> GetFacultiesAsync();
    Task<Result<List<SpecialityLookupDto>>> GetSpecialitiesAsync();
    Task<Result<List<LookupDto>>> GetSkillsAsync();
    Task<Result<List<CourseLookupDto>>> GetCoursesAsync();
}

public class DictionariesService(StudHunterDbContext context) : BaseDictionariesService(context), IDictionariesService;