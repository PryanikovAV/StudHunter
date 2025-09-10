using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;

namespace StudHunter.API.Services;

/// <summary>
/// Service for managing faculties.
/// </summary>
public class FacultyService(StudHunterDbContext context) : BaseFacultyService(context)
{

}
