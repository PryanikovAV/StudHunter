using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.AdminServices;

public class AdminFavoriteService(StudHunterDbContext context) : FavoriteService(context)
{
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteFavoriteAsync(Guid id)
    {
        var favorite = await _context.Favorites.FirstOrDefaultAsync(f => f.Id == id);

        #region Serializers
        if (favorite == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.NotFound("Favorite"));
        #endregion

        return await DeleteEntityAsync<Favorite>(id, hardDelete: true);
    }
}
