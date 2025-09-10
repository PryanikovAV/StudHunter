using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;

namespace StudHunter.API.Services.AdminServices;

/// <summary>
/// Service for managing favorites with administrative privileges.
/// </summary>
public class AdminFavoriteService(StudHunterDbContext context) : BaseFavoriteService(context)
{

}
