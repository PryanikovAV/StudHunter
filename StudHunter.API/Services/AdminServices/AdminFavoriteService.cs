using StudHunter.DB.Postgres;

namespace StudHunter.API.Services.AdminServices;

public class AdminFavoriteService(StudHunterDbContext context) : FavoriteService(context)
{

}
