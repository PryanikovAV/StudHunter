using Microsoft.EntityFrameworkCore;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.DB.Postgres;

namespace StudHunter.API.Services.AdminServices;

public interface IAdminFavoriteService
{
    Task<Result<PagedResult<FavoriteCardDto>>> GetUserFavoritesAsync(Guid targetUserId, PaginationParams paging);
}

public class AdminFavoriteService(StudHunterDbContext context, IRegistrationManager registrationManager)
    : FavoriteService(context, registrationManager), IAdminFavoriteService
{
    public async Task<Result<PagedResult<FavoriteCardDto>>> GetUserFavoritesAsync(Guid targetUserId, PaginationParams paging)
    {
        var query = _context.Favorites
            .AsNoTracking()
            .Include(f => f.Vacancy).ThenInclude(v => v!.Employer)
            .Include(f => f.Student).ThenInclude(s => s!.StudyPlan).ThenInclude(sp => sp!.University)
            .Include(f => f.Employer)
            .Where(f => f.UserId == targetUserId);

        var pagedEntities = await query.OrderByDescending(f => f.AddedAt).ToPagedResultAsync(paging ?? new PaginationParams());

        var dtos = pagedEntities.Items.Select(FavoriteMapper.ToCardDto).ToList();

        return Result<PagedResult<FavoriteCardDto>>.Success(new PagedResult<FavoriteCardDto>(
            dtos, pagedEntities.TotalCount, pagedEntities.PageNumber, pagedEntities.PageSize));
    }
}