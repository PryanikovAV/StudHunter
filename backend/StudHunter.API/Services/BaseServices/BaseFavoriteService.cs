using Microsoft.EntityFrameworkCore;
using StudHunter.API.ModelsDto;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.BaseServices;

public abstract class BaseFavoriteService(StudHunterDbContext context) : BaseService(context)
{
    protected IQueryable<Favorite> GetFullFavoriteQuery() =>
        _context.Favorites
            .Include(f => f.Vacancy).ThenInclude(v => v!.Employer)
            .Include(f => f.Resume).ThenInclude(r => r!.Student)
            .Include(f => f.Employer);

    protected async Task<Guid?> GetTargetOwnerIdAsync(Guid targetId, FavoriteType type)
    {
        return type switch
        {
            FavoriteType.Vacancy => (await _context.Vacancies.AsNoTracking()
                .FirstOrDefaultAsync(v => v.Id == targetId))?.EmployerId,

            FavoriteType.Resume => (await _context.Resumes.AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == targetId))?.StudentId,

            FavoriteType.Employer => targetId,
            _ => null
        };
    }
}
