using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;

namespace StudHunter.API.Services;

/// <summary>
/// Service for managing specialties.
/// </summary>
public class SpecialityService(StudHunterDbContext context) : BaseSpecialityService(context)
{

}
