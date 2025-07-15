using Microsoft.EntityFrameworkCore;
using StudHunter.API.ModelsDto.Favorite;
using StudHunter.API.Services.CommonService;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.AdministratorServices;

public class AdministratorFavoriteService(StudHunterDbContext context) : BaseAdministratorService(context)
{
    public async Task<IEnumerable<FavoriteDto>> GetAllFavoritesAsync()
    {
        return await _context.Favorites.Select(f => new FavoriteDto
        {
            Id = f.Id,
            UserId = f.UserId,
            VacancyId = f.VacancyId,
            ResumeId = f.ResumeId,
            AddedAt = f.AddedAt
        })
            .ToListAsync();
    }

    public async Task<IEnumerable<FavoriteDto>> GetFavoritesAsync(Guid userId)
    {
        return await _context.Favorites
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
    }

    public async Task<(bool Success, string? Error)> DeleteFavoriteAsync(Guid id)
    {
        return await DeleteEntityAsync<Favorite>(id);
    }
}
