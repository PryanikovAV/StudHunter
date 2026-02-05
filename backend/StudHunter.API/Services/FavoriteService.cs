using Microsoft.EntityFrameworkCore;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

public interface IFavoriteService
{
    Task<Result<PagedResult<FavoriteDto>>> GetMyFavoritesAsync(Guid userId, PaginationParams paging);
    Task<Result<bool>> ToggleFavoriteAsync(Guid userId, FavoriteRequest request);
}

public class FavoriteService(StudHunterDbContext context) : BaseFavoriteService(context), IFavoriteService
{
    public async Task<Result<PagedResult<FavoriteDto>>> GetMyFavoritesAsync(Guid userId, PaginationParams paging)
    {
        var blockedIds = await GetBlockedUserIdsAsync(userId);

        var query = GetFullFavoriteQuery().Where(f => f.UserId == userId);

        if (blockedIds.Any())
        {
            query = query.Where(f =>
                (f.VacancyId != null && !blockedIds.Contains(f.Vacancy!.EmployerId)) ||
                (f.ResumeId != null && !blockedIds.Contains(f.Resume!.StudentId)) ||
                (f.EmployerId != null && !blockedIds.Contains(f.EmployerId.Value))
            );
        }

        var pagedEntities = await query
            .OrderByDescending(f => f.AddedAt)
            .ToPagedResultAsync(paging);

        var dtos = pagedEntities.Items.Select(FavoriteMapper.ToDto).ToList();

        var pagedResult = new PagedResult<FavoriteDto>(
            Items: dtos,
            TotalCount: pagedEntities.TotalCount,
            PageNumber: pagedEntities.PageNumber,
            PageSize: pagedEntities.PageSize
    );

        return Result<PagedResult<FavoriteDto>>.Success(pagedResult);
    }

    public async Task<Result<bool>> ToggleFavoriteAsync(Guid userId, FavoriteRequest request)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return Result<bool>.Failure(ErrorMessages.EntityNotFound(nameof(User)));

        var permission = EnsureCanPerform(user, UserAction.AddToFavorite);
        if (!permission.IsSuccess)
            return Result<bool>.Failure(permission.ErrorMessage!, permission.StatusCode);

        var ownerId = await GetTargetOwnerIdAsync(request.TargetId, request.Type);
        if (ownerId == null)
            return Result<bool>.Failure(ErrorMessages.EntityNotFound("Target"));

        var blackListCheck = await EnsureCommunicationAllowedAsync(userId, ownerId.Value);
        if (!blackListCheck.IsSuccess)
            return Result<bool>.Failure("Действие невозможно: пользователь находится в черном списке.");

        var existingQuery = _context.Favorites.Where(f => f.UserId == userId);

        Favorite? existing = request.Type switch
        {
            FavoriteType.Vacancy => await existingQuery.FirstOrDefaultAsync(f => f.VacancyId == request.TargetId),
            FavoriteType.Resume => await existingQuery.FirstOrDefaultAsync(f => f.ResumeId == request.TargetId),
            FavoriteType.Employer => await existingQuery.FirstOrDefaultAsync(f => f.EmployerId == request.TargetId),
            _ => null
        };

        if (!request.IsFavorite)
        {
            if (existing != null)
            {
                _context.Favorites.Remove(existing);
                await _context.SaveChangesAsync();
            }
            return Result<bool>.Success(false);
        }

        if (existing != null)
            return Result<bool>.Success(true);

        var favorite = new Favorite { UserId = userId };
        switch (request.Type)
        {
            case FavoriteType.Vacancy: favorite.VacancyId = request.TargetId; break;
            case FavoriteType.Resume: favorite.ResumeId = request.TargetId; break;
            case FavoriteType.Employer: favorite.EmployerId = request.TargetId; break;
        }

        _context.Favorites.Add(favorite);
        var result = await SaveChangesAsync<Favorite>();

        return result.IsSuccess
            ? Result<bool>.Success(true)
            : Result<bool>.Failure(result.ErrorMessage!);
    }
}

public record FavoriteRequest(
    Guid TargetId,
    FavoriteType Type,
    bool IsFavorite,
    PaginationParams Paging = null!
)
{
    public PaginationParams Paging { get; init; } = Paging ?? new PaginationParams();
};