using Microsoft.EntityFrameworkCore;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

public interface IFavoriteService
{
    Task<Result<PagedResult<FavoriteCardDto>>> GetMyFavoritesAsync(Guid userId, PaginationParams paging);
    Task<Result<bool>> ToggleFavoriteAsync(Guid userId, FavoriteRequest request);
}

public class FavoriteService(StudHunterDbContext context, IRegistrationManager registrationManager)
    : BaseService(context, registrationManager), IFavoriteService
{
    public async Task<Result<PagedResult<FavoriteCardDto>>> GetMyFavoritesAsync(Guid userId, PaginationParams paging)
    {
        var blockedIds = await GetBlockedUserIdsAsync(userId);

        var query = _context.Favorites
            .AsNoTracking()
            .Include(f => f.Vacancy).ThenInclude(v => v!.Employer)
            .Include(f => f.Student).ThenInclude(s => s!.StudyPlan).ThenInclude(sp => sp!.University)
            .Include(f => f.Employer)
            .Where(f => f.UserId == userId);

        if (blockedIds.Any())
        {
            query = query.Where(f =>
                (f.VacancyId != null && !blockedIds.Contains(f.Vacancy!.EmployerId)) ||
                (f.StudentId != null && !blockedIds.Contains(f.StudentId.Value)) ||
                (f.EmployerId != null && !blockedIds.Contains(f.EmployerId.Value))
            );
        }

        var pagedEntities = await query.OrderByDescending(f => f.AddedAt).ToPagedResultAsync(paging ?? new PaginationParams());
        var dtos = pagedEntities.Items.Select(FavoriteMapper.ToCardDto).ToList();

        return Result<PagedResult<FavoriteCardDto>>.Success(new PagedResult<FavoriteCardDto>(
            dtos, pagedEntities.TotalCount, pagedEntities.PageNumber, pagedEntities.PageSize));
    }

    public async Task<Result<bool>> ToggleFavoriteAsync(Guid userId, FavoriteRequest request)
    {
        bool isStudent = await _context.Students.AnyAsync(s => s.Id == userId);

        if (isStudent && request.Type == FavoriteType.Student)
            return Result<bool>.Failure("Студенты не могут добавлять друг друга в избранное", StatusCodes.Status403Forbidden);

        if (!isStudent && (request.Type == FavoriteType.Vacancy || request.Type == FavoriteType.Employer))
            return Result<bool>.Failure("Работодатели могут добавлять в избранное только студентов", StatusCodes.Status403Forbidden);

        Guid targetOwnerId = request.Type switch
        {
            FavoriteType.Vacancy => await _context.Vacancies.Where(v => v.Id == request.TargetId).Select(v => v.EmployerId).FirstOrDefaultAsync(),
            FavoriteType.Student => request.TargetId,
            FavoriteType.Employer => request.TargetId,
            _ => Guid.Empty
        };

        if (targetOwnerId == Guid.Empty)
            return Result<bool>.Failure(ErrorMessages.EntityNotFound("Target"), StatusCodes.Status404NotFound);

        var blackListCheck = await EnsureCommunicationAllowedAsync(userId, targetOwnerId);
        
        if (!blackListCheck.IsSuccess)
            return Result<bool>.Failure(blackListCheck.ErrorMessage!, blackListCheck.StatusCode);

        var existingQuery = _context.Favorites.Where(f => f.UserId == userId);
        Favorite? existing = request.Type switch
        {
            FavoriteType.Vacancy => await existingQuery.FirstOrDefaultAsync(f => f.VacancyId == request.TargetId),
            FavoriteType.Student => await existingQuery.FirstOrDefaultAsync(f => f.StudentId == request.TargetId),
            FavoriteType.Employer => await existingQuery.FirstOrDefaultAsync(f => f.EmployerId == request.TargetId),
            _ => null
        };

        if (!request.IsFavorite)
        {
            if (existing != null)
            {
                _context.Favorites.Remove(existing);
                await SaveChangesAsync<Favorite>();
            }
            return Result<bool>.Success(false);
        }

        if (existing != null) return Result<bool>.Success(true);

        var favorite = new Favorite { UserId = userId };
        switch (request.Type)
        {
            case FavoriteType.Vacancy: favorite.VacancyId = request.TargetId; break;
            case FavoriteType.Student: favorite.StudentId = request.TargetId; break;
            case FavoriteType.Employer: favorite.EmployerId = request.TargetId; break;
        }

        _context.Favorites.Add(favorite);
        var result = await SaveChangesAsync<Favorite>();

        return result.IsSuccess ? Result<bool>.Success(true) : Result<bool>.Failure(result.ErrorMessage!);
    }
}