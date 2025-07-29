using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.Favorite;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

public class FavoriteService(StudHunterDbContext context) : BaseService(context)
{
    public async Task<(List<FavoriteDto>? Entities, int? StatusCode, string? ErrorMessage)> GetAllFavoritesAsync(Guid userId)
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
        }).ToListAsync();

        return (favorites, null, null);
    }

    public async Task<(FavoriteDto? Entity, int? StatusCode, string? ErrorMessage)> GetFavoriteAsync(Guid id, Guid userId)
    {
        var favorite = await _context.Favorites
        .Where(f => f.Id == id && f.UserId == userId)
        .Select(f => new FavoriteDto
        {
            Id = f.Id,
            UserId = f.UserId,
            VacancyId = f.VacancyId,
        }).FirstOrDefaultAsync();

        #region Serializers
        if (favorite == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.NotFound("Favorite"));
        #endregion

        return (favorite, null, null);
    }

    public async Task<(FavoriteDto? Entity, int? StatusCode, string? ErrorMessage)> CreateFavoriteAsync(Guid userId, CreateFavoriteDto dto)
    {
        #region Serializers
        if (dto.VacancyId == null && dto.ResumeId == null)
            return (null, StatusCodes.Status400BadRequest, "Either VacancyId or ResumeId must be provided.");

        if (dto.VacancyId != null && dto.ResumeId != null)
            return (null, StatusCodes.Status400BadRequest, "Only one of VacancyId or ResumeId can be provided.");

        if (dto.VacancyId != null && !await _context.Vacancies.AnyAsync(v => v.Id == dto.VacancyId))
            return (null, StatusCodes.Status404NotFound, ErrorMessages.NotFound("Vacancy"));

        if (dto.ResumeId != null && !await _context.Resumes.AnyAsync(r => r.Id == dto.ResumeId))
            return (null, StatusCodes.Status404NotFound, ErrorMessages.NotFound("Resume"));

        if (await _context.Favorites.AnyAsync(f => f.UserId == userId && (f.VacancyId == dto.VacancyId || f.ResumeId == dto.ResumeId)))
            return (null, StatusCodes.Status409Conflict, ErrorMessages.AlreadyExists("Favorite", "UserId, VacancyId/ResumeId"));
        #endregion

        var favorite = new Favorite
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            VacancyId = dto.VacancyId,
            ResumeId = dto.ResumeId,
            AddedAt = DateTime.UtcNow
        };

        _context.Favorites.Add(favorite);

        var (success, statusCode, errorMessage) = await SaveChangesAsync<Favorite>();

        if (!success)
            return (null, statusCode, errorMessage);

        return (new FavoriteDto
        {
            Id = favorite.Id,
            UserId = favorite.UserId,
            VacancyId = favorite.VacancyId,
            ResumeId = favorite.ResumeId,
            AddedAt = favorite.AddedAt
        }, null, null);
    }

    public virtual async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteFavoriteAsync(Guid id, Guid userId)
    {
        #region Serializers
        var favorite = await _context.Favorites.FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);
        if (favorite == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.NotFound("Favorite"));
        #endregion

        return await HardDeleteEntityAsync<Favorite>(id);
    }
}
