using StudHunter.API.Common;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.AdminServices;

/// <summary>
/// Service for managing favorites with administrative privileges.
/// </summary>
public class AdminFavoriteService(StudHunterDbContext context) : BaseFavoriteService(context)
{
    /// <summary>
    /// Deletes an favorite.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the favorite.</param>
    /// <returns>A tuple indicating whether the deletion was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteFavoriteAsync(Guid id)
    {
        var favorite = await _context.Favorites.FindAsync(id);
        if (favorite == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.NotFound(nameof(Favorite)));

        return await DeleteEntityAsync<Favorite>(id, hardDelete: true);
    }
}
