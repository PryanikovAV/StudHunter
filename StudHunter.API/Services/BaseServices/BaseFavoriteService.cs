using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.AchievementTemplate;
using StudHunter.API.ModelsDto.Favorite;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.BaseServices;

public abstract class BaseFavoriteService(StudHunterDbContext context) : BaseService(context)
{
    protected static FavoriteDto MapToFavoriteDto(Favorite favorite)
    {
        return new FavoriteDto
        {
            Id = favorite.Id,
            UserId = favorite.UserId,
            VacancyId = favorite.VacancyId,
            EmployerId = favorite.EmployerId,
            StudentId = favorite.StudentId,
            AddedAt = favorite.AddedAt
        };
    }

    /// <summary>
    /// Retrieves all favorites for a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier (GUID) of the user.</param>
    /// <returns>A tuple containing a list of favorites, an optional status code, and an optional error message.</returns>
    public async Task<(List<FavoriteDto>? Entities, int? StatusCode, string? ErrorMessage)> GetAllFavoritesByUserAsync(Guid userId)
    {
        var favorites = await _context.Favorites
        .Where(f => f.UserId == userId)
        .Select(f => MapToFavoriteDto(f))
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
        .Select(f => MapToFavoriteDto(f))
        .FirstOrDefaultAsync();

        if (favorite == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Favorite)));

        return (favorite, null, null);
    }
}
