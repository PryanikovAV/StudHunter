using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.Favorite;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.BaseServices;

public class BaseFavoriteService(StudHunterDbContext context) : BaseService(context)
{
    /// <summary>
    /// Retrieves all favorites for a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier (GUID) of the user.</param>
    /// <returns>A tuple containing a list of favorites, an optional status code, and an optional error message.</returns>
    public async Task<(List<FavoriteDto>? Entities, int? StatusCode, string? ErrorMessage)> GetAllFavoritesByUserAsync(Guid userId)
    {
        var favorites = await _context.Favorites
        .Where(f => f.UserId == userId)
        .Select(f => new FavoriteDto
        {
            Id = f.Id,
            UserId = f.UserId,
            VacancyId = f.VacancyId,
            ResumeId = f.ResumeId,
            AddedAt = f.AddedAt
        })
        .ToListAsync();

        return (favorites, null, null);
    }

    /// <summary>
    /// Retrieves a favorite by its ID for a specific user.
    /// </summary>
    /// <param name="favoriteId">The unique identifier (GUID) of the favorite.</param>
    /// <param name="userId">The unique identifier (GUID) of the user.</param>
    /// <returns>A tuple containing the favorite, an optional status code, and an optional error message.</returns>
    public async Task<(FavoriteDto? Entity, int? StatusCode, string? ErrorMessage)> GetFavoriteAsync(Guid favoriteId, Guid userId)
    {
        var favorite = await _context.Favorites
        .Where(f => f.Id == favoriteId && f.UserId == userId)
        .Select(f => new FavoriteDto
        {
            Id = f.Id,
            UserId = f.UserId,
            VacancyId = f.VacancyId,
            ResumeId = f.ResumeId,
            AddedAt = f.AddedAt
        })
        .FirstOrDefaultAsync();

        #region Serializers
        if (favorite == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.NotFound(nameof(Favorite)));
        #endregion

        return (favorite, null, null);
    }
}
